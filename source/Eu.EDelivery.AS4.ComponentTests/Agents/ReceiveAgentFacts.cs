﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Eu.EDelivery.AS4.Common;
using Eu.EDelivery.AS4.ComponentTests.Common;
using Eu.EDelivery.AS4.ComponentTests.Extensions;
using Eu.EDelivery.AS4.Entities;
using Eu.EDelivery.AS4.Exceptions;
using Eu.EDelivery.AS4.Extensions;
using Eu.EDelivery.AS4.Model.Core;
using Eu.EDelivery.AS4.Model.Internal;
using Eu.EDelivery.AS4.Model.PMode;
using Eu.EDelivery.AS4.Security.Encryption;
using Eu.EDelivery.AS4.Security.References;
using Eu.EDelivery.AS4.Security.Signing;
using Eu.EDelivery.AS4.Serialization;
using Eu.EDelivery.AS4.TestUtils;
using Eu.EDelivery.AS4.TestUtils.Stubs;
using Eu.EDelivery.AS4.Xml;
using Xunit;
using static Eu.EDelivery.AS4.ComponentTests.Properties.Resources;
using AgreementReference = Eu.EDelivery.AS4.Model.Core.AgreementReference;
using CollaborationInfo = Eu.EDelivery.AS4.Model.Core.CollaborationInfo;
using Error = Eu.EDelivery.AS4.Model.Core.Error;
using MessageProperty = Eu.EDelivery.AS4.Model.Core.MessageProperty;
using NonRepudiationInformation = Eu.EDelivery.AS4.Model.Core.NonRepudiationInformation;
using Parameter = Eu.EDelivery.AS4.Model.PMode.Parameter;
using PartInfo = Eu.EDelivery.AS4.Model.Core.PartInfo;
using Party = Eu.EDelivery.AS4.Model.Core.Party;
using PartyId = Eu.EDelivery.AS4.Model.Core.PartyId;
using Receipt = Eu.EDelivery.AS4.Model.Core.Receipt;
using Service = Eu.EDelivery.AS4.Model.Core.Service;
using SignalMessage = Eu.EDelivery.AS4.Model.Core.SignalMessage;
using UserMessage = Eu.EDelivery.AS4.Model.Core.UserMessage;

namespace Eu.EDelivery.AS4.ComponentTests.Agents
{
    public class ReceiveAgentFacts : ComponentTestTemplate
    {
        private readonly AS4Component _as4Msh;
        private readonly DatabaseSpy _databaseSpy;
        private readonly string _receiveAgentUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiveAgentFacts" /> class.
        /// </summary>
        public ReceiveAgentFacts()
        {
            Settings receiveSettings = OverrideSettings("receiveagent_http_settings.xml");

            _as4Msh = AS4Component.Start(Environment.CurrentDirectory);

            _databaseSpy = DatabaseSpy.Create(_as4Msh.GetConfiguration());

            _receiveAgentUrl = receiveSettings.Agents.ReceiveAgents.First().Receiver.Setting
                                              .FirstOrDefault(s => s.Key == "Url")
                                              ?.Value;

            Assert.False(
                string.IsNullOrWhiteSpace(_receiveAgentUrl),
                "The URL where the receive agent is listening on, could not be retrieved.");
        }

        #region Scenario where ReceiveAgent receives invalid Messages (no AS4 Messages)

        [Fact]
        public async Task ThenAgentReturnsBadRequest_IfReceivedMessageIsNotAS4Message()
        {
            // Arrange
            byte[] content = Encoding.UTF8.GetBytes(Convert.ToBase64String(receiveagent_message));

            // Act
            HttpResponseMessage response = await StubSender.SendRequest(_receiveAgentUrl, content,
                                                                        "multipart/related; boundary=\"=-C3oBZDXCy4W2LpjPUhC4rw==\"; type=\"application/soap+xml\"; charset=\"utf-8\"");
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Scenario's for received UserMessages that result in errors.

        [Fact]
        public async Task ThenAgentReturnsError_IfMessageHasNonExsistingAttachment()
        {
            // Arrange
            byte[] content = receiveagent_message_nonexist_attachment;

            // Act
            HttpResponseMessage response = await StubSender.SendRequest(_receiveAgentUrl, content,
                                                                        "multipart/related; boundary=\"=-C3oBZDXCy4W2LpjPUhC4rw==\"; type=\"application/soap+xml\"; charset=\"utf-8\"");
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            AS4Message as4Message = await response.DeserializeToAS4Message();
            Assert.IsType<Error>(as4Message.PrimaryMessageUnit);
        }

        [Fact]
        public async Task ThenAgentReturnsError_IfReceivingPModeIsNotValid()
        {
            const string messageId = "some-message-id";

            // Arrange
            var message = AS4Message.Create(
                new UserMessage(
                    messageId: messageId,
                    collaboration: new CollaborationInfo(
                        agreement: new AgreementReference("http://agreements.europa.org/agreement"),
                        service: new Model.Core.Service(
                            value: "Invalid_PMode_Test_Service",
                            type: "eu:europa:services"),
                        action: "Invalid_PMode_Test_Action",
                        conversationId: CollaborationInfo.DefaultConversationId),
                    sender: new Model.Core.Party("Sender", new PartyId("org:eu:europa:as4:example:accesspoint:A")),
                    receiver: new Model.Core.Party("Receiver", new PartyId("org:eu:europa:as4:example:accesspoint:B")),
                    partInfos: new Model.Core.PartInfo[0],
                    messageProperties: new Model.Core.MessageProperty[0]));

            // Act
            HttpResponseMessage response = await StubSender.SendAS4Message(_receiveAgentUrl, message);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            AS4Message result = await SerializerProvider.Default
                .Get(Constants.ContentTypes.Soap)
                .DeserializeAsync(await response.Content.ReadAsStreamAsync(), Constants.ContentTypes.Soap);

            var errorMsg = result.FirstSignalMessage as Error;
            Assert.NotNull(errorMsg);
            Assert.Collection(
                errorMsg.ErrorLines,
                e => Assert.Equal(ErrorCode.Ebms0010, e.ErrorCode));

            InMessage inMessageRecord = _databaseSpy.GetInMessageFor(m => m.EbmsMessageId == messageId);
            Assert.Equal(InStatus.Received, inMessageRecord.Status.ToEnum<InStatus>());
        }

        [Fact]
        public async Task ReturnsErrorMessageWhenDecryptionCertificateCannotBeFound()
        {
            var userMessage = new UserMessage(
                Guid.NewGuid().ToString(),
                new CollaborationInfo(
                    agreement: new AgreementReference(
                        value: "http://agreements.europa.org/agreement",
                        pmodeId: "receiveagent-non_existing_decrypt_cert-pmode"),
                    service: new Service(
                        value: "errorhandling",
                        type: "as4.net:receive_agent:componenttest"),
                    action: "as4.net:receive_agent:decryption_failed",
                    conversationId: "as4.net:receive_agent:conversation"));

            var as4Message = CreateAS4MessageWithAttachment(userMessage);

            var encryptedMessage = AS4MessageUtils.EncryptWithCertificate(as4Message, new StubCertificateRepository().GetStubCertificate());

            // Act
            HttpResponseMessage response = await StubSender.SendAS4Message(_receiveAgentUrl, encryptedMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var contentType = response.Content.Headers.ContentType.MediaType;
            var result = await SerializerProvider.Default.Get(contentType)
                                        .DeserializeAsync(await response.Content.ReadAsStreamAsync(), contentType);

            Assert.True(result.IsSignalMessage);

            var errorMessage = result.FirstSignalMessage as Error;
            Assert.NotNull(errorMessage);
            Assert.Equal(ErrorCode.Ebms0102, errorMessage.ErrorLines.First().ErrorCode);
        }

        private static AS4Message CreateAS4MessageWithAttachment(UserMessage msg)
        {
            var as4Message = AS4Message.Create(msg);

            // Arrange
            byte[] attachmentContents = Encoding.UTF8.GetBytes("some random attachment");
            var attachment = new Attachment("attachment-id", new MemoryStream(attachmentContents), "text/plain");

            as4Message.AddAttachment(attachment);

            return as4Message;
        }

        #endregion

        #region MessageHandling scenarios

        [Fact]
        public async Task ThenInMessageSignalIsStoredWithPModeUrl()
        {
            // Arrange
            var userMessage = new UserMessage(
                $"user-{Guid.NewGuid()}",
                new CollaborationInfo(
                    new AgreementReference(
                        "http://eu.europe.agreements",
                        "callback-pmode")));

            // Act
            await StubSender.SendAS4Message(_receiveAgentUrl, AS4Message.Create(userMessage));

            // Assert
            OutMessage storedReceipt =
                await PollUntilPresent(
                    () => _databaseSpy.GetOutMessageFor(m => m.EbmsRefToMessageId == userMessage.MessageId
                                                            && m.EbmsMessageType == MessageType.Receipt),
                    timeout: TimeSpan.FromSeconds(20));

            ReceivingProcessingMode pmode = _as4Msh.GetConfiguration().GetReceivingPModes().First(p => p.Id == "callback-pmode");
            Assert.Equal(pmode.ReplyHandling.ResponseConfiguration.Protocol.Url, storedReceipt.Url);
        }

        [Fact]
        public async Task ThenInMessageOperationIsToBeDelivered()
        {
            // Arrange
            byte[] content = receiveagent_message;

            // Act
            HttpResponseMessage response = await StubSender.SendRequest(_receiveAgentUrl, content,
                                                                        "multipart/related; boundary=\"=-C3oBZDXCy4W2LpjPUhC4rw==\"; type=\"application/soap+xml\"; charset=\"utf-8\"");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            AS4Message receivedAS4Message = await response.DeserializeToAS4Message();
            Assert.IsType<Receipt>(receivedAS4Message.PrimaryMessageUnit);

            InMessage receivedUserMessage = GetInsertedUserMessageFor(receivedAS4Message);
            Assert.NotNull(receivedUserMessage);
            Assert.Equal(Operation.ToBeDelivered, receivedUserMessage.Operation);
        }

        private InMessage GetInsertedUserMessageFor(AS4Message receivedAS4Message)
        {
            return
                _databaseSpy.GetInMessageFor(
                    i => i.EbmsMessageId.Equals(receivedAS4Message.FirstSignalMessage.RefToMessageId));
        }

        [Fact]
        public async Task ThenInMessageOperationIsToBeForwarded()
        {
            const string messageId = "forwarding_message_id";

            var as4Message = AS4Message.Create(
                new UserMessage(
                    messageId,
                    new CollaborationInfo(
                    agreement: new AgreementReference(
                        value: "forwarding/agreement",
                        type: "forwarding",
                        // Make sure that the forwarding receiving pmode is used; therefore
                        // explicitly set the Id of the PMode that must be used by the receive-agent.
                        pmodeId: "Forward_Push"),
                    service: new Service(
                        value: "Forward_Push_Service",
                        type: "eu:europa:services"),
                    action: "Forward_Push_Action",
                    conversationId: "eu:europe:conversation")));

            // Act
            HttpResponseMessage response = await StubSender.SendAS4Message(_receiveAgentUrl, as4Message);

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.True(String.IsNullOrWhiteSpace(await response.Content.ReadAsStringAsync()));

            InMessage receivedUserMessage = _databaseSpy.GetInMessageFor(m => m.EbmsMessageId == messageId);
            Assert.NotNull(receivedUserMessage);
            Assert.Equal(Operation.ToBeForwarded, receivedUserMessage.Operation);
        }

        [Fact]
        public async Task ReturnsEmptyMessageFromInvalidMessage_IfReceivePModeIsCallback()
        {
            // Act
            HttpResponseMessage response = await StubSender.SendRequest(_receiveAgentUrl, receiveagent_wrong_encrypted_message,
                                                                        "multipart/related; boundary=\"=-WoWSZIFF06iwFV8PHCZ0dg==\"; type=\"application/soap+xml\"; charset=\"utf-8\"");

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.Empty(await response.Content.ReadAsStringAsync());
        }

        [Theory]
        [InlineData(X509ReferenceType.BSTReference)]
        [InlineData(X509ReferenceType.KeyIdentifier)]
        [InlineData(X509ReferenceType.IssuerSerial)]
        public async Task Correctly_Verifies_Encrypted_And_Signed_Message_With_SecurityTokenReference(
            X509ReferenceType securityTokenReferenceType)
        {
            // Arrange
            var pdf = new Attachment("pdf", new MemoryStream(pdf_document), "application/pdf");
            var xml = new Attachment("xml", new MemoryStream(Encoding.UTF8.GetBytes("<Root>Don't modify me</Root>")), "application/xml");
            var as4Message = AS4Message.Create(
                new UserMessage(
                    $"user-{Guid.NewGuid()}",
                    new CollaborationInfo(
                        new AgreementReference("http://agreements.europa.org/agreement"),
                        new Service("getting:started", "org:europa:services"),
                        "eu:sample:03",
                        "eu:edelivery:as4:sampleconversation"),
                    new Party("Sender", new PartyId("org:eu:europa:as4:example:accesspoint:A")),
                    new Party("Receiver", new PartyId("org:eu:europa:as4:example:accesspoint:B")),
                    new[]
                    {
                        PartInfo.CreateFor(pdf),
                        PartInfo.CreateFor(xml)
                    },
                    Enumerable.Empty<MessageProperty>()));

            as4Message.AddAttachments(new[] { pdf, xml });
            as4Message.Sign(
                new CalculateSignatureConfig(
                    Registry.Instance.CertificateRepository.GetCertificate(
                        X509FindType.FindBySubjectName,
                        "AccessPointA"),
                    securityTokenReferenceType,
                    Constants.SignAlgorithms.Sha256,
                    Constants.HashFunctions.Sha256));

            as4Message.Encrypt(
                new KeyEncryptionConfiguration(
                    Registry.Instance.CertificateRepository.GetCertificate(
                        X509FindType.FindBySubjectName,
                        "AccessPointB")),
                DataEncryptionConfiguration.Default);

            // Act
            HttpResponseMessage response = await StubSender.SendAS4Message(_receiveAgentUrl, as4Message);

            // Assert
            AS4Message message = await response.DeserializeToAS4Message();
            Assert.False(
                message.PrimaryMessageUnit is Error,
                (message.PrimaryMessageUnit as Error)?.FormatErrorLines());

            Assert.IsType<Receipt>(message.PrimaryMessageUnit);
        }

        #endregion

        #region SignalMessage receive scenario's

        [Fact]
        public async Task ThenRelatedUserMessageIsAcked()
        {
            // Arrange
            const string expectedId = "message-id";
            StoreToBeAckOutMessage(expectedId, CreateSendingPMode());

            AS4Message as4Message = CreateAS4ReceiptMessage(expectedId);

            // Act
            var response = await StubSender.SendAS4Message(_receiveAgentUrl, as4Message);

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.True(String.IsNullOrWhiteSpace(await response.Content.ReadAsStringAsync()), "An empty response was expected");

            AssertIfStatusOfOutMessageIs(expectedId, OutStatus.Ack);
            AssertIfInMessageExistsForSignalMessage(expectedId);
        }

        private static AS4Message CreateAS4ReceiptMessage(string refToMessageId)
        {
            var r = new Receipt($"receipt-{Guid.NewGuid()}", refToMessageId);

            return AS4Message.Create(r, CreateSendingPMode());
        }

        [Fact]
        public async Task ThenRelatedUserMessageIsNotAcked()
        {
            // Arrange
            const string expectedId = "message-id";

            StoreToBeAckOutMessage(expectedId, CreateSendingPMode());

            AS4Message as4Message = CreateAS4ErrorMessage(expectedId);

            // Act
            var response = await StubSender.SendAS4Message(_receiveAgentUrl, as4Message);

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.True(String.IsNullOrWhiteSpace(await response.Content.ReadAsStringAsync()), "An empty response was expected");

            AssertIfStatusOfOutMessageIs(expectedId, OutStatus.Nack);
            AssertIfInMessageExistsForSignalMessage(expectedId);
        }

        [Fact]
        public async Task ThenResponseWithAccepted_IfNRReceiptHasValidHashes()
        {
            // Arrange
            string ebmsMessageId = Guid.NewGuid().ToString();

            // Act
            HttpResponseMessage response = await TestSendNRReceiptWith(ebmsMessageId, hash => hash);

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Fact]
        public async Task ThenResponseWithError_IfNRReceiptHasInvalidHashes()
        {
            // Arrange
            string ebmsMessageId = Guid.NewGuid().ToString();
            int CorruptHash(int hash) => hash + 10;

            // Act
            HttpResponseMessage response = await TestSendNRReceiptWith(ebmsMessageId, CorruptHash);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            InMessage insertedReceipt = _databaseSpy.GetInMessageFor(m => m.EbmsRefToMessageId == ebmsMessageId);
            Assert.Equal(InStatus.Exception, insertedReceipt.Status.ToEnum<InStatus>());
            Assert.NotEmpty(_databaseSpy.GetInExceptions(m => m.EbmsRefToMessageId == insertedReceipt.EbmsMessageId));
        }

        private async Task<HttpResponseMessage> TestSendNRReceiptWith(string messageId, Func<int, int> selection)
        {
            // Arrange
            var nrrPMode = new SendingProcessingMode { Id = "verify-nrr", ReceiptHandling = { VerifyNRR = true } };
            X509Certificate2 cert = new StubCertificateRepository().GetStubCertificate();

            AS4Message signedUserMessage = SignedUserMessage(messageId, nrrPMode, cert);
            InsertRelatedSignedUserMessage(nrrPMode, signedUserMessage);

            AS4Message signedReceipt = SignedNRReceipt(cert, signedUserMessage, selection);

            // Act
            return await StubSender.SendAS4Message(_receiveAgentUrl, signedReceipt);

        }

        private static AS4Message SignedUserMessage(string messageId, SendingProcessingMode nrrPMode, X509Certificate2 cert)
        {
            AS4Message userMessage = AS4Message.Create(new UserMessage(messageId), nrrPMode);
            userMessage.AddAttachment(
                new Attachment(
                    id: "payload",
                    content: new MemoryStream(Encoding.UTF8.GetBytes("some content!")),
                    contentType: "text/plain"));

            return AS4MessageUtils.SignWithCertificate(userMessage, cert);
        }

        private void InsertRelatedSignedUserMessage(IPMode nrrPMode, AS4Message signedUserMessage)
        {
            string location = Registry.Instance.MessageBodyStore
                .SaveAS4Message(_as4Msh.GetConfiguration().OutMessageStoreLocation, signedUserMessage);

            var outMessage = new OutMessage(signedUserMessage.GetPrimaryMessageId())
            {
                ContentType = signedUserMessage.ContentType,
                MessageLocation = location,

            };
            outMessage.SetPModeInformation(nrrPMode);

            _databaseSpy.InsertOutMessage(outMessage);
        }

        private static AS4Message SignedNRReceipt(X509Certificate2 cert, AS4Message signedUserMessage, Func<int, int> selection)
        {
            IEnumerable<Reference> hashes =
                signedUserMessage
                    .SecurityHeader
                    .GetReferences()
                    .Select(r =>
                    {
                        r.DigestValue = r.DigestValue.Select(v => (byte)selection(v)).ToArray();
                        return Reference.CreateFromReferenceElement(r);
                    });

            AS4Message receipt = AS4Message.Create(
                new Receipt(
                    messageId: $"receipt-{Guid.NewGuid()}",
                    refToMessageId: signedUserMessage.GetPrimaryMessageId(),
                    nonRepudiation: new NonRepudiationInformation(hashes)));

            return AS4MessageUtils.SignWithCertificate(receipt, cert);
        }

        [Fact]
        public async Task OnInvalidReceipt_ExceptionIsLogged()
        {
            string userMessageId = Guid.NewGuid().ToString();

            var receiptString = Encoding.UTF8.GetString(receipt_with_invalid_signature).Replace("{{RefToMessageId}}", userMessageId);

            StoreToBeAckOutMessage(userMessageId, CreateSendingPMode());

            var response = await StubSender.SendRequest(_receiveAgentUrl, Encoding.UTF8.GetBytes(receiptString), "application/soap+xml");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);

            var inMessage = _databaseSpy.GetInMessageFor(m => m.EbmsRefToMessageId == userMessageId);
            Assert.NotNull(inMessage);
            Assert.Equal(MessageType.Receipt, inMessage.EbmsMessageType);
            Assert.Equal(InStatus.Exception, inMessage.Status.ToEnum<InStatus>());

            var inExceptions = _databaseSpy.GetInExceptions(m => m.EbmsRefToMessageId == inMessage.EbmsMessageId);
            Assert.NotNull(inExceptions);
            Assert.NotEmpty(inExceptions);

            var outMessage = _databaseSpy.GetOutMessageFor(m => m.EbmsRefToMessageId == userMessageId);
            Assert.True(outMessage == null, "No OutMessage should be created for the received SignalMessage");
        }

        [Fact]
        public async Task ThenReceivedMultihopUserMessageIsSetAsIntermediaryAndForwarded()
        {
            // Arrange
            var userMessage = new UserMessage(
                "test-" + Guid.NewGuid(),
                new
                    CollaborationInfo(
                        agreement: new AgreementReference(
                            value: "http://agreements.europa.org/agreement",
                            pmodeId: "Forward_Push_Multihop"),
                        service: new Service(
                            value: "Forward_Push_Multihop_Service",
                            type: "eu:europa:services"),
                        action: "Forward_Push_Multihop_Action",
                        conversationId: "eu:europe:conversation"));
            var multihopPMode = new SendingProcessingMode { MessagePackaging = { IsMultiHop = true } };
            AS4Message multihopMessage = AS4Message.Create(userMessage, multihopPMode);

            // Act
            HttpResponseMessage response = await StubSender.SendAS4Message(_receiveAgentUrl, multihopMessage);

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

            InMessage inUserMessage = _databaseSpy.GetInMessageFor(m => m.EbmsMessageId == userMessage.MessageId);

            Assert.NotNull(inUserMessage);
            Assert.True(inUserMessage.Intermediary);
            Assert.Equal(Operation.ToBeForwarded, inUserMessage.Operation);
        }

        [Fact]
        public async Task ThenReceivedMultihopUserMessageIsntSetToIntermediaryButDeliveredWithCorrespondingSentReceipt()
        {
            // Arrange
            var userMessage = new UserMessage(
                "test-" + Guid.NewGuid(),
                new CollaborationInfo(
                    agreement: new AgreementReference(
                        value: "http://agreements.europa.org/agreement",
                        pmodeId: "ComponentTest_ReceiveAgent_Sample1"),
                    service: new Model.Core.Service(
                        value: "getting:started",
                        type: "eu:europa:services"),
                    action: "eu:sample:01",
                    conversationId: "eu:europa:conversation"));
            var multihopPMode = new SendingProcessingMode { MessagePackaging = { IsMultiHop = true } };
            AS4Message multihopMessage = AS4Message.Create(userMessage, multihopPMode);

            // Act
            HttpResponseMessage response = await StubSender.SendAS4Message(_receiveAgentUrl, multihopMessage);

            // Assert
            AS4Message responseReceipt = await response.DeserializeToAS4Message();
            AssertMessageMultihopAttributes(responseReceipt.EnvelopeDocument);
            Assert.True(responseReceipt.IsMultiHopMessage);

            InMessage inUserMessage = _databaseSpy.GetInMessageFor(m => m.EbmsMessageId == userMessage.MessageId);

            Assert.NotNull(inUserMessage);
            Assert.False(inUserMessage.Intermediary);
            Assert.Equal(Operation.ToBeDelivered, inUserMessage.Operation);

            OutMessage outReceipt = _databaseSpy.GetOutMessageFor(m => m.EbmsRefToMessageId == userMessage.MessageId);
            Assert.Equal(OutStatus.Sent, outReceipt.Status.ToEnum<OutStatus>());
        }

        private static void AssertMessageMultihopAttributes(XmlDocument doc)
        {
            var messagingNode = doc.SelectSingleNode("//*[local-name()='Messaging']") as XmlElement;

            Assert.NotNull(messagingNode);
            Assert.Equal(Constants.Namespaces.EbmsNextMsh, messagingNode.GetAttribute("role", Constants.Namespaces.Soap12));
            Assert.True(XmlConvert.ToBoolean(messagingNode.GetAttribute("mustUnderstand", Constants.Namespaces.Soap12)));
        }

        [Fact]
        public async Task ThenReceivedNonMultihopSignalMessageWithoutRelatedUserMessageIsSetToException()
        {
            const string messageId = "message-id";

            AS4Message as4Message = CreateAS4ErrorMessage(messageId);

            // Act
            var response = await StubSender.SendAS4Message(_receiveAgentUrl, as4Message);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            // Assert
            var inMessage = _databaseSpy.GetInMessageFor(m => m.EbmsRefToMessageId == messageId);

            Assert.NotNull(inMessage);
            Assert.Equal(InStatus.Exception, inMessage.Status.ToEnum<InStatus>());

            var inException = _databaseSpy.GetInExceptions(e => e.EbmsRefToMessageId == inMessage.EbmsMessageId);
            Assert.NotNull(inException);
        }

        [Fact]
        public async Task ThenMultiHopSignalMessageIsToBeForwarded()
        {
            // Arrange
            SignalMessage signal = CreateMultihopSignalMessage(
                refToMessageId: "someusermessageid",
                pmodeId: "Forward_Push");

            // Act
            await StubSender.SendAS4Message(_receiveAgentUrl, AS4Message.Create(signal));

            // Assert
            InMessage inMessage = _databaseSpy.GetInMessageFor(m => m.EbmsMessageId == signal.MessageId);

            Assert.NotNull(inMessage);
            Assert.True(inMessage.Intermediary);
            Assert.Equal(Operation.ToBeForwarded, inMessage.Operation);

            Stream messageBody = await Registry.Instance
                .MessageBodyStore
                .LoadMessageBodyAsync(inMessage.MessageLocation);

            AS4Message savedMessage = await SerializerProvider.Default
                .Get(inMessage.ContentType)
                .DeserializeAsync(messageBody, inMessage.ContentType);

            Assert.NotNull(savedMessage.EnvelopeDocument.SelectSingleNode("//*[local-name()='RoutingInput']"));
        }

        [Fact]
        public async Task ThenMultiHopSignalMessageThatHasReachedItsDestinationIsNotified()
        {
            // Arrange
            const string messageId = "some-user-message-id";
            var sendingPMode = new SendingProcessingMode
            {
                ReceiptHandling = new SendReceiptHandling
                {
                    NotifyMessageProducer = true,
                    NotifyMethod = new Method
                    {
                        Type = "FILE",
                        Parameters = new List<Parameter>
                        {
                            new Parameter
                            {
                                Name = "Location",
                                Value = @".\messages\receipts"
                            }
                        }
                    }
                }
            };

            StoreToBeAckOutMessage(messageId, sendingPMode);

            SignalMessage signal = CreateMultihopSignalMessage(
                refToMessageId: messageId,
                pmodeId: "ComponentTest_ReceiveAgent_Sample1");

            // Act
            await StubSender.SendAS4Message(_receiveAgentUrl, AS4Message.Create(signal));

            // Assert
            var inMessage = _databaseSpy.GetInMessageFor(m => m.EbmsRefToMessageId == messageId);
            Assert.NotNull(inMessage);
            Assert.Equal(Operation.ToBeNotified, inMessage.Operation);

            var outMessage = _databaseSpy.GetOutMessageFor(m => m.EbmsMessageId == messageId);
            Assert.NotNull(outMessage);
            Assert.Equal(OutStatus.Ack, outMessage.Status.ToEnum<OutStatus>());
        }

        [Fact]
        public async Task CanReceiveErrorSignalWithoutRefToMessageId()
        {
            var as4Message = AS4Message.Create(
                Error.FromErrorResult(
                    messageId: $"error-{Guid.NewGuid()}",
                    refToMessageId: null,
                    result: new ErrorResult("An Error occurred", ErrorAlias.NonApplicable)));

            string id = as4Message.GetPrimaryMessageId();

            var response = await StubSender.SendAS4Message(_receiveAgentUrl, as4Message);

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.True(String.IsNullOrWhiteSpace(await response.Content.ReadAsStringAsync()));

            var inMessage = _databaseSpy.GetInMessageFor(m => m.EbmsMessageId == id);
            Assert.NotNull(inMessage);
            Assert.Equal(Operation.NotApplicable, inMessage.Operation);
        }

        private static SignalMessage CreateMultihopSignalMessage(string refToMessageId, string pmodeId)
        {
            var userMessage = new UserMessage(
                refToMessageId,
                "some-mpc",
                new CollaborationInfo(
                    new AgreementReference("http://agreements.europa.org/agreement", pmodeId),
                    new Service("Forward_Push_Service", "eu:europe:services"),
                    "Forward_Push_Action",
                    CollaborationInfo.DefaultConversationId),
                new Party("Sender", new PartyId("org:eu:europa:as4:example:accesspoint:A")),
                new Party("Receiver", new PartyId("org:eu:europa:as4:example:accesspoint:B")),
                new PartInfo[0],
                new MessageProperty[0]);

            return Receipt.CreateFor(
                $"receipt-{Guid.NewGuid()}",
                userMessage,
                userMessageSendViaMultiHop: true);
        }

        private void StoreToBeAckOutMessage(string messageId, SendingProcessingMode sendingPMode)
        {
            var outMessage = new OutMessage(messageId);

            outMessage.SetStatus(OutStatus.Sent);
            outMessage.SetPModeInformation(sendingPMode);

            _databaseSpy.InsertOutMessage(outMessage);
        }

        #endregion

        #region Scenario's for receiving bundled messages

        [Fact]
        public async Task Received_Bundled_User_And_Receipt_Message_Should_Process_All_Messages()
        {
            // Arrange
            string ebmsMessageId = "test-" + Guid.NewGuid();
            StoreToBeAckOutMessage(ebmsMessageId, CreateSendingPMode());

            var userMessage = new UserMessage(
                "usermessage-" + Guid.NewGuid(),
                new CollaborationInfo(
                    agreement: new AgreementReference(
                        value: "http://agreements.europa.org/agreement",
                        pmodeId: "receive_bundled_message_pmode"),
                    service: new Service(
                        value: "bundling",
                        type: "as4.net:receive_agent:componenttest"),
                    action: "as4.net:receive_agent:bundling",
                    conversationId: "as4.net:receive_agent:conversation"));

            var receipt = new Receipt($"receipt-{Guid.NewGuid()}", ebmsMessageId);

            var bundled = AS4Message.Create(userMessage);
            bundled.AddMessageUnit(receipt);

            // Act
            HttpResponseMessage response = await StubSender.SendAS4Message(_receiveAgentUrl, bundled);

            // Assert
            AS4Message receivedReceipt = await response.DeserializeToAS4Message();

            Assert.IsType<Receipt>(receivedReceipt.FirstSignalMessage);
            Assert.Equal(userMessage.MessageId, receivedReceipt.FirstSignalMessage.RefToMessageId);

            AssertIfInMessageExistsForSignalMessage(ebmsMessageId);
            AssertIfInMessageIsStoredFor(userMessage.MessageId, Operation.ToBeDelivered);
        }

        private void AssertIfInMessageIsStoredFor(string id, Operation o = Operation.NotApplicable)
        {
            var inMessage = _databaseSpy.GetInMessageFor(m => m.EbmsMessageId == id);
            Assert.NotNull(inMessage);
            Assert.Equal(o.ToString(), inMessage.Operation.ToString());
        }

        [Fact]
        public async Task Received_Bundled_UserMessages_Should_Responds_With_Bundled_Receipts()
        {
            // Arrange
            var bundled = AS4Message.Create(
                new UserMessage(
                    $"user1-{Guid.NewGuid()}",
                    new CollaborationInfo(
                        new AgreementReference(
                            value: "http://agreements.europe.org/agreement",
                            pmodeId: "receive_bundled_message_pmode"))));

            bundled.AddMessageUnits(
                Enumerable.Range(2, 3)
                          .Select(i => new UserMessage($"user{i}-{Guid.NewGuid()}")));

            bundled.Sign(new CalculateSignatureConfig(
                Registry.Instance.CertificateRepository.GetCertificate(
                    X509FindType.FindBySubjectName,
                    "AccessPointA")));

            // Act
            HttpResponseMessage response = await StubSender.SendAS4Message(_receiveAgentUrl, bundled);

            // Assert
            AS4Message receipts = await response.DeserializeToAS4Message();

            int receiptCount = receipts.MessageUnits.Count();
            int userMessageCount = bundled.MessageUnits.Count();
            Assert.True(
                receiptCount == userMessageCount,
                $"{userMessageCount} UserMessage should result in {receiptCount} Receipts");

            Assert.True(
                receipts.SignalMessages.Select(s => s.RefToMessageId).SequenceEqual(bundled.MessageIds),
                "All Receipts must reference the right UserMessages");

            Assert.All(
                receipts.MessageUnits,
                r => Assert.True(
                    Assert.IsType<Receipt>(r).NonRepudiationInformation != null,
                    $"Receipt for UserMessage {r.RefToMessageId} is not a Non-Repudiation Receipt"));

            foreach (string ebmsMessageId in bundled.MessageIds)
            {
                OutMessage entry = await PollUntilPresent(
                    () => _databaseSpy.GetOutMessageFor(m => m.EbmsRefToMessageId == ebmsMessageId),
                    TimeSpan.FromSeconds(20));

                Assert.Equal(MessageType.Receipt, entry.EbmsMessageType);
            }
        }

        #endregion

        private static SendingProcessingMode CreateSendingPMode()
        {
            return new SendingProcessingMode
            {
                Id = "receive_agent_facts_pmode",
                ReceiptHandling = { NotifyMessageProducer = true },
                ErrorHandling = { NotifyMessageProducer = true }
            };
        }

        private static AS4Message CreateAS4ErrorMessage(string refToMessageId)
        {
            Error error = Error.FromErrorResult(
                $"error-{Guid.NewGuid()}",
                refToMessageId,
                new ErrorResult("An error occurred", ErrorAlias.NonApplicable));

            return AS4Message.Create(error, CreateSendingPMode());
        }

        private void AssertIfInMessageExistsForSignalMessage(string expectedId)
        {
            InMessage inMessage = _databaseSpy.GetInMessageFor(m => m.EbmsRefToMessageId == expectedId);
            Assert.NotNull(inMessage);
            Assert.Equal(InStatus.Received, inMessage.Status.ToEnum<InStatus>());
            Assert.Equal(Operation.ToBeNotified, inMessage.Operation);
        }

        // ReSharper disable once UnusedParameter.Local
        private void AssertIfStatusOfOutMessageIs(string expectedId, OutStatus expectedStatus)
        {
            OutMessage outMessage = _databaseSpy.GetOutMessageFor(m => m.EbmsMessageId == expectedId);

            Assert.NotNull(outMessage);
            Assert.Equal(expectedStatus, outMessage.Status.ToEnum<OutStatus>());
        }

        // TODO:
        // - Create a test that verifies if the Status for a received receipt/error is set to
        // --> ToBeNotified when the receipt is valid

        // - Create a test that verifies if the Status for a received UserMessage is set to
        // - Exception when the UserMessage is not valid (an InException should be present).
        protected override void Disposing(bool isDisposing)
        {
            _as4Msh.Dispose();
        }
    }
}