using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Eu.EDelivery.AS4.Fe.Monitor.Model
{
    /// <summary>
    /// Monitor controller
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class MonitorController : Controller
    {
        private readonly IMonitorService monitorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorController"/> class.
        /// </summary>
        /// <param name="monitorService">The monitor service.</param>
        public MonitorController(IMonitorService monitorService)
        {
            this.monitorService = monitorService;
        }

        /// <summary>
        /// Gets the in exceptions.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>ExceptionMessage</returns>
        [HttpGet]
        [Route("exceptions")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Returned when in exceptions are returned.", typeof(MessageResult<ExceptionMessage>))]
        [SwaggerResponse((int)HttpStatusCode.ExpectationFailed, "No messages are found.", typeof(ErrorModel))]
        public async Task<IActionResult> GetInExceptions(ExceptionFilter filter)
        {
            return new OkObjectResult(await monitorService.GetExceptions(filter));
        }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("messages")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Messages found.", typeof(MessageResult<Message>))]
        [SwaggerResponse((int)HttpStatusCode.ExpectationFailed, "No messages are found.", typeof(ErrorModel))]
        public async Task<IActionResult> GetMessages(MessageFilter filter)
        {
            return new OkObjectResult(await monitorService.GetMessages(filter));
        }

        /// <summary>
        /// Gets the related messages.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("relatedmessages")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Related messages returned.", typeof(MessageResult<Message>))]
        public async Task<IActionResult> GetRelatedMessages(Direction direction, string messageId)
        {
            return new OkObjectResult(await monitorService.GetRelatedMessages(direction, messageId));
        }

        /// <summary>
        /// Gets the message body.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="id">The message identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("messagebody")]
        [SwaggerResponse((int)HttpStatusCode.OK, "A message body returned.", typeof(MessageResult<Message>))]
        public async Task<FileStreamResult> GetMessageBody(Direction direction, long id)
        {
            return File(await monitorService.DownloadMessageBody(direction, id), "application/xml");
        }

        /// <summary>
        /// Gets the exception body.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="id">The message identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("exceptionbody")]
        [SwaggerResponse((int)HttpStatusCode.OK, "An exception body returned.", typeof(MessageResult<Message>))]
        public async Task<FileResult> GetExceptionBody(Direction direction, long id)
        {
            return File(await monitorService.DownloadExceptionMessageBody(direction, id), "application/txt");
        }

        [HttpGet]
        [Route("detail/{direction}/{id}")]
        public async Task<IActionResult> GetExceptionDetail(Direction direction, long id)
        {
            return new OkObjectResult(await monitorService.GetExceptionDetail(direction, id));
        }
    }
}