using System.ComponentModel.DataAnnotations.Schema;

namespace GraphicsCardProject.Helpers
{
    public class OrderDetailsHelper
    {
        public int OrderId { get; set; }

        public int ProductItemId { get; set; }

        public int CustomerId { get; set; }

        public string? DateCreated { get; set; }

        public int QtyO { get; set; }

        public int QtyS { get; set; }

        public int QtyB { get; set; }

        public string? ProductName { get; set; }

        [Column(TypeName = "money")]
        public decimal? Tax { get; set; }

        [Column(TypeName = "money")]
        public decimal? AfterTax { get; set; }

        [Column(TypeName = "money")]
        public decimal? TotalCost { get; set; }

        [Column(TypeName = "money")]
        public decimal ProductCost { get; set; }
    }
}
