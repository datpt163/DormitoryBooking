using AuthenticationWebApi.Common.Data;
using AuthenticationWebApi.Common.Repository;
using CommonModel.Message;
using MassTransit;

namespace AuthenticationWebApi.Consumer
{
    public class PaymentConsumer : IConsumer<PaymentMessage>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestClient<GetPriceMessage> _client;
        private readonly IRequestClient<PaymentSuccess> _clientCallCreateRoom;

        public PaymentConsumer(IRequestClient<GetPriceMessage> client, IUnitOfWork unitOfWork, IRequestClient<PaymentSuccess> clientCallCreateRoom)
        {
            _unitOfWork = unitOfWork;
            _client = client;
            _clientCallCreateRoom = clientCallCreateRoom;   
        }

        public async Task Consume(ConsumeContext<PaymentMessage> context)
        {
            string errorMessage = "";
            var response = await _client.GetResponse<ResponseMessage<float>>(new GetPriceMessage { roomId = context.Message.RoomId });
            errorMessage = response.Message.ErrorMessage;
            if (string.IsNullOrEmpty(response.Message.ErrorMessage))
            {
                var user = _unitOfWork.Users.FindByCondition(c => c.Id == context.Message.UserId).FirstOrDefault();

                if(user != null)
                {
                    var fee = (float)response.Message.Data;
                    user.CurrenRoomId = context.Message.RoomId;
                    user.Balance = user.Balance - fee;

                    if (user.Balance < 0)
                        errorMessage = "Account don't have enough money";
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                     var response2 = await _clientCallCreateRoom.GetResponse<ResponseMessage<object>>(new PaymentSuccess {Bookingid = context.Message.Bookingid, RoomId = context.Message.RoomId, UserId = context.Message.UserId });
                    errorMessage = response2.Message.ErrorMessage;
                    
                }
            }
            await context.RespondAsync(new ResponseMessage<object>() { ErrorMessage = errorMessage });
        }
    }
}
