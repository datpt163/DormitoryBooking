using CommonModel;
using CommonModel.Message;
using JwtAuthenticationManagement;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RoomWebApi.Common.Data;
using RoomWebApi.Common.Repository;
using System.Security.Principal;

namespace RoomWebApi.Servies
{
    public interface IRoomService
    {
        Task<ResponseService> GetCurrentRoom(string token);
    }
    public class RoomService : IRoomService
    {
        private readonly IRequestClient<GetCurrentRoom> _client;
        private readonly JwtTokenHandle _jwtTokenHandle;
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(IUnitOfWork unitOfWork, JwtTokenHandle jwtTokenHandle, IRequestClient<GetCurrentRoom> client)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenHandle = jwtTokenHandle;
            _client = client;
        }

        public async Task<ResponseService> GetCurrentRoom(string token)
        {
            var userId = _jwtTokenHandle.VerifyToken(token);
            var response = await _client.GetResponse<ResponseMessage<Guid>>(new GetCurrentRoom() { UserId = userId });

            var room = await _unitOfWork.Rooms.FindByCondition(c => c.Id == response.Message.Data).FirstOrDefaultAsync();

            if (room is null)
                return new ResponseService(){ ErrorMessage = "Null", Data = null};
            return new ResponseService() { ErrorMessage = "", Data = room };
        }
    }
}
