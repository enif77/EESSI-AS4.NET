﻿using System;
using System.Linq;
using Eu.EDelivery.AS4.Builders;
using Eu.EDelivery.AS4.Extensions;
using Eu.EDelivery.AS4.Model.PMode;
using Eu.EDelivery.AS4.Services.DynamicDiscovery;
using FluentValidation;
using FluentValidation.Results;
using NLog;

namespace Eu.EDelivery.AS4.Validators
{
    /// <summary>
    /// Validator responsible for Validation Model <see cref="SendingProcessingMode" />
    /// </summary>
    public class SendingProcessingModeValidator : AbstractValidator<SendingProcessingMode>
    {
        // TODO: [Obsolete("AbstractValidator is deprecated in FluentValidation 8. Please inherit from ValidatorBase<T> instead, and build your rules in the Rules method.
        // For more information about upgrading to FluentValidation 8 please see https://fluentvalidation.net/upgrading-to-8")]
        // https://docs.fluentvalidation.net/en/latest/upgrading-to-8.html
        // https://github.com/FluentValidation/FluentValidation/blob/2b13ac2b66017def9e92ad4cf0be72a4b30373d9/src/FluentValidation/AbstractValidator.cs

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="SendingProcessingModeValidator"/> class.
        /// </summary>
        private SendingProcessingModeValidator()
        {
            RuleFor(pmode => pmode.Id)
                .NotEmpty()
                .WithMessage("Id element must not be empty");

            RuleFor(pmode => pmode)
                .Must(pmode => pmode.PushConfigurationSpecified || pmode.DynamicDiscoverySpecified)
                .When(pmode => pmode.MepBinding == MessageExchangePatternBinding.Push)
                .WithMessage("Either a <PushConfiguration/> or <DynamicDiscovery/> element must be specified");

            RulesForPushConfiguration();
            RulesForDynamicDiscovery();
            RulesForReliability();
            RulesForReceiptHandling();
            RulesForErrorHandling();
            RulesForExceptionHandling();
            RulesForSigning();
            RulesForEncryption();
        }

        public static readonly SendingProcessingModeValidator Instance = new SendingProcessingModeValidator();

        private void RulesForPushConfiguration()
        {
            When(p => p.PushConfigurationSpecified, () =>
            {
                const string errorMsg = "PushConfiguration.Protocol.Url element should be specified when SMP Profile is missing";

                RuleFor(pmode => pmode.PushConfiguration.Protocol)
                    .NotNull()
                    .WithMessage(errorMsg);

                RuleFor(pmode => pmode.PushConfiguration.Protocol.Url)
                    .NotNull()
                    .When(pmode => pmode?.PushConfiguration?.Protocol != null)
                    .WithMessage("PushConfiguration.Protocol.Url should be specified with a valid HTTP(S) URL");

                RuleFor(pmode => pmode.PushConfiguration.TlsConfiguration.ClientCertificateInformation)
                    .Must((pmode, info) =>
                    {
                        object clientCertInfo = pmode.PushConfiguration.TlsConfiguration.ClientCertificateInformation;
                        if (clientCertInfo is ClientCertificateReference r)
                        {
                            return !String.IsNullOrWhiteSpace(r.ClientCertificateFindValue);
                        }

                        if (clientCertInfo is PrivateKeyCertificate c)
                        {
                            return !String.IsNullOrWhiteSpace(c.Certificate) 
                                   && !String.IsNullOrWhiteSpace(c.Password);
                        }

                        return false;
                    })
                    .When(pmode => pmode.PushConfiguration?.TlsConfiguration?.IsEnabled ?? false)
                    .WithMessage(
                        "PushConfiguration.TlsConfiguration should have either an <ClientCertificateReference/> " + 
                        "with a non-empty <ClientCertificateFindValue/> or an <PrivateKeyCertificate/> element " +
                        "with a non-empty <Certificate/> and <Password/> elements " +
                        "when the PushConfiguration.TlsConfiguration.IsEnabled = true");
            });
        }

        private void RulesForDynamicDiscovery()
        {
            When(pmode => pmode.DynamicDiscoverySpecified,() =>
            {
                RuleFor(pmode => pmode.DynamicDiscovery.SmpProfile)
                    .Must(GenericTypeBuilder.CanResolveTypeThatImplements<IDynamicDiscoveryProfile>)
                    .When(pmode => pmode.DynamicDiscovery.SmpProfile != null)
                    .WithMessage(
                        "DynamicDiscovery.SmpProfile should be a fully-qualified assembly name of a " + 
                        $"{nameof(IDynamicDiscoveryProfile)} implementation when there exists a <SmpProfile/> element");
            });
        }

        private void RulesForReliability()
        {
            When(pmode => pmode.Reliability?.ReceptionAwareness?.IsEnabled == true, () =>
            {
                RuleFor(pmode => pmode.Reliability.ReceptionAwareness.RetryCount)
                    .Must(retryCount => retryCount > 0)
                    .WithMessage(
                        "Reliability.ReceptionAwareness.RetryCount should be greater than 0 when Reliability.ReceptionAwareness.IsEnabled = true");

                RuleFor(pmode => pmode.Reliability.ReceptionAwareness.RetryInterval.AsTimeSpan())
                    .Must(retryInterval => retryInterval > default(TimeSpan))
                    .WithMessage(
                        $"Reliability.ReceptionAwareness.RetryInterval should be greater than {default(TimeSpan)} when Reliability.ReceptionAwareness.IsEnabled = true");
            });
        }

        private void RulesForReceiptHandling()
        {
            RulesForReceiptNotifyMethod();
            RulesForReceiptReliability();
        }

        private void RulesForReceiptNotifyMethod()
        {
            When(pmode => pmode.ReceiptHandling?.NotifyMessageProducer == true, () =>
            {
                RuleFor(pmode => pmode.ReceiptHandling.NotifyMethod)
                    .NotNull()
                    .WithMessage("ReceiptHandling.NotifyMethod should be specified when the ReceiptHandling.NotifyMessageProducer = true");

                When(pmode => pmode.ReceiptHandling.NotifyMethod != null, () =>
                {
                    RuleFor(pmode => pmode.ReceiptHandling.NotifyMethod.Type)
                        .NotEmpty()
                        .WithMessage("ReceiptHandling.NotifyMethod.Type should be specified when the ReceiptHandling.NotifyMessageProducer = true");

                    RuleFor(pmode => pmode.ReceiptHandling.NotifyMethod.Parameters)
                        .NotNull()
                        .ForEach((Parameter p) => p != null)
                        .SetCollectionValidator(ParameterValidator.Instance)
                        .WithMessage(
                            "ReceiptHandling.NotifyMethod.Parameters should be specified with an empty tag or " +
                            "with non-empty Name and Value attributes when the ReceiptHandling.NotifyMessageProducer = true");
                });
            });
        }

        private void RulesForReceiptReliability()
        {
            When(pmode => pmode.ReceiptHandling?.Reliability?.IsEnabled == true, () =>
            {
                RuleFor(pmode => pmode.ReceiptHandling.Reliability.RetryCount)
                    .Must(i => i > 0)
                    .WithMessage("ReceiptHandling.Reliability.RetryCount should be greater than 0 when the ReceiptHandling.Reliability.IsEnabled = true");

                RuleFor(pmode => pmode.ReceiptHandling.Reliability.RetryInterval.AsTimeSpan())
                    .Must(t => t > default(TimeSpan))
                    .WithMessage(
                        $"ReceiptHandling.Reliability.RetryInterval should be greater than {default(TimeSpan)} when the ReceiptHandling.Reliability.IsEnabled = true");
            });
        }

        private void RulesForErrorHandling()
        {
            RulesForErrorNotifyMethod();
            RulesForErrorReliability();
        }

        private void RulesForErrorNotifyMethod()
        {
            When(pmode => pmode.ErrorHandling?.NotifyMessageProducer == true, () =>
            {
                RuleFor(pmode => pmode.ErrorHandling.NotifyMethod)
                    .NotNull()
                    .WithMessage("Errorhandling.NotifyMethod should be specified when the ErrorHandling.NotifyMessageProducer = true");

                When(pmode => pmode.ErrorHandling.NotifyMethod != null, () =>
                {
                    RuleFor(pmode => pmode.ErrorHandling.NotifyMethod.Type)
                        .NotEmpty()
                        .WithMessage("ErrorHandling.NotifyMethod.Type should be specified when the ErrorHandling.NotifyMessageProducer = true");

                    RuleFor(pmode => pmode.ErrorHandling.NotifyMethod.Parameters)
                        .NotNull()
                        .ForEach((Parameter p) => p != null)
                        .SetCollectionValidator(ParameterValidator.Instance)
                        .WithMessage(
                            "ErrorHandling.NotifyMethod.Parameters should be specified with an empty tag or " +
                            "with non-empty Name and Value attributes when the ErrorHandling.NotifyMessageProducer = true");
                });
            });
        }

        private void RulesForErrorReliability()
        {
            When(pmode => pmode.ErrorHandling?.Reliability?.IsEnabled == true, () =>
            {
                RuleFor(pmode => pmode.ErrorHandling.Reliability.RetryCount)
                    .Must(i => i > 0)
                    .WithMessage("ErrorHandling.Reliability.RetryCount should be greater than 0 when the ErrorHandling.Reliability.IsEnabled = true");

                RuleFor(pmode => pmode.ErrorHandling.Reliability.RetryInterval.AsTimeSpan())
                    .Must(t => t > default(TimeSpan))
                    .WithMessage(
                        $"ErrorHandling.Reliability.RetryInterval should be greater than {default(TimeSpan)} when the ErrorHandling.Reliability.IsEnabled = true");
            });
        }

        private void RulesForExceptionHandling()
        {
            RulesForExceptionNotifyMethod();
            RulesForExceptionReliability();
        }

        private void RulesForExceptionNotifyMethod()
        {
            When(pmode => pmode.ExceptionHandling.NotifyMessageProducer, () =>
            {
                RuleFor(pmode => pmode.ExceptionHandling.NotifyMethod)
                    .NotNull()
                    .WithMessage("ExceptionHandling.NotifyMethod should be specified when the ExceptionHandling.NotifyMessageProducer = true");

                When(pmode => pmode.ExceptionHandling.NotifyMethod != null, () =>
                {
                    RuleFor(pmode => pmode.ExceptionHandling.NotifyMethod.Type)
                        .NotEmpty()
                        .WithMessage("ExceptionHandling.NotifyMethod.Type should be specified when the ExceptionHandling.NotifyMessageProducer = true");

                    RuleFor(pmode => pmode.ExceptionHandling.NotifyMethod.Parameters)
                        .NotNull()
                        .ForEach((Parameter p) => p != null)
                        .SetCollectionValidator(ParameterValidator.Instance)
                        .WithMessage(
                            "ExceptionHandling.NotifyMethod.Parameters should be specified as an empty tag or " +
                            "with non-empty Name and Value attributes when the ExceptionHandling.NotifyMessageProducer = true");
                });
            });
        }

        private void RulesForExceptionReliability()
        {
            When(pmode => pmode.ExceptionHandling?.Reliability?.IsEnabled == true, () =>
            {
                RuleFor(pmode => pmode.ExceptionHandling.Reliability.RetryCount)
                    .Must(i => i > 0)
                    .WithMessage(
                        "ExceptionHandling.Reliability.RetryCount should be greater than 0 when the ExceptionHandling.Reliability.IsEnabled = true");

                RuleFor(pmode => pmode.ExceptionHandling.Reliability.RetryInterval.AsTimeSpan())
                    .Must(t => t > default(TimeSpan))
                    .WithMessage(
                        $"Excepitonhandling.Reliability.RetryInterval should be greater than {default(TimeSpan)} when the ExceptionHandling.Reliability.IsEnabled = true");
            });
        }

        private void RulesForSigning()
        {
            When(pmode => pmode.Security.Signing.IsEnabled, () =>
            {
                RuleFor(pmode => Constants.SignAlgorithms.IsSupported(pmode.Security.Signing.Algorithm))
                    .NotNull()
                    .NotEmpty()
                    .WithMessage(
                        $"Security.Signing.Algorithm should be one of these: {String.Join(", ", Constants.SignAlgorithms.SupportedAlgorithms)} " + 
                        "when Security.Signing.IsEnabled = true");

                RuleFor(pmode => Constants.HashFunctions.IsSupported(pmode.Security.Signing.HashFunction))
                    .NotNull()
                    .NotEmpty()
                    .WithMessage(
                        $"Security.Signing.HashFunction should be one of these: {String.Join(", ", Constants.HashFunctions.SupportedAlgorithms)} " + 
                        "when Security.Signing.IsEnabled = true");

                RuleFor(pmode => pmode.Security.Signing.SigningCertificateInformation)
                    .Must(cert =>
                    {
                        if (cert is CertificateFindCriteria c)
                        {
                            return !String.IsNullOrWhiteSpace(c.CertificateFindValue);
                        }

                        if (cert is PrivateKeyCertificate k)
                        {
                            return !String.IsNullOrWhiteSpace(k.Certificate)
                                   && !String.IsNullOrWhiteSpace(k.Password);
                        }

                        return false;
                    })
                    .WithMessage(
                        "Security.Signing should have either an <CertificateFindCriteria/> " +
                        "with a non-empty <CertificateFindValue/> or an <PrivateKeyCertificate/> element " +
                        "with a non-empty <Certificate/> and <Password/> elements " +
                        "when the Security.Signing.IsEnabled = true");
            });
        }

        private void RulesForEncryption()
        {
            When(pmode => pmode.Security.Encryption.IsEnabled && !IsDynamicDiscovery(pmode), () =>
            {
                RuleFor(pmode => pmode.Security.Encryption.EncryptionCertificateInformation)
                    .Must(cert =>
                    {
                        if (cert is CertificateFindCriteria c)
                        {
                            return !String.IsNullOrWhiteSpace(c.CertificateFindValue);
                        }

                        if (cert is PublicKeyCertificate k)
                        {
                            return !String.IsNullOrWhiteSpace(k.Certificate);
                        }

                        return false;
                    })
                    .WithMessage(
                        "Security.Encryption should have either an <CertificateFindCriteria/> " +
                        "with a non-empty <CertificateFindValue/> or an <PublicKeyCertificate/> element " +
                        "with a non-empty <Certificate/> element when the Security.Signing.IsEnabled = true");
            });
        }

        private static bool IsDynamicDiscovery(SendingProcessingMode pmode)
        {
            return !string.IsNullOrWhiteSpace(pmode?.DynamicDiscovery?.SmpProfile);
        }

        /// <summary>
        /// Validates the specified instance
        /// </summary>
        /// <param name="instance">The object to validate</param>
        /// <returns>A ValidationResult object containing any validation failures</returns>
        public override ValidationResult Validate(SendingProcessingMode instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            try
            {
                ValidateKeySize(instance);
            }
            catch (Exception exception)
            {
                Logger.Debug(exception);
            }

            return base.Validate(instance);
        }

        private static void ValidateKeySize(SendingProcessingMode model)
        {
            if (model.Security?.Encryption?.IsEnabled == true)
            {
                var keysizes = new[] { 128, 192, 256 };
                int actualKeySize = model.Security.Encryption.AlgorithmKeySize;

                if (!keysizes.Contains(actualKeySize) && model.Security?.Encryption != null)
                {
                    int defaultKeySize = Encryption.Default.AlgorithmKeySize;
                    Logger.Warn(
                        $"Invalid Encryption 'Key Size': {actualKeySize}, {defaultKeySize} is taken as default");
                    model.Security.Encryption.AlgorithmKeySize = defaultKeySize;
                }
            }
        }
    }
}