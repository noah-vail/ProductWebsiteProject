using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GraphicsCardProject.DAL.DomainClasses
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; } // Generates Foreign Key
        [Required]
        public int BrandId { get; set; }


        [ForeignKey("CategoryId")]
        public Category? Category { get; set; } // Generates Foreign Key
        [Required]
        public int CategoryId { get; set; }


        [Required]
        [MaxLength(100)]
        public string? ProductName { get; set; }

        [Required]
        [MaxLength(100)]
        public string? BrandName { get; set; }

        [Required]
        public string? RAM { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public double Price { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public double MSRP { get; set; }

        [Required]
        public int QtyOnHand { get; set; }

        [Required]
        public int QtyOnBackOrder { get; set; }

        [Required]
        public string? GraphicName { get; set; }

        /** ADDED COLUMNS **/
        public string? Memory {  get; set; }
        public string? Processor {  get; set; }
        public string? Graphics { get; set; }
        public string? Storage {  get; set; }
        /** ADDED COLUMNS **/


        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[]? Timer { get; set; }
    }
}
