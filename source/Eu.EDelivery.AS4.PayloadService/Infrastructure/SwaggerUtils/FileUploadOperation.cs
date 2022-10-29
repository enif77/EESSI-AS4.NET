using System.Collections.Generic;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Eu.EDelivery.AS4.PayloadService.Infrastructure.SwaggerUtils
{
    /// <summary>
    /// <see cref="IOperationFilter"/> implementation to implement a 'File Upload' into Swagger.
    /// </summary>
    public class FileUploadOperation : IOperationFilter
    {
        /// <summary>
        /// Apply the 'File Upload' to the given <paramref name="operation"/>.
        /// </summary>
        /// <param name="operation">The Operation.</param>
        /// <param name="context">The Context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // TODO: Recreate the FileUploadOperation.

            //if (operation.OperationId == null)
            //{
            //    return;  // TODO: How to set OperationId?
            //}

            //if (operation.OperationId.ToLower() == "apipayloaduploadpost")
            //{
            //    if (operation.Parameters == null)
            //    {
            //        operation.Parameters = new List<OpenApiParameter>();
            //    }
                
            //    operation.Parameters.Add(new NonBodyParameter
            //    {
            //        Name = "File",
            //        In = "formData",
            //        Description = "Upload Payload content",
            //        Required = true,
            //        Type = "file"
            //    });

            //    operation.Consumes.Add("application/form-data");
            //}
        }
    }    
}
