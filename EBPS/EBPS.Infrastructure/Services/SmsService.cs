using EBPS.Application.Interfaces;
using EBPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBPS.Infrastructure.Services
{
    public class SmsService : ISmsService
    {

        public void SendLowBalanceNotification(Wallet wallet, decimal amount)
        {
            Console.WriteLine($"Notification sent for insufficient balance to process payment for {amount.ToString("N2")}");
        }

        public void SendPaymentSuccessNotification(Bill bill)
        {
            Console.WriteLine($" process payment for {bill.Amount.ToString("N2")} was successful");
        }
    }
}
