using MassTransit;
using CommonModel;
using JwtAuthenticationManagement;
using CommonModel.Message;
using Microsoft.EntityFrameworkCore;
using BookingWebApi.Common.Data;
using BookingWebApi.Common.Repository;
namespace BookingWebApi.Services
{
    public interface IBookingService
    {
        public Task<ResponseService> Booking(string token, Guid roomId);
        public Task<ResponseService> GetList(string token);
    }

    public class BookingService : IBookingService
    {
        
        private readonly JwtTokenHandle _jwtTokenHandle;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestClient<PaymentMessage> _client;
        private readonly IRequestClient<GetRoomMessage> _clientGetRoom;

        public BookingService(JwtTokenHandle jwtTokenHandel, IUnitOfWork unitOfWork, IRequestClient<PaymentMessage> client, IRequestClient<GetRoomMessage> clientGetRoom)
        {
            _jwtTokenHandle = jwtTokenHandel;
            _unitOfWork = unitOfWork;
            _client = client;
            _clientGetRoom = clientGetRoom;
        }
        public async Task<ResponseService> Booking(string token, Guid roomId)
        {
            var userId = _jwtTokenHandle.VerifyToken(token);

            var semesters = _unitOfWork.Semesters.FindAll();
            DateTime currentDate = DateTime.Now;
            var bookingId = Guid.NewGuid();
            foreach (var s in semesters)
            {
                if (currentDate >= s.StartDate && currentDate <= s.EndDate)
                {
                    _unitOfWork.Bookings.Add(new Booking() { Id = bookingId, RoomId = roomId, UserId = userId, SemesterId = s.Id });
                    break;
                }
            }
            await _unitOfWork.SaveChangesAsync();
            var response = await _client.GetResponse<ResponseMessage<object>>(new PaymentMessage {Bookingid = bookingId,  RoomId = roomId, UserId = userId });

            return new ResponseService() { ErrorMessage = response.Message.ErrorMessage };

        }

        public async Task<ResponseService> GetList(string token)
        {
            var userId = _jwtTokenHandle.VerifyToken(token);
            var listBooking = _unitOfWork.Bookings.FindByCondition(c => c.UserId == userId).Include(c => c.Semester).ToList();
            var list = new List<ResponseGetBooking>();

            try
            {
                foreach (var s in listBooking)
                {
                    var room = await _clientGetRoom.GetResponse<ResponseMessage<RoomResponse>>(new GetRoomMessage() { RoomId = s.RoomId });
                    list.Add(new ResponseGetBooking() { semester = s.Semester.Name ?? String.Empty , capacity = room.Message.Data.Capacity, price = room.Message.Data.Price });
                }

                return new ResponseService() {  Data = list };
            }
            catch
            {
                return new ResponseService(){ ErrorMessage = "Some thing wrong", Data = null};
            }
        }

        public class ResponseGetBooking
        {
            public string semester { get; set; } = string.Empty;
            public int capacity { get; set; }
            public float price { get; set; }
        }
    }
}
