namespace Online_Store.Models
{
    public class ViewOrderGroups
    {
        public string Username { get; set; }
        public DateOnly DateOfPurchase { get; set; }
        public List<ViewOrder> Orders { get; set; }
    }

    public class ViewOrder
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double ProductPrice { get; set; }
    }
}
