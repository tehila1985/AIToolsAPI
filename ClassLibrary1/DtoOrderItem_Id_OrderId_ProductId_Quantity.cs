using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class DtoOrderItem_Id_OrderId_ProductId_Quantity
    {
        public int OrderItemId { get; set; }
        public  int OrderId { get; set; }
        public string ItemName { get; set; }
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }
}

//public record DtoOrderItem_Id_OrderId_ProductId_Quantity(
//    int OrderItemId,
//    int OrderId,
//    string ItemName,
//    int? ProductId,
//    int Quantity,
//    decimal PriceAtPurchase
//);