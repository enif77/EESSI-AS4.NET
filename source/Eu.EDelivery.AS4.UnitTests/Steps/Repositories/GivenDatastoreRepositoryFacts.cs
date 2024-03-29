﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eu.EDelivery.AS4.Common;
using Eu.EDelivery.AS4.Entities;
using Eu.EDelivery.AS4.Repositories;
using Eu.EDelivery.AS4.UnitTests.Common;
using Eu.EDelivery.AS4.UnitTests.Repositories;
using Xunit;

namespace Eu.EDelivery.AS4.UnitTests.Steps.Repositories
{
    /// <summary>
    /// Testing the <see cref="DatastoreRepository" />
    /// </summary>
    public class GivenDatastoreRepositoryFacts : GivenDatastoreFacts
    {
        public class OutMessages : GivenDatastoreRepositoryFacts
        {
            [Fact]
            public void ThenGetOutMessageSucceeded()
            {
                // Arrange
                const string ebmsMessageId = "message-id";
                const Operation expected = Operation.Delivered;
                InsertOutMessage(ebmsMessageId, expected);

                using (DatastoreContext context = GetDataStoreContext())
                {
                    var repository = new DatastoreRepository(context);

                    // Act
                    Operation actual =
                        repository.GetOutMessageData(@where: m => m.EbmsMessageId == ebmsMessageId,
                                                     selection: m => m.Operation)
                                  .SingleOrDefault();

                    // Assert
                    Assert.Equal(expected, actual);
                }
            }

            [Fact]
            public async Task ThenInsertOutMessageSucceedsAsync()
            {
                // Arrange
                var outMessage = new OutMessage(ebmsMessageId: "message-id") { MessageLocation = "location" };

                // Act
                using (DatastoreContext context = GetDataStoreContext())
                {
                    new DatastoreRepository(context).InsertOutMessage(outMessage);

                    await context.SaveChangesAsync();
                }

                // Assert
                AssertOutMessage(outMessage.EbmsMessageId, Assert.NotNull);
            }

            [Fact]
            public void ThenUpdateOutMessageSucceedsAsync()
            {
                // Arrange
                const string sharedId = "message-id";
                long outMessageId = InsertOutMessage(sharedId, Operation.ToBeSent).Id;

                // Act
                using (DatastoreContext context = GetDataStoreContext())
                {
                    new DatastoreRepository(context).UpdateOutMessage(
                       outMessageId,
                       m => m.Operation = Operation.Sent);

                    context.SaveChanges();
                }

                // Assert
                AssertOutMessage(sharedId, m => Assert.Equal(Operation.Sent, m.Operation));
            }

            private OutMessage InsertOutMessage(string ebmsMessageId, Operation operation = Operation.NotApplicable)
            {
                var outMessage = new OutMessage(ebmsMessageId: ebmsMessageId);
                outMessage.Operation = operation;

                GetDataStoreContext.InsertOutMessage(outMessage, withReceptionAwareness: false);

                return outMessage;
            }

            private void AssertOutMessage(string messageId, Action<OutMessage> assertAction)
            {
                using (var contex = GetDataStoreContext())
                {
                    OutMessage outMessage = contex.OutMessages.FirstOrDefault(m => m.EbmsMessageId.Equals(messageId));
                    assertAction(outMessage);
                }
            }
        }

        public class InExceptions : GivenDatastoreRepositoryFacts
        {
            [Fact]
            public async Task ThenInsertInExceptionSucceeds()
            {
                // Arrange
                var inException = InException.ForEbmsMessageId($"inex-{Guid.NewGuid()}", "error");

                // Act
                using (DatastoreContext context = GetDataStoreContext())
                {
                    new DatastoreRepository(context).InsertInException(inException);

                    await context.SaveChangesAsync();
                }

                GetDataStoreContext.AssertInException(inException.EbmsRefToMessageId, Assert.NotNull);
            }
        }

        public class OutExceptions : GivenDatastoreRepositoryFacts
        {
            [Fact]
            public async Task ThenInsertOutExceptionSucceeds()
            {
                // Arrange
                var outException = OutException.ForEbmsMessageId($"outex-{Guid.NewGuid()}", "error");

                // Act
                using (DatastoreContext context = GetDataStoreContext())
                {
                    new DatastoreRepository(context).InsertOutException(outException);

                    await context.SaveChangesAsync();
                }

                // Assert
                GetDataStoreContext.AssertOutException(outException.EbmsRefToMessageId, Assert.NotNull);
            }
        }

        public class InMessages : GivenDatastoreRepositoryFacts
        {
            [Fact]
            public void SelectsOnlyMessageLocation()
            {
                // Arrange
                const string messageId = "single-id";
                const string expected = "message-location";
                InsertInMessage(messageId, m => m.MessageLocation = expected);

                // Act
                string actual = ExerciseRepository(sut => sut.GetInMessageData(messageId, m => m.MessageLocation));

                // Assert
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void GetsMessageIdsForFoundUserMessages()
            {
                TestFoundInMessagesFor(id => InsertInMessageWithOperation(id), repository => repository.SelectExistingInMessageIds);
            }

            [Fact]
            public void GetsMmessagesIdsForFoundSignalMessages()
            {
                TestFoundInMessagesFor(InsertRefInMessage, repository => repository.SelectExistingRefInMessageIds);
            }

            private void TestFoundInMessagesFor(Action<string> insertion, Func<DatastoreRepository, Func<IEnumerable<string>, IEnumerable<string>>> sutAction)
            {
                // Arrange
                const string expectedId = "message-id";
                insertion(expectedId);

                using (DatastoreContext context = GetDataStoreContext())
                {
                    var repository = new DatastoreRepository(context);
                    var expectedMessageIds = new[] { expectedId };

                    // Act
                    IEnumerable<string> actualMessageIds = sutAction(repository)(expectedMessageIds);

                    // Assert
                    Assert.Equal(expectedMessageIds, actualMessageIds);
                }
            }

            [Theory]
            [InlineData("shared-id")]
            public void ThenInMessageExistsSucceeded(string sharedId)
            {
                // Arrange
                InsertInMessageWithOperation(sharedId);

                using (DatastoreContext context = GetDataStoreContext())
                {
                    var repository = new DatastoreRepository(context);

                    // Act
                    bool result = repository.InMessageExists(m => m.EbmsMessageId == sharedId);

                    // Assert
                    Assert.True(result);
                }
            }

            [Theory]
            [InlineData("shared-id")]
            public void ThenInsertInMessageSucceedsAsync(string sharedId)
            {
                // Arrange
                InsertInMessageWithOperation(sharedId);

                // Assert
                GetDataStoreContext.AssertInMessage(sharedId, Assert.NotNull);
            }

            [Theory]
            [InlineData("share-id")]
            public async Task ThenUpdateInMessageSucceedsAsync(string sharedId)
            {
                // Arrange
                InsertInMessageWithOperation(sharedId, Operation.ToBeDelivered);

                // Act
                using (DatastoreContext context = GetDataStoreContext())
                {
                    new DatastoreRepository(context).UpdateInMessage(
                        sharedId,
                        m => m.Operation = Operation.Delivered);

                    await context.SaveChangesAsync();
                }

                // Assert
                GetDataStoreContext.AssertInMessage(sharedId, m => Assert.Equal(Operation.Delivered, m.Operation));
            }

            private void InsertInMessageWithOperation(string ebmsMessageId, Operation operation = Operation.NotApplicable)
            {
                InsertInMessage(ebmsMessageId, m => m.Operation = operation);
            }

            private void InsertInMessage(string messageId, Action<InMessage> arrangeMessage)
            {
                var message = new InMessage(ebmsMessageId: messageId);
                arrangeMessage(message);

                GetDataStoreContext.InsertInMessage(message);
            }

            private void InsertRefInMessage(string refToEbmsMessageId)
            {
                GetDataStoreContext.InsertInMessage(new InMessage(Guid.NewGuid().ToString()) { EbmsRefToMessageId = refToEbmsMessageId });
            }

            private TResult ExerciseRepository<TResult>(Func<DatastoreRepository, TResult> act)
            {
                using (DatastoreContext context = GetDataStoreContext())
                {
                    var sut = new DatastoreRepository(context);

                    // Act
                    return act(sut);
                }
            }
        }
    }
}