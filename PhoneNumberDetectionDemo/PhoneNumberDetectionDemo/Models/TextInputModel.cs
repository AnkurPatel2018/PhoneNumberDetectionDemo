using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhoneNumberDetectionDemo.Models
{
    public class TextInputModel
    {
        public enum InputSource
        {
            DirectInput,
            TextFile
            
        }

        public InputSource SelectedSource { get; set; }

        [Required(ErrorMessage = "Please enter the input text")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Please enter at least 1 character")]
        public string Text { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}