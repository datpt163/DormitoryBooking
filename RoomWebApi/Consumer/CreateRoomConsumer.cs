using CommonModel.Message;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RoomWebApi.Common.Data;
using RoomWebApi.Common.Repository;

namespace RoomWebApi.Consumer
{
    public class CreateRoomConsumer : IConsumer<CreateRoomMessage>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoomConsumer(IPublishEndpoint publishEndpoint, IUnitOfWork unitOfWork)
        {
            _publishEndpoint = publishEndpoint;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<CreateRoomMessage> context)
        {
            var room = _unitOfWork.Rooms.FindByCondition(c => c.Id == context.Message.RoomId).Include(c => c.Type).FirstOrDefault();

            if (room == null)
                await _publishEndpoint.Publish(new CreateRoomFaile() { Bookingid = context.Message.Bookingid, UserId = context.Message.UserId, RoomId = context.Message.RoomId });
            else
            {
                room.CurrentPeople++;

                if (room.CurrentPeople == room.Type.Capacity)
                    room.IsAvailble = false;
                _unitOfWork.Rooms.Update(room);
                await _unitOfWork.SaveChangesAsync();
                await _publishEndpoint.Publish(new CreateRoomSuccess() { UserId = context.Message.UserId, RoomId = context.Message.RoomId });
            }
        }
    }
}
