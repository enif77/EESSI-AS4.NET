﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Eu.EDelivery.AS4.ComponentTests.Common;
using Eu.EDelivery.AS4.Entities;
using Eu.EDelivery.AS4.Extensions;
using Eu.EDelivery.AS4.Model.Core;
using Eu.EDelivery.AS4.Model.Internal;
using Eu.EDelivery.AS4.TestUtils.Stubs;
using Xunit;
using RetryReliability = Eu.EDelivery.AS4.Entities.RetryReliability;

namespace Eu.EDelivery.AS4.ComponentTests.Agents
{
    public class RetryAgentForDeliveryFacts : ComponentTestTemplate
    {
        private readonly AS4Component _as4Msh;
        private readonly DatabaseSpy _databaseSpy;
        private readonly string _receiveAgentUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryAgentForDeliveryFacts"/> class.
        /// </summary>
        public RetryAgentForDeliveryFacts()
        {
            Settings settings = OverrideSettings("receive_deliver_agent_settings.xml");

            _as4Msh = AS4Component.Start(Environment.CurrentDirectory);
            _databaseSpy = DatabaseSpy.Create(_as4Msh.GetConfiguration());

            _receiveAgentUrl = 
                settings.Agents.ReceiveAgents.First()
                        .Receiver
                        .Setting.FirstOrDefault(s => s.Key == "Url")
                        ?.Value;

        }

        protected override void Disposing(bool isDisposing)
        {
            _as4Msh.Dispose();
        }

        [Fact]
        public async Task Message_Is_Set_To_Delivered_After_Its_Being_Retried()
        { 
            // Arrang
            AS4Message as4Message = CreateAS4Message();

            // Act
            await TestDeliverRetryByBlockingDeliveryLocationFor(as4Message, TimeSpan.FromSeconds(1));

            // Assert
            InMessage actual =
                await PollUntilPresent(
                    () => _databaseSpy.GetInMessageFor(m => m.Operation == Operation.Delivered),
                    timeout: TimeSpan.FromSeconds(10));

            // Assert
            Assert.Equal(InStatus.Delivered, actual.Status.ToEnum<InStatus>());
            Assert.Equal(Operation.Delivered, actual.Operation);

            RetryReliability rr = _databaseSpy.GetRetryReliabilityFor(r => r.RefToInMessageId == actual.Id);
            Assert.True(0 < rr.CurrentRetryCount, "0 < actualMessage.CurrentRetryCount");
            Assert.Equal(RetryStatus.Completed, rr.Status);
        }

        [Fact]
        public async Task Message_Is_Set_To_Exception_If_Delivery_Fails_After_Exhausted_Retries()
        {
            // Arrang
            AS4Message as4Message = CreateAS4Message();

            // Act
            await TestDeliverRetryByBlockingDeliveryLocationFor(as4Message, TimeSpan.FromSeconds(30));

            // Assert
            InMessage actual =
                await PollUntilPresent(
                    () => _databaseSpy.GetInMessageFor(m => m.Operation == Operation.DeadLettered),
                    timeout: TimeSpan.FromSeconds(10));

            RetryReliability rr = 
                _databaseSpy.GetRetryReliabilityFor(r => r.RefToInMessageId == actual.Id);

            Assert.Equal(rr.CurrentRetryCount, rr.MaxRetryCount);
            Assert.Equal(RetryStatus.Completed, rr.Status);
        }

        private async Task TestDeliverRetryByBlockingDeliveryLocationFor(AS4Message as4Message, TimeSpan period)
        {
            string deliverLocation = DeliverMessageLocationOf(as4Message);
            CleanDirectoryAt(Path.GetDirectoryName(deliverLocation));

            using (WriteBlockingFileTo(deliverLocation))
            {
                await StubSender.SendAS4Message(_receiveAgentUrl, as4Message);

                // Assert
                // Blocks the delivery location for a period of time
                await Task.Delay(period);
            }
        }

        private static AS4Message CreateAS4Message()
        {
            return AS4Message.Create(
                new UserMessage(
                    "user-" + Guid.NewGuid(),
                    new CollaborationInfo(
                        new AgreementReference("agreement", "receiveagent-retryreliability-pmode"),
                        new Service("eu:europa:services", "getting:started"),
                        action: "eu:sample:01",
                        conversationId: "1"),
                    new Party("Sender", new PartyId("org:eu:europa:as4:example:accesspoint:A")),
                    new Party("Receiver", new PartyId("org:eu:europa:as4:example:accesspoint:B")),
                    new PartInfo[0],
                    new MessageProperty[0]));
        }

        private static void CleanDirectoryAt(string location)
        {
            foreach (FileInfo file in new DirectoryInfo(location).EnumerateFiles())
            {
                file.Delete();
            }
        }

        private static IDisposable WriteBlockingFileTo(string deliverLocation)
        {
            var fileStream = new FileStream(deliverLocation, FileMode.CreateNew);
            var streamWriter = new StreamWriter(fileStream);

            streamWriter.Write("<blocking content>");

            return streamWriter;
        }

        private static string DeliverMessageLocationOf(AS4Message as4Message)
        {
            return Path.Combine(Environment.CurrentDirectory, @"messages\in", as4Message.GetPrimaryMessageId() + ".xml");
        }
    }
}
