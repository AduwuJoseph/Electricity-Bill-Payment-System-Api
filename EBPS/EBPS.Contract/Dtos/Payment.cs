using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBPS.Contract.Dtos
{
    public class PaymentDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string BillId { get; set; }

        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
