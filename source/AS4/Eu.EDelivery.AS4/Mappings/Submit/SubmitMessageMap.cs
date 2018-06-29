﻿using System.Linq;
using AutoMapper;
using Eu.EDelivery.AS4.Factories;

namespace Eu.EDelivery.AS4.Mappings.Submit
{
    public class SubmitMessageMap : Profile
    {
        public SubmitMessageMap()
        {
            CreateMap<Model.Submit.SubmitMessage, Model.Core.UserMessage>()
                .ForMember(dest => dest.MessageId, src => src.Ignore())
                .ForMember(dest => dest.RefToMessageId, src => src.MapFrom(s => s.MessageInfo.RefToMessageId))
                .ForMember(dest => dest.Timestamp, src => src.Ignore())
                .ForMember(dest => dest.IsTest, src => src.Ignore())
                .ForMember(dest => dest.IsDuplicate, src => src.Ignore())
                .ForMember(dest => dest.Mpc, src => src.Ignore())
                .AfterMap(
                    (submit, user) =>
                    {
                        user.MessageId = submit.MessageInfo?.MessageId ?? IdentifierFactory.Instance.Create();

                        user.Sender = SubmitSenderResolver.ResolveSender(submit);
                        user.Receiver = SubmitReceiverResolver.ResolveReceiver(submit);

                        user.Mpc = SubmitMpcResolver.Default.Resolve(submit);
                        user.CollaborationInfo = new Model.Core.CollaborationInfo(
                            SubmitMessageAgreementResolver.ResolveAgreementReference(submit, user),
                            SubmitServiceResolver.ResolveService(submit),
                            SubmitActionResolver.ResolveAction(submit),
                            SubmitConversationIdResolver.ResolveConverstationId(submit));

                        if (submit.HasPayloads)
                        {
                            foreach (Model.Core.PartInfo p in 
                                SubmitPayloadInfoResolver.Default.Resolve(submit))
                            {
                                user.AddPartInfo(p);
                            }
                        }

                        if (submit.MessageProperties?.Any() == true)
                        {
                            foreach (Model.Core.MessageProperty p in
                                SubmitMessagePropertiesResolver.Default.Resolve(submit))
                            {
                                user.AddMessageProperty(p);
                            }
                        }
                    }).ForAllOtherMembers(x => x.Ignore());
        }
    }
}