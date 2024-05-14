

namespace VTSMASTER.Server.Services.CarouselService
{
	public interface ICarousel
	{
		Task<ServiceResponse<List<HomeCarousel>>> GetImagesAsync();
	}
}
