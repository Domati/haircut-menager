using HaircutManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace HaircutManager.Controllers
{
    public class CaptchaController : Controller
    {
        [Route("get-captcha")]
        public IActionResult GetCaptcha()
        {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }
    }
}
