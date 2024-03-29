﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Eu.EDelivery.AS4.Repositories;

namespace Eu.EDelivery.AS4.Strategies.Uploader
{
    /// <summary>
    /// Class to provide the right <see cref="IAttachmentUploader" /> implementation
    /// </summary>
    internal class AttachmentUploaderProvider : IAttachmentUploaderProvider
    {
        public static readonly IAttachmentUploaderProvider Instance = new AttachmentUploaderProvider();

        private readonly ICollection<UploaderEntry> _uploaders;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentUploaderProvider" /> class
        /// </summary>
        private AttachmentUploaderProvider()
        {
            _uploaders = new Collection<UploaderEntry>();

            this.Accept(s => StringComparer.OrdinalIgnoreCase.Equals(s, FileAttachmentUploader.Key), new FileAttachmentUploader());
            this.Accept(s => StringComparer.OrdinalIgnoreCase.Equals(s, EmailAttachmentUploader.Key), new EmailAttachmentUploader());
            this.Accept(s => StringComparer.OrdinalIgnoreCase.Equals(s, PayloadServiceAttachmentUploader.Key), new PayloadServiceAttachmentUploader());
        }

        /// <summary>
        /// Get the right <see cref="IAttachmentUploader" /> implementation
        /// for a given <paramref name="type" />
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IAttachmentUploader Get(string type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            UploaderEntry entry = _uploaders.FirstOrDefault(u => u.Condition(type));

            if (entry?.Uploader == null)
            {
                throw new KeyNotFoundException(
                    $"(Deliver) No {nameof(IAttachmentUploader)} impelemtation found for key: {type}");
            }

            return entry.Uploader;
        }

        /// <summary>
        /// Adds a new <see cref="IAttachmentUploader" /> implementation
        /// for a given <paramref name="condition" />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="uploader"></param>
        public void Accept(Func<string, bool> condition, IAttachmentUploader uploader)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            if (uploader == null)
            {
                throw new ArgumentNullException(nameof(uploader));
            }

            _uploaders.Add(new UploaderEntry(condition, uploader));
        }

        private class UploaderEntry
        {
            public UploaderEntry(Func<string, bool> condition, IAttachmentUploader uploader)
            {
                Condition = condition;
                Uploader = uploader;
            }

            public Func<string, bool> Condition { get; }

            public IAttachmentUploader Uploader { get; }
        }
    }

    public interface IAttachmentUploaderProvider
    {
        /// <summary>
        /// Accept a <see cref="IAttachmentUploader" /> implementation in the <see cref="IAttachmentUploaderProvider" />.
        /// </summary>
        /// <param name="condition">Condition for which the <see cref="IAttachmentUploader" /> must be used.</param>
        /// <param name="uploader"><see cref="IAttachmentUploader" /> implementation to be used.</param>
        void Accept(Func<string, bool> condition, IAttachmentUploader uploader);

        /// <summary>
        /// Get a <see cref="IAttachmentUploader" /> implementation based on a given <paramref name="type" />.
        /// </summary>
        /// <param name="type">The type for which the <see cref="IAttachmentUploader" /> implementation is accepted.</param>
        /// <returns></returns>
        IAttachmentUploader Get(string type);
    }
}