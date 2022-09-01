﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Eu.EDelivery.AS4.Model.Core;
using Eu.EDelivery.AS4.Model.Internal;
using Eu.EDelivery.AS4.Model.PMode;
using Eu.EDelivery.AS4.Model.Submit;
using Eu.EDelivery.AS4.Services.DynamicDiscovery;
using Eu.EDelivery.AS4.Steps;
using Eu.EDelivery.AS4.Steps.Submit;
using Eu.EDelivery.AS4.UnitTests.Extensions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using AS4Party = Eu.EDelivery.AS4.Model.Core.Party;
using AS4PartyId = Eu.EDelivery.AS4.Model.Core.PartyId;
using SubmitParty = Eu.EDelivery.AS4.Model.Common.Party;
using SubmitPartyId = Eu.EDelivery.AS4.Model.Common.PartyId;
using PModeParty = Eu.EDelivery.AS4.Model.PMode.Party;
using PModePartyId = Eu.EDelivery.AS4.Model.PMode.PartyId;

namespace Eu.EDelivery.AS4.UnitTests.Steps.Submit
{
    public class GivenDynamicDiscoveryStepFacts
    {
        [CustomProperty]
        public Property Resolve_Only_ToParty_From_AS4Message_If_UserMessage(
            MessagingContextMode contextMode,
            Maybe<SignalMessage> signalM)
        {
            return Prop.ForAll(
                GenAS4Party().ToArbitrary(),
                receiver =>
                {
                    var user = new UserMessage(
                        $"user-{Guid.NewGuid()}", 
                        AS4Party.DefaultFrom, 
                        receiver);

                    AS4Message message =
                        signalM.Select(s => AS4Message.Create(new MessageUnit[] { s, user }))
                               .GetOrElse(AS4Message.Create(user));

                    var context = new MessagingContext(message, contextMode)
                    {
                        SendingPMode = new SendingProcessingMode
                        {
                            DynamicDiscovery = new DynamicDiscoveryConfiguration()
                        }
                    };

                    var act = new Lazy<bool>(() =>
                    {
                        var spy = new SpyToPartyDynamicDiscoveryProfile();
                        var sut = new DynamicDiscoveryStep(_ => spy);
                        sut.ExecuteAsync(context)
                           .GetAwaiter()
                           .GetResult();

                        return spy.ToParty
                                  .Equals(receiver)
                                  .Equals(contextMode == MessagingContextMode.Forward
                                          && context.AS4Message.IsUserMessage);
                    });

                    return Prop.Throws<InvalidOperationException, bool>(act)
                               .Label($"Throws {nameof(InvalidOperationException)}")
                               .Or(() => act.Value)
                               .Label("Resolved ToParty is UserMessge.ToParty");
                });
        }

        private static Gen<AS4Party> GenAS4Party()
        {
            return Arb.Generate<NonEmptyString>()
                      .Two()
                      .Select(t => new AS4PartyId(t.Item1.Get, t.Item2.Get))
                      .NonEmptyListOf()
                      .Zip(Arb.Generate<NonEmptyString>(), (ids, role) => new AS4Party(role.Get, ids));
        }

        [Property(MaxTest = 1000)]
        public Property Resolve_Either_Submit_Or_SendingPMode_ToParty(bool allowOverride)
        {
            return Prop.ForAll(
                GenSubmitParty().ToArbitrary(),
                GenPModeParty().ToArbitrary(),
                (submitParty, pmodeParty) =>
                {
                    (bool dynamicallyDiscovered, AS4Party resolved) = 
                        ExerciseDynamicDiscovery(submitParty, pmodeParty, allowOverride);

                    bool submitSameAsPModeParty =
                        submitParty?.Equals(pmodeParty) ?? false;

                    bool resolvedSubmit =
                        submitParty?.Role != null
                        && submitParty.PartyIds.EmptyIfNull().All(p => p?.Id != null)
                        && resolved != null
                        && resolved.Role.Equals(submitParty.Role)
                        && resolved.PartyIds
                                   .Zip(submitParty?.PartyIds.EmptyIfNull(), Tuple.Create)
                                   .All(t => t.Item1.Id.Equals(t.Item2.Id)
                                             && t.Item1.Type.Equals(t.Item2.Type.AsMaybe()));

                    bool resolvedPMode =
                        submitParty == null 
                        && pmodeParty?.Role != null 
                        && pmodeParty.PartyIds.EmptyIfNull().All(p => p?.Id != null)
                        && resolved != null
                        && resolved.Role.Equals(pmodeParty.Role)
                        && resolved.PartyIds
                                   .Zip(pmodeParty?.PartyIds.EmptyIfNull(), Tuple.Create)
                                   .All(t => t.Item1.Id.Equals(t.Item2.Id) 
                                             && t.Item1.Type.Equals(t.Item2.Type.AsMaybe()));

                    return (dynamicallyDiscovered && resolvedSubmit && allowOverride)
                           .Or(dynamicallyDiscovered && resolvedPMode)
                           .Or(dynamicallyDiscovered && submitSameAsPModeParty && resolvedSubmit)
                           .Or(!dynamicallyDiscovered)
                           .Label(
                               $"PMode {(dynamicallyDiscovered ? "is" : "isn't")} dynamically discoverd"
                               + $" but the resolved ToParty {(resolvedSubmit ? "is" : "isn't")} from SubmitMessage "
                               + $" and {(resolvedPMode ? "also" : "not")} from SendingPMode");
                });
        }

        private static Gen<SubmitParty> GenSubmitParty()
        {
            return Arb.Generate<string>()
                      .Two()
                      .Select(t => new SubmitPartyId(t.Item1, t.Item2))
                      .OrNull()
                      .ArrayOf()
                      .OrNull()
                      .Zip(Arb.Generate<string>(),
                           (ids, role) => new SubmitParty { Role = role, PartyIds = ids });
        }

        private static Gen<PModeParty> GenPModeParty()
        {
            return Arb.Generate<string>()
                      .Two()
                      .Select(t => new PModePartyId { Id = t.Item1, Type = t.Item2 })
                      .OrNull()
                      .ListOf()
                      .OrNull()
                      .Zip(Arb.Generate<string>(),
                           (ids, role) => new PModeParty { Role = role, PartyIds = ids?.ToList() });
        }

        private static (bool, AS4Party) ExerciseDynamicDiscovery(
            SubmitParty submitParty, 
            PModeParty pmodeParty,
            bool allowOverride)
        {
            var context = new MessagingContext(
                new SubmitMessage
                {
                    PartyInfo = { ToParty = submitParty }
                })
                {
                    SendingPMode = new SendingProcessingMode
                    {
                        AllowOverride = allowOverride,
                        DynamicDiscovery = new DynamicDiscoveryConfiguration(),
                        MessagePackaging = { PartyInfo = new PartyInfo { ToParty = pmodeParty } }
                    }
                };

            try
            {
                var spy = new SpyToPartyDynamicDiscoveryProfile();
                var sut = new DynamicDiscoveryStep(_ => spy);
                sut.ExecuteAsync(context)
                       .GetAwaiter()
                       .GetResult();

                return (true, spy.ToParty);
            }
            catch
            {
                return (false, null);
            }
        }

        public class SpyToPartyDynamicDiscoveryProfile : IDynamicDiscoveryProfile
        {
            public AS4Party ToParty { get; private set; }

            public Task<XmlDocument> RetrieveSmpMetaDataAsync(AS4Party party, IDictionary<string, string> properties)
            {
                ToParty = party;
                return Task.FromResult(new XmlDocument());
            }

            public DynamicDiscoveryResult DecoratePModeWithSmpMetaData(
                SendingProcessingMode pmode,
                XmlDocument smpMetaData)
            {
                return DynamicDiscoveryResult.Create(pmode);
            }
        }

        [Fact]
        public async Task ThenExecuteStepResultInContactingSMPServer()
        {
            // Arrange
            SendingProcessingMode pmode = EnabledDynamicDiscoveryPMode(
                smpProfile: typeof(ChangeIdDiscoveryProfile).AssemblyQualifiedName);

            string beforeId = pmode.Id;

            // Act
            StepResult result = await ExerciseDynamicDiscovery(pmode);

            // Assert
            string afterId = result.MessagingContext.SendingPMode.Id;
            Assert.NotEqual(beforeId, afterId);
        }

        [Fact]
        public async Task ThenExecuteStepFailsWithMissingToPartyId()
        {
            // Arrange
            SendingProcessingMode pmode = EnabledDynamicDiscoveryPMode(
                smpProfile: typeof(ChangeIdDiscoveryProfile).AssemblyQualifiedName);

            pmode.MessagePackaging.PartyInfo.ToParty.PartyIds.Clear();

            // Act
            await Assert.ThrowsAsync<ConfigurationErrorsException>(
                () => ExerciseDynamicDiscovery(pmode));
        }

        private static SendingProcessingMode EnabledDynamicDiscoveryPMode(string smpProfile)
        {
            return new SendingProcessingMode
            {
                DynamicDiscovery = new DynamicDiscoveryConfiguration
                {
                    SmpProfile = smpProfile
                },
                MessagePackaging = new SendMessagePackaging
                {
                    PartyInfo = new PartyInfo
                    {
                        ToParty = new PModeParty
                        {
                            Role = Guid.NewGuid().ToString(),
                            PartyIds = { new PModePartyId { Id = Guid.NewGuid().ToString()} }
                        }
                    }
                }
            };
        }

        private static async Task<StepResult> ExerciseDynamicDiscovery(SendingProcessingMode pmode)
        {
            var step = new DynamicDiscoveryStep();

            return await step.ExecuteAsync(
                new MessagingContext(new SubmitMessage()) {SendingPMode = pmode});
        }

        public class ChangeIdDiscoveryProfile : IDynamicDiscoveryProfile
        {
            /// <summary>
            /// Retrieves the SMP meta data <see cref="XmlDocument"/> for a given <paramref name="party"/> using a given <paramref name="properties"/>.
            /// </summary>
            /// <param name="party">The party identifier.</param>
            /// <param name="properties"></param>
            /// <returns></returns>
            public Task<XmlDocument> RetrieveSmpMetaDataAsync(AS4.Model.Core.Party party, IDictionary<string, string> properties)
            {
                return Task.FromResult(new XmlDocument());
            }

            /// <summary>
            /// Complete the <paramref name="pmode"/> with the SMP metadata that is present in the <paramref name="smpMetaData"/> <see cref="XmlDocument"/>
            /// </summary>
            /// <param name="pmode">The <see cref="SendingProcessingMode"/> that must be decorated with the SMP metadata</param>
            /// <param name="smpMetaData">An XmlDocument that contains the SMP MetaData that has been received from an SMP server.</param>
            /// <returns>The completed <see cref="SendingProcessingMode"/></returns>
            public DynamicDiscoveryResult DecoratePModeWithSmpMetaData(SendingProcessingMode pmode, XmlDocument smpMetaData)
            {
                pmode.Id = Guid.NewGuid().ToString();
                return DynamicDiscoveryResult.Create(pmode);
            }
        }
    }
}
