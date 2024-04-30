using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhoneNumberDetectionDemo.ViewModel
{
    public class PhoneNumberViewModel
    {
        public string InputText { get; set; }
        public List<DetectedPhoneNumberViewModel> DetectedPhoneNumbers { get; set; }
        public string Message { get; set; }
    }
}