using VTSMASTER.Client.Pages.Admin;

namespace VTSMASTER.Client.Services.CarouselService
{
	public class CarouselService : ICarouselService
	{
		private readonly HttpClient _http;
		public List<HomeCarousel> Carousels { get; set; } = new List<HomeCarousel>();
        public CarouselService(HttpClient http)
		{
			_http = http;
		}
		public async Task<List<HomeCarousel>> GetCarousels()
		{
			var response = await _http.GetFromJsonAsync<ServiceResponse<List<HomeCarousel>>>("api/carousel");
			if (response != null && response.Data != null)
                Carousels = response.Data;
			return Carousels;
        }
	}
}
