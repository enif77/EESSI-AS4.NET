﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Eu.EDelivery.AS4.Model.Common;
using Eu.EDelivery.AS4.Model.Core;
using Eu.EDelivery.AS4.Model.PMode;
using Eu.EDelivery.AS4.Model.Submit;
using Eu.EDelivery.AS4.Serialization;
using Eu.EDelivery.AS4.UnitTests.Extensions;
using FsCheck;
using FsCheck.Xunit;
using MimeKit;
using Xunit;
using AgreementReference = Eu.EDelivery.AS4.Model.Core.AgreementReference;

namespace Eu.EDelivery.AS4.UnitTests.Model
{
    /// <summary>
    /// Testing <seealso cref="AS4Message" />
    /// </summary>
    public class GivenAS4MessageFacts
    {

        public class Empty
        {
            [Fact]
            public void EmptyInstance_IsNotTheSameWithDifferentId()
            {
                // Arrange
                AS4Message expected =
                    AS4Message.Create(new FilledUserMessage(), new SendingProcessingMode());

                // Act
                AS4Message actual = AS4Message.Empty;

                // Assert
                Assert.NotEqual(expected, actual);
            }

            [Fact]
            public void EmptyIsntanceReturnsExpected()
            {
                Assert.Equal(AS4Message.Empty, AS4Message.Empty);
            }
        }

        public class AddAttachments
        {
            [Property]
            public void ThenMessageRemainsSoapAfterAttachmentsAreRemoved(NonEmptyArray<Guid> ids)
            {
                // Arrange
                AS4Message sut = AS4Message.Empty;
                IEnumerable<Attachment> attachments = ids.Get.Distinct().Select(i => new Attachment(i.ToString()));

                // Act / Assert
                Assert.All(attachments, a =>
                {
                    sut.AddAttachment(a);
                    Assert.NotEqual(Constants.ContentTypes.Soap, sut.ContentType);
                });

                Assert.All(attachments, a => sut.RemoveAttachment(a));                
                Assert.Equal(Constants.ContentTypes.Soap, sut.ContentType);
            }
        }

        public class IsPulling
        {
            [Fact]
            public void IsTrueWhenSignalMessageIsPullRequest()
            {
                // Arrange
                AS4Message as4Message = AS4Message.Create(new PullRequest($"pr-{Guid.NewGuid()}", null));

                // Act
                bool isPulling = as4Message.IsPullRequest;

                // Assert
                Assert.True(isPulling);
            }
        }

        public class AS4MessageDeserializeFacts
        {
            [Fact]
            public async Task Then_MessageUnits_Appear_In_The_Same_Order_As_Serialized()
            {
                var serializer = new SoapEnvelopeSerializer();
                using (var str = new MemoryStream(
                    Encoding.UTF8.GetBytes(
                        Properties.Resources.as4_soap_user_receipt_message)))
                {
                    AS4Message actual = await serializer
                        .DeserializeAsync(str, Constants.ContentTypes.Soap);

                    Assert.IsType<Receipt>(actual.MessageUnits.First());
                    Assert.IsType<UserMessage>(actual.MessageUnits.ElementAt(1));
                    Assert.IsType<Receipt>(actual.MessageUnits.Last());
                    Assert.Equal(
                        Enumerable.Range(1, 3),
                        actual.MessageUnits.Select(m => int.Parse(m.MessageId)));
                }
            }
        }

        public class AS4MessageSerializeFacts : GivenAS4MessageFacts
        {
            [Theory]
            [InlineData("mpc")]
            public void ThenSerializeWithoutAttachmentsReturnsSoapMessage(string mpc)
            {
                // Act
                UserMessage userMessage = CreateUserMessage();
                AS4Message message = BuildAS4Message(mpc, userMessage);

                using (var soapStream = new MemoryStream())
                {
                    XmlDocument document = SerializeSoapMessage(message, soapStream);
                    XmlNode envelopeElement = document.DocumentElement;

                    // Assert
                    Assert.NotNull(envelopeElement);
                    Assert.Equal(Constants.Namespaces.Soap12, envelopeElement.NamespaceURI);
                }
            }

            [Theory]
            [InlineData("mpc")]
            public void ThenPullRequestCorrectlySerialized(string mpc)
            {
                // Arrange
                UserMessage userMessage = CreateUserMessage();

                AS4Message message = BuildAS4Message(mpc, userMessage);

                // Act
                using (var soapStream = new MemoryStream())
                {
                    XmlDocument document = SerializeSoapMessage(message, soapStream);

                    // Assert
                    XmlAttribute mpcAttribute = GetMpcAttribute(document);
                    Assert.NotNull(mpcAttribute);
                    Assert.Equal(mpc, mpcAttribute.Value);
                }
            }

            private static XmlAttribute GetMpcAttribute(XmlDocument document)
            {
                const string node = "/s12:Envelope/s12:Header/eb:Messaging/eb:SignalMessage/eb:PullRequest";
                XmlAttributeCollection attributes = document.SelectEbmsNode(node).Attributes;

                return attributes?.Cast<XmlAttribute>().FirstOrDefault(x => x.Name == "mpc");
            }

            [Theory]
            [InlineData("mpc")]
            public void ThenSerializeWithAttachmentsReturnsMimeMessage(string messageContents)
            {
                // Arrange
                var attachmentStream = new MemoryStream(Encoding.UTF8.GetBytes(messageContents));
                var attachment = new Attachment("attachment-id", attachmentStream, "text/plain");

                UserMessage userMessage = CreateUserMessage();

                AS4Message message = AS4Message.Create(userMessage);
                message.AddAttachment(attachment);

                // Act
                AssertMimeMessageIsValid(message);
            }

            private void AssertMimeMessageIsValid(AS4Message message)
            {
                using (var mimeStream = new MemoryStream())
                {
                    MimeMessage mimeMessage = SerializeMimeMessage(message, mimeStream);
                    Stream envelopeStream = mimeMessage.BodyParts.OfType<MimePart>().First().Content.Open();
                    string rawXml = new StreamReader(envelopeStream).ReadToEnd();

                    // Assert
                    Assert.NotNull(rawXml);
                    Assert.Contains("Envelope", rawXml);
                }
            }

            [Fact]
            public void ThenSaveToUserMessageCorrectlySerialized()
            {
                // Arrange
                UserMessage userMessage = CreateUserMessage();
                AS4Message message = AS4Message.Create(userMessage);

                // Act
                using (var soapStream = new MemoryStream())
                {
                    XmlDocument document = SerializeSoapMessage(message, soapStream);

                    // Assert
                    Assert.NotNull(document.DocumentElement);
                    Assert.Contains("Envelope", document.DocumentElement.Name);
                }
            }
        }

        protected UserMessage CreateUserMessage()
        {
            return new UserMessage("message-id");
        }

        protected XmlDocument SerializeSoapMessage(AS4Message message, MemoryStream soapStream)
        {
            ISerializer serializer = new SoapEnvelopeSerializer();
            serializer.Serialize(message, soapStream);

            soapStream.Position = 0;
            var document = new XmlDocument();
            document.Load(soapStream);

            return document;
        }

        protected XmlDocument SerializeSoapMessage(AS4Message message)
        {
            using (var soapStream = new MemoryStream())
            {
                ISerializer serializer = new SoapEnvelopeSerializer();
                serializer.Serialize(message, soapStream);

                soapStream.Position = 0;
                var document = new XmlDocument();
                document.Load(soapStream);

                return document; 
            }
        }

        protected MimeMessage SerializeMimeMessage(AS4Message message, MemoryStream mimeStream)
        {
            ISerializer serializer = new MimeMessageSerializer(new SoapEnvelopeSerializer());
            serializer.Serialize(message, mimeStream);

            mimeStream.Position = 0;

            return MimeMessage.Load(mimeStream);
        }

        protected AS4Message BuildAS4Message(string mpc, UserMessage userMessage)
        {
            AS4Message as4Message = AS4Message.Create(userMessage);
            as4Message.AddMessageUnit(new PullRequest($"pr-{Guid.NewGuid()}", mpc));

            return as4Message;
        }
    }
}