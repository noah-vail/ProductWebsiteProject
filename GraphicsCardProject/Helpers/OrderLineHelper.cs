using GraphicsCardProject.DAL.DomainClasses;

namespace GraphicsCardProject.Helpers
{
    public class OrderLineHelper
    {
        public int Qty { get; set; }
        public Product? Item { get; set; }
    }
}
