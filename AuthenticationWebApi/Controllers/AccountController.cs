using AuthenticationWebApi.Model;
using AuthenticationWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CommonModel;
using AuthenticationWebApi.Common.Data;
using Newtonsoft.Json.Linq;
namespace AuthenticationWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accService;


        public AccountController(IAccountService acountService)
        {
            _accService = acountService;
        }

        [HttpPost("Auth")]
        public async Task<IActionResult> Authenticate(LoginRequest loginRequest)
        {
            var response = await _accService.AuthAsync(loginRequest);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                return ResponseBadRequest(response.ErrorMessage);
            return ResponseOk(response.Data);
        }

        [HttpGet("reset")]
        public async Task<IActionResult> Reset()
        {
            var responseService = await _accService.Reset();
            if (!string.IsNullOrEmpty(responseService.ErrorMessage))
                return ResponseBadRequest(responseService.ErrorMessage);
            return ResponseNoContent();
        }

        [HttpGet("reserve-room-next-semester")]
        public async Task<IActionResult> ReserveRoomForNextSemester()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var response = await _accService.Reserve(token);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                return ResponseBadRequest(response.ErrorMessage);
            return ResponseNoContent();
        }
    }
}
