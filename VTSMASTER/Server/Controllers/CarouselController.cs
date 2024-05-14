using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VTSMASTER.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CarouselController : ControllerBase
	{
		private readonly ICarousel _carousel;

		public CarouselController(ICarousel carousel)
		{
			_carousel = carousel;
		}

		[HttpGet]
		public async Task<ActionResult<ServiceResponse<List<HomeCarousel>>>> GetCarousels()
		{
			var result = await _carousel.GetImagesAsync();
			return Ok(result);
		}
	}
}
