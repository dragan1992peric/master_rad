using VTSMASTER.Shared;

namespace VTSMASTER.Client.Services.ProductService
{
    public interface IProductService
    {
        List<Product> Products { get; set; }
        Task GetProducts();
    }
}
