namespace Basket.Application.ViewModels
{
    public class BasketViewModel
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime Create { get; set; }
        public string UserId { get; set; }
        public List<ItemViewModel> Items { get; set; }
    }
}
