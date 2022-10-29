﻿using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Eu.EDelivery.AS4.Fe.Runtime
{
    /// <summary>
    /// Controller to get AS4 runtime types
    /// </summary>
    [Route("api/[controller]")]
    [ResponseCache(Duration = 60)]
    public class RuntimeController
    {
        private readonly IRuntimeLoader runtimeLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeController"/> class.
        /// </summary>
        /// <param name="runtimeLoader">The runtime loader.</param>
        public RuntimeController(IRuntimeLoader runtimeLoader)
        {
            this.runtimeLoader = runtimeLoader;
        }

        /// <summary>
        /// Get receivers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getreceivers")]
        [SwaggerResponse((int)HttpStatusCode.OK, "A list of receivers is returned.", typeof(OkResult))]
        public IEnumerable<ItemType> GetReceivers()
        {
            return runtimeLoader.Receivers;
        }

        /// <summary>
        /// Get a list of steps.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getsteps")]
        [SwaggerResponse((int)HttpStatusCode.OK, "A list of steps is returned.", typeof(OkResult))]
        public IEnumerable<ItemType> GetSteps()
        {
            return runtimeLoader.Steps;
        }

        /// <summary>
        /// Gets the transformer list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("gettransformers")]
        [SwaggerResponse((int)HttpStatusCode.OK, "A list of transformers returned.", typeof(OkResult))]
        public IEnumerable<ItemType> GetTransformerList()
        {
            return runtimeLoader.Transformers;
        }

        /// <summary>
        /// Gets the certificate repositories.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcertificaterepositories")]
        [SwaggerResponse((int)HttpStatusCode.OK, "A list of certificate repositories returned.", typeof(OkResult))]
        public IEnumerable<ItemType> GetCertificateRepositories()
        {
            return runtimeLoader.CertificateRepositories;
        }

        /// <summary>
        /// Gets the deliver senders.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getdeliversenders")]
        [SwaggerResponse((int)HttpStatusCode.OK, "A list of deliver senders returned.", typeof(OkResult))]
        public IEnumerable<ItemType> GetDeliverSenders()
        {
            return runtimeLoader.DeliverSenders;
        }

        /// <summary>
        /// Gets the attachment uploaders.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getattachmentuploaders")]
        [SwaggerResponse((int)HttpStatusCode.OK, "A list of attachment uploaders returned.", typeof(OkResult))]
        public IEnumerable<ItemType> GetAttachmentUploaders()
        {
            return runtimeLoader.AttachmentUploaders;
        }

        /// <summary>
        /// Gets the dynamic discovery profiles.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getdynamicdiscoveryprofiles")]
        [SwaggerResponse((int) HttpStatusCode.OK, "A list of dynamic discovery profiles returned.", typeof(OkResult))]
        public IEnumerable<ItemType> GetDynamicDiscoveryProfiles()
        {
            return runtimeLoader.DynamicDiscoveryProfiles;
        }

        /// <summary>
        /// Gets the notify senders.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getnotifysenders")]
        [SwaggerResponse((int)HttpStatusCode.OK, "A list od notify senders returned.", typeof(OkResult))]
        public IEnumerable<ItemType> GetNotifySenders()
        {
            return runtimeLoader.NotifySenders;
        }

        /// <summary>
        /// Gets all runtime types.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getall")]
        [AllowAnonymous]
        [SwaggerResponse((int)HttpStatusCode.OK, "A list of runtime types returned.", typeof(OkResult))]
        public IActionResult GetAllRuntimeTypes()
        {
            return new ObjectResult(new
            {
                Receivers = GetReceivers(),
                Steps = GetSteps(),
                Transformers = GetTransformerList(),
                CertificateRepositories = GetCertificateRepositories(),
                DeliverSenders = GetDeliverSenders(),
                NotifySenders = GetNotifySenders(),
                AttachmentUploaders = GetAttachmentUploaders(),
                DynamicDiscoveryProfiles = GetDynamicDiscoveryProfiles(),
                RuntimeMetaData = runtimeLoader.MetaData
            });
        }

        /// <summary>
        /// Get metadata for all the AS4 runtime types.
        /// This will return all the attributes used in the code.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getruntimemetadata")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Metadata for all runtime types returned.", typeof(OkResult))]
        public IActionResult GetRuntimeMetaData()
        {
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(runtimeLoader.MetaData),
                ContentType = "application/json"
            };
        }
    }
}