using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraphicsCardProject.DAL.DomainClasses
{
    public class OrderLineItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        public int OrderId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        public int ProductId { get; set; }


        public int QtyOrdered { get; set; }
        public int QtySold { get; set; }
        public int QtyOnBackorder { get; set; }

        [Column(TypeName = "money")]
        public decimal SellingPrice { get; set; }
    }
}
