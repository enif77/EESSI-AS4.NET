﻿using Eu.EDelivery.AS4.Common;
using Eu.EDelivery.AS4.ComponentTests.Common;
using Eu.EDelivery.AS4.Entities;
using Eu.EDelivery.AS4.Model.Core;
using Eu.EDelivery.AS4.Model.PMode;
using Eu.EDelivery.AS4.Repositories;
using Eu.EDelivery.AS4.Serialization;
using Eu.EDelivery.AS4.Xml;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Encryption = Eu.EDelivery.AS4.Model.PMode.Encryption;
using Party = Eu.EDelivery.AS4.Model.Core.Party;
using PartyId = Eu.EDelivery.AS4.Model.Core.PartyId;
using Receipt = Eu.EDelivery.AS4.Model.Core.Receipt;
using Signing = Eu.EDelivery.AS4.Model.PMode.Signing;
using UserMessage = Eu.EDelivery.AS4.Model.Core.UserMessage;

namespace Eu.EDelivery.AS4.ComponentTests.Agents
{
    public class OutboundProcessingAgentFacts : ComponentTestTemplate
    {
        private readonly AS4Component _msh;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboundProcessingAgentFacts"/> class.
        /// </summary>
        public OutboundProcessingAgentFacts()
        {
            OverrideSettings("outboundprocessingagent_settings.xml");
            _msh = AS4Component.Start(Environment.CurrentDirectory);
        }

        [Fact]
        public async Task Compressed_Signed_Encrypted_UserMessage_Gets_Processed_With_Multihop_Information()
        {
            AS4Message as4Message = await ProcessedMessageIsSetToSentWithoutAlteringMultihopInformation(
                CreateUserMessage,
                (useCompression: true, signing: Signed(), encryption: Encrypted()));

            Assert.All(
                as4Message.UserMessages.SelectMany(u => u.PayloadInfo),
                p => Assert.Equal("application/gzip", p.CompressionType));
        }

        [Fact]
        public async Task Uncompressed_Unsigned_Unencrypted_UserMessage_Gets_Processed_With_Multihop_Information()
        {
            await ProcessedMessageIsSetToSentWithoutAlteringMultihopInformation(
                CreateUserMessage,
                (useCompression: false, signing: Unsigned(), encryption: Unencrypted()));
        }

        [Fact]
        public async Task Compressed_Signed_Unencrypted_UserMessage_Gets_Processed_With_Multihop_Information()
        {
            AS4Message as4Message = await ProcessedMessageIsSetToSentWithoutAlteringMultihopInformation(
                CreateUserMessage,
                (useCompression: true, signing: Signed(), encryption: Unencrypted()));

            Assert.All(
                as4Message.UserMessages.SelectMany(u => u.PayloadInfo),
                p => Assert.Equal("application/gzip", p.CompressionType));
        }

        [Fact]
        public async Task Uncompressed_Unsigned_Encrytped_UserMessage_Gets_Processed_With_Multihop_Information()
        {
            await ProcessedMessageIsSetToSentWithoutAlteringMultihopInformation(
                CreateUserMessage,
                (useCompression: false, signing: Unsigned(), encryption: Encrypted()));
        }

        [Fact]
        public async Task Compressed_Signed_Unencrypted_SignalMessage_Gets_Processed_With_Multihop_Information()
        {
            AS4Message as4Message = await ProcessedMessageIsSetToSentWithoutAlteringMultihopInformation(
                CreateSignalMessage,
                (useCompression: true, signing: Signed(), encryption: Unencrypted()));

            Assert.All(
                as4Message.UserMessages.SelectMany(u => u.PayloadInfo),
                p => Assert.Equal("application/gzip", p.CompressionType));
        }

        [Fact]
        public async Task Uncompressed_Unsigned_Unencrypted_SignalMessage_Gets_Processed_With_Multihop_Information()
        {
            await ProcessedMessageIsSetToSentWithoutAlteringMultihopInformation(
                CreateSignalMessage,
                (useCompression: false, signing: Unsigned(), encryption: Unencrypted()));
        }

        private async Task<AS4Message> ProcessedMessageIsSetToSentWithoutAlteringMultihopInformation(
            Func<SendingProcessingMode, AS4Message> createMessage,
            (bool useCompression, Signing signing, Encryption encryption) pmodeInfo)
        {
            // Arrange
            var multihopPMode = new SendingProcessingMode
            {
                MessagePackaging =
                {
                    IsMultiHop = true,
                    UseAS4Compression = pmodeInfo.useCompression
                },
                Security =
                {
                    Signing = pmodeInfo.signing,
                    Encryption = pmodeInfo.encryption
                },
                Reliability =
                {
                    ReceptionAwareness =
                    {
                        IsEnabled = true
                    }
                }
            };
            AS4Message multihopMessage = createMessage(multihopPMode);

            var datastoreSpy = DatabaseSpy.Create(_msh.GetConfiguration());
            OutMessage tobeProcessedEntry = CreateToBeProcessedOutMessage(multihopPMode, multihopMessage, asIntermediary: true);

            // Act
            datastoreSpy.InsertOutMessage(tobeProcessedEntry);

            // Assert
            OutMessage processedEntry =
                await PollUntilPresent(
                    () => datastoreSpy.GetOutMessageFor(
                        m => m.EbmsMessageId == multihopMessage.GetPrimaryMessageId()
                             && m.Operation == Operation.ToBeSent),
                    timeout: TimeSpan.FromSeconds(30));

            AS4Message processedMessage =
                await DeserializeOutMessageBody(Registry.Instance.MessageBodyStore, processedEntry);

            Assert.True(processedMessage.IsMultiHopMessage);
            Assert.True(
                datastoreSpy.GetRetryReliabilityFor(
                    r => r.RefToOutMessageId == processedEntry.Id) == null,
                "No 'RetryReliability' record should be inserted when receiving multihop AS4Message");

            return processedMessage;
        }

        private static AS4Message CreateUserMessage(SendingProcessingMode pmode)
        {
            var msg = AS4Message.Create(
                pmode: pmode,
                message: new UserMessage(
                    "test-" + Guid.NewGuid(),
                    new Party("sender-role", new PartyId("sender-id")),
                    new Party("receiver-role", new PartyId("receiver-id"))));

            msg.AddAttachment(
                new Attachment(
                    id: "test-" + Guid.NewGuid(),
                    content: new MemoryStream(Encoding.UTF8.GetBytes("my content!")),
                    contentType: "text/plain"));

            return msg;
        }

        private static AS4Message CreateSignalMessage(SendingProcessingMode pmode)
        {
            var routedUserMessage = new RoutingInputUserMessage
            {
                mpc = "some-mpc",
                PartyInfo = new Xml.PartyInfo
                {
                    To = new To
                    {
                        PartyId = new[] { new Xml.PartyId { Value = "org:eu:europa:as4:example:accesspoint:B" } },
                        Role = "Receiver"
                    },
                    From = new From
                    {
                        PartyId = new[] { new Xml.PartyId { Value = "org:eu:europa:as4:example:accesspoint:A" } },
                        Role = "Sender"
                    }
                },
                CollaborationInfo = new Xml.CollaborationInfo
                {
                    Action = "OutboundProcessing_Action",
                    Service = new Xml.Service { Value = "OutboundProcessing_Service", type = "eu:europa:services" }
                }
            };
            var receipt = new Receipt(
                $"receipt-{Guid.NewGuid()}",
                $"user-{Guid.NewGuid()}",
                new Model.Core.NonRepudiationInformation(new Reference[0]),
                routedUserMessage);

            return AS4Message.Create(receipt, pmode);
        }

        private static Signing Signed()
        {
            return new Signing
            {
                IsEnabled = true,
                CertificateType = PrivateKeyCertificateChoiceType.CertificateFindCriteria,
                SigningCertificateInformation = new CertificateFindCriteria
                {
                    CertificateFindType = X509FindType.FindBySubjectName,
                    CertificateFindValue = "AccessPointA"
                }
            };
        }

        private static Signing Unsigned()
        {
            return new Signing { IsEnabled = false };
        }

        private static Encryption Encrypted()
        {
            return new Encryption
            {
                IsEnabled = true,
                CertificateType = PublicKeyCertificateChoiceType.CertificateFindCriteria,
                EncryptionCertificateInformation = new CertificateFindCriteria
                {
                    CertificateFindType = X509FindType.FindBySubjectName,
                    CertificateFindValue = "AccessPointB"
                }
            };
        }

        private static Encryption Unencrypted()
        {
            return new Encryption { IsEnabled = false };
        }

        private OutMessage CreateToBeProcessedOutMessage(IPMode pmode, AS4Message msg, bool asIntermediary)
        {
            IAS4MessageBodyStore bodyStore = Registry.Instance.MessageBodyStore;
            string location = _msh.GetConfiguration().OutMessageStoreLocation;

            var tobeProcessedEntry = new OutMessage(msg.GetPrimaryMessageId())
            {
                ContentType = msg.ContentType,
                MessageLocation = bodyStore.SaveAS4Message(location, msg)
            };
            tobeProcessedEntry.Intermediary = asIntermediary;
            tobeProcessedEntry.Operation = Operation.ToBeProcessed;
            tobeProcessedEntry.SetPModeInformation(pmode);

            return tobeProcessedEntry;
        }

        private static async Task<AS4Message> DeserializeOutMessageBody(
            IAS4MessageBodyStore bodyStore,
            OutMessage processedEntry)
        {
            using (Stream output = await bodyStore.LoadMessageBodyAsync(processedEntry.MessageLocation))
            {
                return await SerializerProvider.Default
                    .Get(processedEntry.ContentType)
                    .DeserializeAsync(output, processedEntry.ContentType);
            }
        }

        protected override void Disposing(bool isDisposing)
        {
            _msh.Dispose();
        }
    }
}
