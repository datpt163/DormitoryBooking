using AuthenticationWebApi.Common.Data;
using AuthenticationWebApi.Common.Repository;
using CommonModel.Message;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationWebApi.Consumer
{
    public class GetCurrentRoomConsumer : IConsumer<GetCurrentRoom>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetCurrentRoomConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<GetCurrentRoom> context)
        {
            var user = _unitOfWork.Users.FindByCondition(c => c.Id == context.Message.UserId).FirstOrDefault();

            if(user != null)
            {
                if(user.CurrenRoomId is null)
                    await context.RespondAsync(new ResponseMessage<Guid>() { ErrorMessage = "User not have room" });
                else
                await context.RespondAsync(new ResponseMessage<Guid>() { Data = user.CurrenRoomId.Value });
            }
            else
            await context.RespondAsync(new ResponseMessage<Guid>() {ErrorMessage = "User not found" });
        }
    }
}
