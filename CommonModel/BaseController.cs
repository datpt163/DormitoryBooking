using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace CommonModel
{
    public class BaseController : ControllerBase
    {
        protected IActionResult ResponseOk(dynamic? dataResponse = null, bool overrideBody = true)
        {
            return StatusCode((int)HttpStatusCode.OK, overrideBody ? new { data = dataResponse, statusCode = (int)HttpStatusCode.OK } : dataResponse);
        }

        protected IActionResult ResponseCreated(dynamic? dataResponse = null, string? messageResponse = null)
        {
            return StatusCode((int)HttpStatusCode.Created, new { data = dataResponse, message = messageResponse, statusCode = (int)HttpStatusCode.Created });
        }

        protected IActionResult ResponseNoContent()
        {
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        protected IActionResult ResponseBadRequest(string? messageResponse = null, dynamic? dataResponse = null, bool overrideBody = true)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, overrideBody ? new { Message = messageResponse, StatusCode = (int)HttpStatusCode.BadRequest } : dataResponse);
        }

        protected IActionResult ResponseUnauthorized(string? messageResponse = null)
        {
            return StatusCode((int)HttpStatusCode.Unauthorized, new { Message = messageResponse, StatusCode = (int)HttpStatusCode.Unauthorized });
        }

        protected IActionResult ResponseForbidden(string? messageResponse = null, int codeResponse = (int)HttpStatusCode.Forbidden)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, new { message = messageResponse, code = codeResponse, statusCode = (int)HttpStatusCode.Forbidden });
        }

        protected IActionResult ResponseInternalServerError(string? messageResponse = null, int codeResponse = (int)HttpStatusCode.InternalServerError)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = messageResponse, code = codeResponse, statusCode = (int)HttpStatusCode.InternalServerError });
        }
    }
}
