using AuthenticationWebApi.Common.Data;
using AuthenticationWebApi.Common.Repository;
using AuthenticationWebApi.Model;
using CommonModel;
using CommonModel.Message;
using JwtAuthenticationManagement;
using MassTransit;
using MassTransit.Clients;
using Microsoft.EntityFrameworkCore;
namespace AuthenticationWebApi.Services
{

    public interface IAccountService
    {
        Task<ResponseService> AuthAsync(LoginRequest accountAuthRequest);
        Task<ResponseService> Reset();
        Task<ResponseService> Reserve(string token);

    }
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtTokenHandle _jwthandle;
        private readonly IRequestClient<ResetRoomMessage> _client;

        public AccountService(IUnitOfWork unitOfWork, JwtTokenHandle jwtTokenHandle, IRequestClient<ResetRoomMessage> client) 
        {
            _unitOfWork = unitOfWork;
            _client = client;
            _jwthandle = jwtTokenHandle;
        }

        public async Task<ResponseService> AuthAsync(LoginRequest accountAuthRequest)
        {
            var ac = _unitOfWork.Users.FindByCondition(s => s.Email.Equals(accountAuthRequest.email) && s.Password.Equals(accountAuthRequest.password)).Include(s => s.Roles).FirstOrDefault();
            if (ac is null)
            {
                return new ResponseService() {ErrorMessage = "Account not found", Data = null };
            }

            var claimInfo = new ClainmInfor() { UserId = ac.Id };

            foreach (var r in ac.Roles)
                claimInfo.Roles.Add(new RoleInfo() { Id = r.Id, Name = r.Name });

            var token = await _jwthandle.GenerateJwtTokenTw(claimInfo);
            return new ResponseService() { ErrorMessage = "", Data = new { accessToken = token, refreshToken = "" }};
        }

        public async Task<ResponseService> Reserve(string token)
        {
           var userId =  _jwthandle.VerifyToken(token);
            var user = _unitOfWork.Users.FindByCondition(c => c.Id == userId).FirstOrDefault();

            if (user == null)
                return new ResponseService() { ErrorMessage = "User not found", Data = null };

            user.Active = true;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService() { ErrorMessage = "", Data = null };
        }

        public async Task<ResponseService> Reset()
        {
            var list = _unitOfWork.Users.FindByCondition(c => c.CurrenRoomId != null);

            foreach(var r in list)
            {
                if(r.Active == true)
                {
                    r.Active = false;
                }
                else
                {
                    var response = await _client.GetResponse<ResponseMessage<object>>(new ResetRoomMessage() { roomId = r.Id });
                    if (!string.IsNullOrEmpty(response.Message.ErrorMessage))
                    {
                        return new ResponseService() { ErrorMessage = "Something Wrong", Data = null };
                    }
                    r.CurrenRoomId = null;
                }
            }
            _unitOfWork.Users.UpdateRange(list);
            _unitOfWork.SaveChanges();
            return new ResponseService() { ErrorMessage = "", Data = null };
        }
    }
}
