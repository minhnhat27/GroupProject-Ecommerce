using GroupProject_Ecommerce.Models;

namespace GroupProject_Ecommerce.ViewModels
{
    public class PreviewMailModel
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public List<Product> Products { get; set; }
        public List<string> Receivers { get; set; }
    }
}
