using EBPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBPS.Application.Interfaces
{
    public interface ISmsService
    {
        Task<object> SendSMS(string smsBody, string fromPhoneNumber, string toPhoneNumber);
        void SendPaymentSuccessNotification(Bill bill);
        void SendLowBalanceNotification(Wallet wallet, decimal amount);
    }
}
