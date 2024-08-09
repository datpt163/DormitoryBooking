using CommonModel.Message;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RoomWebApi.Common.Data;
using RoomWebApi.Common.Repository;

namespace RoomWebApi.Consumer
{
    public class GetPriceOfRoomConsumer : IConsumer<GetPriceMessage>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPriceOfRoomConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<GetPriceMessage> context)
        {
            var room = _unitOfWork.Rooms.FindByCondition(c => c.Id == context.Message.roomId).Include(c => c.Type).FirstOrDefault();

            if (room == null)
            {
                await context.RespondAsync(new ResponseMessage<float>() { ErrorMessage = "Room not found" });
            }
            else
            {
                if(room.IsAvailble == false)
                {
                    await context.RespondAsync(new ResponseMessage<float>() { ErrorMessage = "Room is not availble" });
                }
                else
                {
                    await context.RespondAsync(new ResponseMessage<float>() { Data = room.Type.Price });
                }
            }
        }
    }
}
