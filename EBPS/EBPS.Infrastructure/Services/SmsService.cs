using EBPS.Application.Interfaces;
using EBPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace EBPS.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");

        string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");


        public SmsService()
        {
            TwilioClient.Init(accountSid, authToken);
            
        }


        public async Task<object> SendSMS(string smsBody, string fromPhoneNumber, string toPhoneNumber)
        {
            var message = await MessageResource.CreateAsync(

                body: smsBody,

                from: new Twilio.Types.PhoneNumber(fromPhoneNumber),

                to: new Twilio.Types.PhoneNumber(toPhoneNumber));
            return message;
        }

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
