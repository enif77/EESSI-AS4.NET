﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Eu.EDelivery.AS4.Fe.Authentication;
using Eu.EDelivery.AS4.Fe.SmpConfiguration.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Eu.EDelivery.AS4.Fe.SmpConfiguration
{
    /// <summary>
    ///     Smp configuration controller
    /// </summary>
    [Route("api/[controller]")]
    public class SmpConfigurationController
    {
        private readonly ISmpConfigurationService _smpConfiguration;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SmpConfigurationController" /> class.
        /// </summary>
        /// <param name="smpConfiguration">The SMP configuration.</param>
        public SmpConfigurationController(ISmpConfigurationService smpConfiguration)
        {
            _smpConfiguration = smpConfiguration;
        }

        /// <summary>
        ///     Get all Smp configurations
        /// </summary>
        /// <returns>List of SMP configurations</returns>
        [HttpGet]
        [SwaggerResponse((int) HttpStatusCode.OK, "All SMP configurations returned.", typeof(IEnumerable<SmpConfigurationRecord>))]
        public async Task<IEnumerable<SmpConfigurationRecord>> Get()
        {
            return await _smpConfiguration.GetRecordsAsync();
        }

        /// <summary>
        ///     Gets Smp configuration by identifier
        /// </summary>
        /// <param name="id">The identifier</param>
        /// <returns>Matching Smp configuration</returns>
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse((int) HttpStatusCode.OK, "All SMP configurations by identifier returned.", typeof(IEnumerable<SmpConfigurationDetail>))]
        public async Task<SmpConfigurationDetail> Get(int id)
        {
            return await _smpConfiguration.GetByIdAsync(id);
        }

        /// <summary>
        ///     Posts the specified SMP configuration.
        /// </summary>
        /// <param name="smpConfiguration">The SMP configuration.</param>
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [SwaggerResponse((int) HttpStatusCode.OK, "A specified SMP configuration is posted.", typeof(OkResult))]
        public async Task<IActionResult> Post([FromBody] SmpConfigurationDetail smpConfiguration)
        {
            SmpConfigurationDetail configuration = await _smpConfiguration.CreateAsync(smpConfiguration);
            return new OkObjectResult(configuration);
        }

        /// <summary>
        ///     Puts the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="smpConfiguration">The SMP configuration.</param>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        [SwaggerResponse((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Put(int id, [FromBody] SmpConfigurationDetail smpConfiguration)
        {         
            await _smpConfiguration.UpdateAsync(id, smpConfiguration);
            return new OkResult();
        }

        /// <summary>
        ///     Delete an existing <see cref="Entities.SmpConfiguration" />
        /// </summary>
        /// <param name="id">The id of the <see cref="Entities.SmpConfiguration" /></param>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        [SwaggerResponse((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(int id)
        {
            await _smpConfiguration.DeleteAsync(id);
            return new OkResult();
        }
    }
}