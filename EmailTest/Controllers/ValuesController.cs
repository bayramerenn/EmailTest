using Microsoft.AspNetCore.Mvc;

namespace EmailTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public ValuesController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                await _emailService.SendGeneratePassword("bayram.eren@experilabs.com", "test");
            }
            catch (Exception ex)
            {

                return Ok(ex);
            }
            return Ok();
        }
    }
}