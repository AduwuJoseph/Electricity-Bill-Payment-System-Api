using EBPS.Application.Interfaces;
using EBPS.Contract.Dtos;
using EBPS.Contract.Enums;
using EBPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBPS.Infrastructure.Services
{
    public class EbpsService : IEbpsService
    {
        public async Task<Bill> CreateBill(CreateBillDto m)
        {            
            return new Bill
            {
                Amount = m.Amount,
                Providers = m.Providers,
                Status = BillStatus.Pending.ToString(),
            };
        }

        public async Task<Wallet> FundWallet(string email, decimal amount)
        {
            return new Wallet
            {
                Email = email,
                Balance = amount,
            };
        }

        public Task<Wallet> GetWallet(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ProcessPayment(string validationRef)
        {
            throw new NotImplementedException();
        }
    }
}
