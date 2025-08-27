using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Models
{
    public class DeleteRequest
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int WorkerId { get; set; }
        public int? AdminId { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        public DateTime CreatedAt { get; set; }
        public Product Product { get; set; }
        public User Worker { get; set; }
    }
}
