namespace Online_Store.Models
{
    public class ViewProductsWithNotificationFlag
    {
        public IEnumerable<KeyValuePair<int, ViewProduct>> Pairs { get; set; }

        public bool HasUnreadNotification {  get; set; }
    }
}
