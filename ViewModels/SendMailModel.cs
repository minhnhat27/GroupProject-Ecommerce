namespace GroupProject_Ecommerce.ViewModels
{
    public class SendMailModel
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public List<int> Products { get; set; }
        public List<string> Receivers { get; set; }
    }
}
