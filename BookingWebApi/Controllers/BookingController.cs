using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CommonModel;
using BookingWebApi.Services;
using JwtAuthenticationManagement;
namespace BookingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : BaseController
    {
        private readonly IBookingService _bookingService;
       
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> Booking(Guid roomId)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var response = await _bookingService.Booking(token, roomId);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
                return ResponseBadRequest(response.ErrorMessage);
            return ResponseNoContent();
        }
            
        [HttpGet("GetList")]
        public async Task<IActionResult> Getlist(Guid roomId)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var response = await _bookingService.GetList(token);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
                return ResponseBadRequest(response.ErrorMessage);
            return ResponseOk(response.Data);
        }
    }
}
