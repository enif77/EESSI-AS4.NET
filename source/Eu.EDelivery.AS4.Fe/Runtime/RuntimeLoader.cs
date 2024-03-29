﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Eu.EDelivery.AS4.Fe.Settings;
using Eu.EDelivery.AS4.Model.PMode;
using Eu.EDelivery.AS4.Receivers;
using Eu.EDelivery.AS4.Repositories;
using Eu.EDelivery.AS4.Services.DynamicDiscovery;
using Eu.EDelivery.AS4.Steps;
using Eu.EDelivery.AS4.Strategies.Sender;
using Eu.EDelivery.AS4.Strategies.Uploader;
using Eu.EDelivery.AS4.Transformers;
using Microsoft.Extensions.Options;
using NLog;

namespace Eu.EDelivery.AS4.Fe.Runtime
{
    /// <summary>
    /// Runtime load
    /// </summary>
    /// <seealso cref="IRuntimeLoader" />
    public class RuntimeLoader : IRuntimeLoader
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Type[] ConfiguredAttributes =
        {
            typeof(InfoAttribute),
            typeof(DescriptionAttribute),
            typeof(DefaultValueAttribute),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeLoader"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private RuntimeLoader(IOptions<ApplicationSettings> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (settings.Value == null)
            {
                throw new ArgumentException(@"Application settings doesn't contain any value", nameof(settings));
            }

            Initialize(settings.Value.Runtime);
        }

        /// <summary>
        /// Creates an initialized loader instance to have access to the runtime types.
        /// </summary>
        /// <param name="settings">The settings containing the runtime folder to search for runtime assemblies.</param>
        public static RuntimeLoader Initialize(IOptions<ApplicationSettings> settings)
        {
            return new RuntimeLoader(settings);
        }

        /// <summary>
        /// Gets the receivers.
        /// </summary>
        /// <value>
        /// The receivers.
        /// </value>
        public IEnumerable<ItemType> Receivers { get; private set; }
        
        /// <summary>
        /// Gets the steps.
        /// </summary>
        /// <value>
        /// The steps.
        /// </value>
        public IEnumerable<ItemType> Steps { get; private set; }
        
        /// <summary>
        /// Gets the transformers.
        /// </summary>
        /// <value>
        /// The transformers.
        /// </value>
        public IEnumerable<ItemType> Transformers { get; private set; }
        
        /// <summary>
        /// Gets the certificate repositories.
        /// </summary>
        /// <value>
        /// The certificate repositories.
        /// </value>
        public IEnumerable<ItemType> CertificateRepositories { get; private set; }

        /// <summary>
        /// Gets the deliver senders.
        /// </summary>
        /// <value>
        /// The deliver senders.
        /// </value>
        public IEnumerable<ItemType> DeliverSenders { get; private set; }
        
        /// <summary>
        /// Gets the notify senders.
        /// </summary>
        /// <value>
        /// The notify senders.
        /// </value>
        public IEnumerable<ItemType> NotifySenders { get; private set; }
        
        /// <summary>
        /// Gets the attachment uploaders.
        /// </summary>
        /// <value>
        /// The attachment uploaders.
        /// </value>
        public IEnumerable<ItemType> AttachmentUploaders { get; private set; }
        
        /// <summary>
        /// Gets the dynamic discovery profiles.
        /// </summary>
        /// <value>
        /// The dynamic discovery profiles.
        /// </value>
        public IEnumerable<ItemType> DynamicDiscoveryProfiles { get; private set; }
        
        /// <summary>
        /// Gets the receiving pmode.
        /// </summary>
        /// <value>
        /// The receiving pmode.
        /// </value>
        public IEnumerable<ItemType> MetaData { get; private set; }

        /// <summary>
        /// Load the information from the AS4 runtime
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IRuntimeLoader Initialize(string folder)
        {
            if (String.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentException(@"Folder cannot be null or whitespace.", nameof(folder));
            }

            if (!Directory.Exists(folder))
            {
                throw new Exception($"The module folder {folder} doesn't exist");
            }

            
            IDictionary<Type, Type[]> implementationsByInterface = LoadTypeImplementationsByInterfaceFromFiles(folder);
            ItemType[] CreateItemsForType<T>() => implementationsByInterface[typeof(T)].Select(CreateItemForType).ToArray();

            Receivers = CreateItemsForType<IReceiver>();
            Steps = CreateItemsForType<IStep>();
            Transformers = CreateItemsForType<ITransformer>();
            CertificateRepositories = CreateItemsForType<ICertificateRepository>();
            DeliverSenders = CreateItemsForType<IDeliverSender>();
            NotifySenders = CreateItemsForType<INotifySender>();
            AttachmentUploaders = CreateItemsForType<IAttachmentUploader>();
            DynamicDiscoveryProfiles = CreateItemsForType<IDynamicDiscoveryProfile>();
            MetaData = CreateItemsForType<IPMode>()
                       .Concat(new[] { CreateItemForType(typeof(Entities.SmpConfiguration)) })
                       .ToArray();

            return this;
        }

        private static IDictionary<Type, Type[]> LoadTypeImplementationsByInterfaceFromFiles(string folder)
        {
            Type[] interfaces = 
            {
                typeof(IReceiver),
                typeof(IStep),
                typeof(ITransformer),
                typeof(ICertificateRepository),
                typeof(IDeliverSender),
                typeof(INotifySender),
                typeof(IAttachmentUploader),
                typeof(IDynamicDiscoveryProfile),
                typeof(IPMode)
            };
            Type notConfigurableAttr = typeof(NotConfigurableAttribute);

            IEnumerable<Type> implementations =
                Directory
                    .GetFiles(folder, "Eu.EDelivery.AS4*.dll")
                    .Where(f => !f.Contains("Test"))
                    .SelectMany(LoadTypesFromAssemblyPath)
                    .Where(t => !t.IsInterface
                                && !t.IsAbstract
                                && interfaces.Any(i => t.GetInterface(i.Name) != null)
                                && !t.IsDefined(notConfigurableAttr));

            return interfaces.Select(
                face => new KeyValuePair<Type, Type[]>(
                    face, implementations.Where(
                        impl => impl.GetInterface(face.Name) != null).ToArray()))
                             .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private static IEnumerable<Type> LoadTypesFromAssemblyPath(string file)
        {
            try
            {
                return Assembly.LoadFile(Path.GetFullPath(file))
                               .GetExportedTypes();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Enumerable.Empty<Type>();
            }
        }

        private static ItemType CreateItemForType(Type type)
        {
            var infoAttr = type.GetCustomAttribute<InfoAttribute>();
            var descAttr = type.GetCustomAttribute<DescriptionAttribute>();

            return new ItemType
            {
                Name = infoAttr?.FriendlyName ?? type.Name,
                Description = descAttr?.Description ?? String.Empty,
                TechnicalName = type.AssemblyQualifiedName,
                Properties = LoadPropertiesForType(type.Name.ToLower(), type).ToArray()
            };
        }

        private static IEnumerable<Property> LoadPropertiesForType(string parentPath, Type type)
        {
            IEnumerable<PropertyInfo> propertyInfos =
                type.GetRuntimeProperties()
                    .Where(p => ConfiguredAttributes.Any(p.IsDefined))
                    .ToArray();

            if (!propertyInfos.Any())
            {
                return Enumerable.Empty<Property>();
            }

            return propertyInfos.Select(p =>
            {
                var infoAttr = p.GetCustomAttribute<InfoAttribute>();
                var descAttr = p.GetCustomAttribute<DescriptionAttribute>();
                var defvAttr = p.GetCustomAttribute<DefaultValueAttribute>();

                string childPath = parentPath + "." + p.Name.ToLower();
                return new Property
                {
                    Type = infoAttr?.Type ?? p.Name,
                    TechnicalName = p.Name,
                    Description = descAttr?.Description ?? String.Empty,
                    FriendlyName = infoAttr?.FriendlyName ?? p.Name,
                    Regex = infoAttr?.Regex ?? String.Empty,
                    Required = infoAttr?.Required,
                    DefaultValue = defvAttr?.Value ?? infoAttr?.DefaultValue,
                    Attributes = infoAttr?.Attributes,
                    Path = childPath,
                    Properties = LoadPropertiesForType(childPath, p.PropertyType).ToArray()
                };
            });
        }
    }
}