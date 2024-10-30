using EBPS.Api.EventHelpers;
using EBPS.Application.Interfaces;
using EBPS.Contract.Dtos;
using EBPS.Contract.Enums;
using EBPS.Contract.Helpers;
using EBPS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;

namespace EBPS.Api.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class EbpsController : ControllerBase
    {
        private readonly ILogger<EbpsController> _logger;
        private readonly ISmsService _smsService;
        private readonly IEbpsService _ebpsService;
        private readonly EventPublisher _eventPublisher;
        IHttpContextAccessor _contextAccessor;
        public EbpsController(ILogger<EbpsController> logger, ISmsService smsService, IEbpsService ebpsService, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _smsService = smsService;
            _ebpsService = ebpsService;
            _contextAccessor = contextAccessor;
            _eventPublisher = new EventPublisher(_contextAccessor);
        }
        [HttpPost("electricity/verify")]
        public async Task<IActionResult> CreateBill([FromBody] CreateBillDto model)
        {
            var bills = new List<Bill>();
            if (ModelState.IsValid)
            {
                var bs = _eventPublisher.SubscribeEvent(EventDefinitions.bill_created);
                if (bs != null)
                    bills = JsonConvert.DeserializeObject<List<Bill>>(bs);
                var chk = bills.FirstOrDefault(x => x.Providers == model.Providers && x.Status == BillStatus.Pending.ToString());
                if (chk != null)
                {
                    bills.Remove(chk);
                    chk.Amount = model.Amount;
                    bills.Add(chk);
                }
                else
                {
                    bills.Add(await _ebpsService.CreateBill(model));
                }
                // Publish the bill_created event
                _eventPublisher.PublishEvent(EventDefinitions.bill_created, bills);
                return Ok(new { status = true, data = bills, message = $"Electricity bill successfully created for {model.Providers}" });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpPost("Vend/{validationRef}/pay")]
        public IActionResult ProcessPayment(string validationRef, [FromForm] string provider)
        {
            if(string.IsNullOrEmpty(provider)) return BadRequest("Invalid provider value");
            // Retrieve bill by validationRef and wallet by associated ID
            Wallet? wallet = null;
            var wallets = new List<Wallet>();
            var wa = _eventPublisher.SubscribeEvent(EventDefinitions.add_funds);
            if (wa != null) {
                 wallets = JsonConvert.DeserializeObject<List<Wallet>>(wa);

                wallet = wallets.FirstOrDefault(x => x.Email == validationRef);
            }

            Bill? bill = null;
            var bills = new List<Bill>();
            var bs = _eventPublisher.SubscribeEvent(EventDefinitions.bill_created);
            if (bs != null) {
                bills = JsonConvert.DeserializeObject<List<Bill>>(bs);
                bill = bills.FirstOrDefault(x => x.Providers == provider && x.Status == BillStatus.Pending.ToString());
            }

            if (bill != null && wallet != null) {
                if (wallet.Balance >= bill.Amount)
                {
                    bills.Remove(bill);
                    wallets.Remove(wallet);
                    wallet.Balance -= bill.Amount;
                    bill.Status = BillStatus.Paid.ToString();

                    bills.Add(bill);
                    wallets.Add(wallet);

                    var payments = new List<PaymentDto>();
                    var ps = _eventPublisher.SubscribeEvent(EventDefinitions.payment_completed);
                    if (!string.IsNullOrEmpty(ps))
                    {
                        payments = JsonConvert.DeserializeObject<List<PaymentDto>>(ps);
                    }
                    payments.Add(new PaymentDto { 
                        Amount = bill.Amount,
                        BillId = bill.Id,
                        Email = validationRef,
                        Status = BillStatus.Paid.ToString(),
                    });
                           
                    
                    // Publish event for successful payment processing
                    _eventPublisher.PublishEvent(EventDefinitions.payment_completed, payments);

                    // Update wallet and bill status
                    _eventPublisher.PublishEvent(EventDefinitions.bill_created, bills);
                    _eventPublisher.PublishEvent(EventDefinitions.add_funds, wallets);

                    // Send SMS notification for successful payment
                    _smsService.SendPaymentSuccessNotification(bill);

                    return Ok(new { message = "Payment successful" });
                }
                else
                {
                    _smsService.SendLowBalanceNotification(wallet, bill.Amount);
                    return BadRequest(new { message = "Insufficient wallet balance" });
                }
            }
            return BadRequest("You have no data to process payment");
        }
        [HttpPost("wallets/{email}/add-funds")]
        public async Task<IActionResult> AddFunds(string email, [FromForm] decimal amount)
        {
            if (amount <= 0) return BadRequest("Invalid amount passed: amount must be grater than 0");
            var wallets = new List<Wallet>();
            // Retrieve wallet by ID and update balance
            var bs = _eventPublisher.SubscribeEvent(EventDefinitions.add_funds);
            if (bs != null)
                wallets = JsonConvert.DeserializeObject<List<Wallet>>(bs);
            var chk = wallets.Where(x => x.Email == email).FirstOrDefault();
            if (chk != null)
            {
                wallets.Remove(chk);
                chk.Balance += amount;
                wallets.Add(chk);
                _eventPublisher.PublishEvent(EventDefinitions.add_funds, wallets);
                return Ok(new { status = true, data = wallets, message = $"Wallet successfully updated for {email}" });
            }
            else
            {
                chk = await _ebpsService.FundWallet(email,amount);
                wallets.Add(chk);
                _eventPublisher.PublishEvent(EventDefinitions.add_funds, wallets);
                return Ok(new { status = true, data = wallets, message = $"Wallet successfully created for {email}" });
            }
        }
    }
}
