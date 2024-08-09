using CommonModel.Message;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RoomWebApi.Common.Data;
using RoomWebApi.Common.Repository;

namespace RoomWebApi.Consumer
{
    public class GetRoomConsumer : IConsumer<GetRoomMessage>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoomConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<GetRoomMessage> context)
        {
            var room = _unitOfWork.Rooms.FindByCondition(c => c.Id == context.Message.RoomId).Include(c => c.Type).FirstOrDefault();

            if (room != null)
                await context.RespondAsync(new ResponseMessage<RoomResponse>() { Data = new RoomResponse() { Capacity = room.Type.Capacity, Price = room.Type.Price  } });
            else
                await context.RespondAsync(new ResponseMessage<RoomResponse>() { ErrorMessage = "Room not found" });
        }
    }
}

