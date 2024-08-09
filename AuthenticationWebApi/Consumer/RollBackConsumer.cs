using AuthenticationWebApi.Common.Data;
using AuthenticationWebApi.Common.Repository;
using CommonModel.Message;
using MassTransit;

namespace AuthenticationWebApi.Consumer
{
    public class RollBackConsumer : IConsumer<RollBackUser>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestClient<GetPriceMessage> _client;
        private readonly IPublishEndpoint _publishEndpoint;

        public RollBackConsumer(IRequestClient<GetPriceMessage> client, IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _client = client;
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<RollBackUser> context)
        {
            var response = await _client.GetResponse<ResponseMessage<float>>(new GetPriceMessage { roomId = context.Message.RoomId });

            var user = _unitOfWork.Users.FindByCondition(c => c.Id == context.Message.UserId).FirstOrDefault();
            if(user != null)
            {
                user.CurrenRoomId = null;
                user.Balance = user.Balance + response.Message.Data;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();
            }
            await _publishEndpoint.Publish(new RollBackUserSucces() { BookingId = context.Message.Bookingid});

        }
    }
}
