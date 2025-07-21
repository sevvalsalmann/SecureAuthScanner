using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TestProject.Controllers
{
    // Eksik Authorize - HATA! (Tarama aracı bunu bulmalı)
    [ApiController]
    [Route("[controller]")]
    public class NoAuthController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult PublicData() => Ok("Bu controller'da Authorize yok!");
    }

    // Doğru kullanılmış Authorize
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SecureController : ControllerBase
    {
        [HttpGet("secure")]
        public IActionResult SecureData() => Ok("Bu controller Authorize ile korumalı!");
    }

    // Boş Authorize - HATA! (Tarama aracı bunu bulmalı)
    [Authorize()]
    [ApiController]
    [Route("[controller]")]
    public class EmptyAuthorizeController : ControllerBase
    {
        [HttpGet("empty")]
        public IActionResult EmptyData() => Ok("Boş Authorize attribute!");
    }

    // Sadece AllowAnonymous olan metot - HATA! (Tarama aracı bunu bulmalı)
    [ApiController]
    [Route("[controller]")]
    public class AnonController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("anon")]
        public IActionResult AnonData() => Ok("Tamamen açık endpoint!");
    }

    // Yalnızca metot düzeyinde Authorize - Doğru!
    [ApiController]
    [Route("[controller]")]
    public class MixedController : ControllerBase
    {
        [Authorize]
        [HttpGet("user")]
        public IActionResult UserData() => Ok("Metot düzeyinde Authorize var!");

        [AllowAnonymous]
        [HttpGet("open")]
        public IActionResult OpenData() => Ok("Açık metot!");
    }
}
