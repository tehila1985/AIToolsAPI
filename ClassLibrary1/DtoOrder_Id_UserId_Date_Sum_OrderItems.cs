using Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace Dto
{

    public class DtoOrder_Id_UserId_Date_Sum_OrderItems
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalPrice { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<DtoOrderItem_Id_OrderId_ProductId_Quantity> OrderItems { get; set; } = new List<DtoOrderItem_Id_OrderId_ProductId_Quantity>();

    }
}
//public record DtoOrder_Id_UserId_Date_Sum_OrderItems(
//    int OrderId,
//    int UserId,
//    DateTime OrderDate,
//    string Status,
//    decimal TotalPrice,
//    ICollection<DtoOrderItem_Id_OrderId_ProductId_Quantity> OrderItems
//);

