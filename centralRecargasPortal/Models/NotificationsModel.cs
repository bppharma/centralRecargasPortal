using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace centralRecargasPortal.Models
{
    public class NotificationsModel
    {
        public class SellNotification
        {
            public string PhoneNumber { get; set; }
            public string MessageToSend { get; set; }
            public string EmailSend { get; set; }
        }
    }
}
