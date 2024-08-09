using CommonModel.Message;
using MassTransit;
using MassTransit.Transports;

namespace BookingWebApi.Saga
{
    public class InitBust
    {
        public static IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("amqp://localhost:5672/", h =>
            {
                h.Username("myuser");
                h.Password("mypass");
            });
        });
    }

    public class BookingSagaData : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
        public bool PaymentSuccess { get; set; }
        public bool CreateRoomFaile { get; set; }
        public bool CreateRoomSuccess { get; set; }
        public bool RollBackUserSucces { get; set; }
    }

    public class BookingSaga : MassTransitStateMachine<BookingSagaData>
    {
        public State UpdateStatusRoom { get; set; }
        public State AfterCreateRoom { get; set; }
        public State RollBack { get; set; }

        public Event<PaymentSuccess> PaymentSuccess { get; set; }

        public Event<CreateRoomSuccess> CreateRoomSuccess { get; set; }
        public Event<CreateRoomFaile> CreateRoomFaile { get; set; }
        public Event<RollBackUserSucces> RollBackUserSucces { get; set; }

        public BookingSaga()
        {
            InstanceState(x => x.CurrentState);

            Event(() => PaymentSuccess, e => e.CorrelateById(m => m.Message.Bookingid));
            Event(() => CreateRoomSuccess, e => e.CorrelateById(m => m.Message.Bookingid));
            Event(() => CreateRoomFaile, e => e.CorrelateById(m => m.Message.Bookingid));
            Event(() => RollBackUserSucces, e => e.CorrelateById(m => m.Message.BookingId));

            Initially(
              When(PaymentSuccess)
              .ThenAsync(async context =>
              {
                  context.Saga.RoomId = context.Message.RoomId;
                  context.Saga.UserId = context.Message.UserId;

                  if (context.Message.RoomId == Guid.Parse("f57d4b7f-b46f-4f5d-b8a8-cb64c4966a3b"))
                      await context.RespondAsync(new ResponseMessage<object> { ErrorMessage = "Service room is break" });
                  else
                  {
                      await context.RespondAsync(new ResponseMessage<object> { ErrorMessage = "" });
                  }
              }).Publish(context => new CreateRoomMessage() { Bookingid = context.Message.Bookingid, RoomId = context.Message.RoomId, UserId = context.Message.UserId })
              .TransitionTo(AfterCreateRoom));

            During(AfterCreateRoom,
                When(CreateRoomSuccess)
                .Finalize(),
                When(CreateRoomFaile)
                .Publish(context => (new RollBackUser() { Bookingid = context.Message.Bookingid, RoomId = context.Message.RoomId, UserId = context.Message.UserId }))
                  .Finalize()
                .TransitionTo(RollBack)
                );
            During(RollBack,
               When(RollBackUserSucces)
               .Publish(context => new RollBackBooking() { BookingId = context.Message.BookingId })
               .Finalize()
               );
        }
    }
}
