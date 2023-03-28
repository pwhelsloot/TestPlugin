using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AMCS.TestPlugin.Server.Controllers
{
  [ApiController]
  public class TestController : Controller
  {
    [Route("gettestmessage")]
    [HttpGet]
    public async Task<IActionResult> GetTestMessage(string deviceId)
    {
      object responses = new
      {
        TestMessage = "Dit is een test message"
      };

      return Json(new { TestResponse = JsonConvert.SerializeObject(responses) });
    }
  }
}