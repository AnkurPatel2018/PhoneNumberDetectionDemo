using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhoneNumberDetectionDemo.ViewModel
{
    public class DetectedPhoneNumberViewModel
    {
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Valid { get; set; }
    }
}