using PhoneNumberDetectionDemo.Common;
using PhoneNumberDetectionDemo.Models;
using PhoneNumberDetectionDemo.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;


namespace PhoneNumberDetectionDemo.Controllers
{
    public class PhoneNumberDetectionController : Controller
    {
        // GET: PhoneNumberDetection
        public ActionResult Index()
        {
            return View(new TextInputModel());
        }

        [HttpPost]
        public ActionResult ProcessInput(TextInputModel model)
        {
            #region Declaration

            if ((model.File != null && model.File.ContentLength > 0) || !string.IsNullOrWhiteSpace(model.Text))
            {

                List<DetectedPhoneNumberViewModel> detectedPhoneNumbers = new List<DetectedPhoneNumberViewModel>();

                #endregion

                string inputText = PhoneNumberUtility.FetchData(model);

                detectedPhoneNumbers = PhoneNumberUtility.FormatPhoneNumber(inputText);

                if (detectedPhoneNumbers.Count == 0)
                {
                    // No phone numbers were detected
                    ViewBag.Message = "No phone numbers were detected in the input text.";
                    return View("Index");
                }

                // Prepare the view model to display the results
                var viewModel = new PhoneNumberViewModel
                {
                    InputText = inputText,
                    DetectedPhoneNumbers = detectedPhoneNumbers,
                    Message = ""
                };

                return View("Results", viewModel);
            }
            else
            {
                ViewBag.Message = "Please enter atleast one phone number.";
                return View("Index", model);
            }

        }
    }
}