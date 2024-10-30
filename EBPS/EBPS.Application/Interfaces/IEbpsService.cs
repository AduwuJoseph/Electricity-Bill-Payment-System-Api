using EBPS.Contract.Dtos;
using EBPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBPS.Application.Interfaces
{
    public interface IEbpsService
    {
        Task<Wallet> FundWallet(string email, decimal amount);
        Task<Bill> CreateBill(CreateBillDto m);
        Task<Wallet> GetWallet(string Id);
        Task<bool> ProcessPayment(string validationRef);
    }
}
