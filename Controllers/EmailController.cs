using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;

namespace EmailSenderApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _emailService = emailService;
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpPost]
        public IActionResult SendEmail(EmailDto request)
        {
            if (!ReCaptchaPassed(request.CaptchaToken))
            {
                return BadRequest()
             ;
            }
            _emailService.SendEmail(request);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool ReCaptchaPassed(string gRecaptchaResponse)
        {
            try
            {
                var secret = _config["SecretKey"];
                HttpClient httpClient = _httpClientFactory.CreateClient();
                var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={gRecaptchaResponse}").Result;
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
                string JSONres = res.Content.ReadAsStringAsync().Result;
                dynamic JSONdata = JObject.Parse(JSONres);
                if (JSONdata.success != "true")
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}