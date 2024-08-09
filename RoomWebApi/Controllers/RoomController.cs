using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CommonModel;
using JwtAuthenticationManagement;
using CommonModel.Message;
using MassTransit;
using RoomWebApi.Common.Data;
using RoomWebApi.Servies;
using RoomWebApi.Common.Repository;
namespace RoomWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : BaseController
    {
        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet("CurrentRoom")]
        public async Task<IActionResult> GetCurrentRoom()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var responseService = await _roomService.GetCurrentRoom(token);

            if (!string.IsNullOrEmpty(responseService.ErrorMessage))
                return ResponseBadRequest(messageResponse: responseService.ErrorMessage);
            return ResponseOk(dataResponse: responseService.Data);
           
        }
    }
}
