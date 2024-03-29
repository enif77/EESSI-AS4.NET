﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eu.EDelivery.AS4.Model.Internal;

namespace Eu.EDelivery.AS4.Steps
{
    /// <summary>
    /// Interface <see cref="IStep" /> to describe a single step to execute
    /// </summary>
    public interface IStep
    {
        /// <summary>
        /// Execute the step on a given <paramref name="messagingContext"/>.
        /// </summary>
        /// <param name="messagingContext"><see cref="MessagingContext"/> on which the step must be executed.</param>
        /// <returns></returns>
        Task<StepResult> ExecuteAsync(MessagingContext messagingContext);
    }

    /// <summary>
    /// Versioned interface for the <see cref="IStep"/> interface
    /// to make the <see cref="IStep"/> implementation configurable
    /// </summary>
    public interface IConfigStep : IStep
    {
        /// <summary>
        /// Configure the step with a given Property Dictionary
        /// </summary>
        /// <param name="properties"></param>
        void Configure(IDictionary<string, string> properties);
    }
}