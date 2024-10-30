using Amazon.SimpleNotificationService;
using Amazon.SQS;
using EBPS.Application.Interfaces;
using EBPS.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBPS.Api.EventHelpers
{
    public class EventPublisher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;
        public EventPublisher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _session = httpContextAccessor.HttpContext.Session;
        }
        public void PublishEvent(string eventType, object data)
        {
            
            var message = JsonConvert.SerializeObject(data);
            // Using session to implement the event publishing
            _session.SetString(eventType, message);
        }

        public string? SubscribeEvent(string eventType)=> _session.GetString(eventType);
            
    }
}
