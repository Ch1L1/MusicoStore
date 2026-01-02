namespace WebMVC.Models.Products;

public class ProductListViewModel
{
    public IEnumerable<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
}
