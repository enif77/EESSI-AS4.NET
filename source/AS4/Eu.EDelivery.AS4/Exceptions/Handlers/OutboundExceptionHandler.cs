﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Eu.EDelivery.AS4.Common;
using Eu.EDelivery.AS4.Entities;
using Eu.EDelivery.AS4.Model.Core;
using Eu.EDelivery.AS4.Model.Internal;
using Eu.EDelivery.AS4.Repositories;
using Eu.EDelivery.AS4.Serialization;
using Eu.EDelivery.AS4.Streaming;
using NLog;

namespace Eu.EDelivery.AS4.Exceptions.Handlers
{
    public class OutboundExceptionHandler : IAgentExceptionHandler
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly Func<DatastoreContext> _createContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboundExceptionHandler"/> class.
        /// </summary>
        public OutboundExceptionHandler() : this(Registry.Instance.CreateDatastoreContext) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboundExceptionHandler" /> class.
        /// </summary>
        /// <param name="createContext">The create context.</param>
        public OutboundExceptionHandler(Func<DatastoreContext> createContext)
        {
            _createContext = createContext;
        }

        /// <summary>
        /// Handles the transformation exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        public async Task<MessagingContext> HandleTransformationException(Exception exception, Stream contents)
        {
            Logger.Error($"Exception occured during transformation: {exception.Message}");

            await SideEffectUsageRepository(
                repository =>
                {
                    OutException outException = CreateMinimumOutException(exception);
                    outException.MessageBody = contents.ToBytes();

                    repository.InsertOutException(outException);
                });

            return new MessagingContext(exception);
        }

        private static OutException CreateMinimumOutException(Exception exception)
        {
            return new OutException
            {
                Exception = exception.Message,
                InsertionTime = DateTimeOffset.Now,
                ModificationTime = DateTimeOffset.Now
            };
        }

        /// <summary>
        /// Handles the error exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task<MessagingContext> HandleErrorException(Exception exception, MessagingContext context)
        {
            Logger.Error($"Exception occured while executing Error Pipeline: {exception.Message}");
            return await HandleExecutionException(exception, context);
        }

        /// <summary>
        /// Handles the execution exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="context">The message context.</param>
        /// <returns></returns>
        public async Task<MessagingContext> HandleExecutionException(Exception exception, MessagingContext context)
        {
            Logger.Error($"Exception occured while executing Steps:{exception.Message}");           

            string ebmsMessageId = await GetEbmsMessageId(context);

            if (string.IsNullOrEmpty(ebmsMessageId) == false)
            {
                await SideEffectUsageRepository(
                    repository =>
                    {
                        repository.UpdateOutMessage(ebmsMessageId, m => m.Status = OutStatus.Exception);

                        OutException outException = CreateOutExceptionWithContextInfo(exception, context);
                        outException.EbmsRefToMessageId = ebmsMessageId;

                        repository.InsertOutException(outException);
                    });
            }
            else
            {
                await SideEffectUsageRepository(
                    repository =>
                    {
                        OutException ex = CreateOutExceptionWithContextInfo(exception, context);
                        ex.MessageBody = AS4XmlSerializer.TryToXmlBytesAsync(context.SubmitMessage).Result;

                        repository.InsertOutException(ex);
                    });
            }

            return new MessagingContext(exception);
        }
        
        private static OutException CreateOutExceptionWithContextInfo(Exception exception, MessagingContext context)
        {
            OutException outException = CreateMinimumOutException(exception);

            outException.PMode = AS4XmlSerializer.ToString(context.SendingPMode);
            outException.Operation =
                context.SendingPMode?.ExceptionHandling?.NotifyMessageProducer == true
                    ? Operation.ToBeNotified
                    : default(Operation);

            return outException;
        }

        private async Task SideEffectUsageRepository(Action<DatastoreRepository> usage)
        {
            using (DatastoreContext context = _createContext())
            {
                var repository = new DatastoreRepository(context);
                usage(repository);

                await context.SaveChangesAsync();
            }
        }

        private static async Task<string> GetEbmsMessageId(MessagingContext context)
        {
            string ebmsMessageId = context.EbmsMessageId;

            if (String.IsNullOrWhiteSpace(ebmsMessageId) && context.ReceivedMessage != null)
            {
                var as4Message = await TryDeserialize(context.ReceivedMessage);
                ebmsMessageId = as4Message?.GetPrimaryMessageId();
            }

            return ebmsMessageId;
        }

        private static async Task<AS4Message> TryDeserialize(ReceivedMessage message)
        {
            var serializer = SerializerProvider.Default.Get(message.ContentType);
            try
            {
                message.UnderlyingStream.Position = 0;
                var as4Message = await serializer.DeserializeAsync(message.UnderlyingStream, message.ContentType, CancellationToken.None);

                return as4Message;
            }
            catch
            {
                return null;
            }
        }
    }
}
