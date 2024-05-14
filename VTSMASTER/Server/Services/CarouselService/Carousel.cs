namespace VTSMASTER.Server.Services.CarouselService
{
	public class Carousel : ICarousel
	{
		private readonly DataContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public Carousel(DataContext context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}
		public async Task<ServiceResponse<List<HomeCarousel>>> GetImagesAsync()
		{
			

			var karoseli = await _context.HomeCarousels.ToListAsync();
			return new ServiceResponse<List<HomeCarousel>> { Data = karoseli };
		}
	}
}
