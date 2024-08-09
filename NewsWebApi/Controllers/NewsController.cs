using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NewsWebApi.Controllers
{
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetNews()
        {
            return Ok("23");
        }
    }
}
