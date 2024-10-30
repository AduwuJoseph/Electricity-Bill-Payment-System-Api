using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBPS.Domain.Entities
{
    public class Wallet
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public decimal Balance { get; set; }

        [EmailAddress]
        public string Email {  get; set; }
    }

}
