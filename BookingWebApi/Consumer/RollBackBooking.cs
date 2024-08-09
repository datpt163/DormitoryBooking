using BookingWebApi.Common.Data;
using BookingWebApi.Common.Repository;
using CommonModel.Message;
using MassTransit;

namespace BookingWebApi.Consumer
{
    public class RollBackBookingConsumer : IConsumer<RollBackBooking>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RollBackBookingConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<RollBackBooking> context)
        {
            var booking = _unitOfWork.Bookings.FindByCondition(c => c.Id == context.Message.BookingId).FirstOrDefault();

            if(booking != null)
            {
                _unitOfWork.Bookings.Remove(booking);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
