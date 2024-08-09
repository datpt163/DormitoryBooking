using CommonModel.Message;
using MassTransit;
using RoomWebApi.Common.Data;
using RoomWebApi.Common.Repository;

namespace RoomWebApi.Consumer
{
    public class ResetConsumer : IConsumer<ResetRoomMessage>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResetConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Consume(ConsumeContext<ResetRoomMessage> context)
        {
            var room = _unitOfWork.Rooms.FindByCondition(c => c.Id == context.Message.roomId).FirstOrDefault();

            if (room != null)
            {
                room.CurrentPeople--;
                room.IsAvailble = true;
                _unitOfWork.Rooms.Update(room);
                await _unitOfWork.SaveChangesAsync();
                await context.RespondAsync(new ResponseMessage<object>());
            }
            else
                await context.RespondAsync(new ResponseMessage<RoomResponse>() { ErrorMessage = "Room not found" });
        }
    }
}
