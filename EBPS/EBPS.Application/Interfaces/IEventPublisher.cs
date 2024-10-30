using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBPS.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishEvent(string eventType, object data);
        Task<bool> SubscribeEvent(string eventType, object data);
    }

}
