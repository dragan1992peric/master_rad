namespace VTSMASTER.Client.Services.CarouselService
{
	public interface ICarouselService
	{
        List<HomeCarousel> Carousels { get; set; }
        Task<List<HomeCarousel>> GetCarousels();
	}
}
