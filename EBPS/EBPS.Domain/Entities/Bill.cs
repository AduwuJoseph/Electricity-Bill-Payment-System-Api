using EBPS.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBPS.Domain.Entities
{
    public class Bill
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Range(0, 900000000000, MinimumIsExclusive = false, MaximumIsExclusive = false)]
        public decimal Amount { get; set; }
        [AllowedValues("A", "B")]
        [Required]
        public string Providers { get; set; }   
        public string Status { get; set; } = BillStatus.Pending.ToString(); // Pending or Paid
    }

}
