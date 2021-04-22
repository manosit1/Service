using Microsoft.AspNetCore.Mvc;

namespace workshop.Controllers
{
    [Route("api/")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        Services.GetNews service = new Services.GetNews ();
        [HttpGet]
        [Route("GetMessage")]
        public  IActionResult GetMessage(){
         
            return Ok(service.GetNewsRead());
        
        }
        [HttpPost]
        [Route("CreateNews")]
        public IActionResult CreateNews(Models.RegisterNews input){
            return Ok(service.AddNews(input));
        }
    }
}