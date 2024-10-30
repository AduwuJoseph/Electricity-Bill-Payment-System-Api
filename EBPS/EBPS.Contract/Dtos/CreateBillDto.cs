using EBPS.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBPS.Contract.Dtos
{
    public class CreateBillDto
    {
            [Range(0, 900000000000, MinimumIsExclusive = false, MaximumIsExclusive = false)]
            public decimal Amount { get; set; }
            [AllowedValues("A", "B")]
            [Required]
            public string Providers { get; set; }
    }
}
