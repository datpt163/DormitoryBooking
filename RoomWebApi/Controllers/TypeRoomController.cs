using CommonModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoomWebApi.Common.Data;
using RoomWebApi.Servies;
namespace RoomWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeRoomController : BaseController
    {
        private readonly ITypeRoomService _typeRoomService;
        public TypeRoomController (ITypeRoomService typeRoomService)
        {
            _typeRoomService = typeRoomService;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            var responseService = _typeRoomService.GetAll();

            if (!string.IsNullOrEmpty(responseService.ErrorMessage))
                return ResponseBadRequest(messageResponse: responseService.ErrorMessage);
            return ResponseOk(dataResponse: responseService.Data);
        }

        [HttpGet("{id}/rooms")]
        public async Task<IActionResult> GetListRoom(Guid id)
        {
            var responseService = await _typeRoomService.GetRoomsByTypeId(id);

            if (!string.IsNullOrEmpty(responseService.ErrorMessage))
                return ResponseBadRequest(messageResponse: responseService.ErrorMessage);
            return ResponseOk(dataResponse: responseService.Data);
        }
    }
}
