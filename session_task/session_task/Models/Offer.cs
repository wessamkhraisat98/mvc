
namespace session_task.Models
{
    public class Offer
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int SupplierId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
