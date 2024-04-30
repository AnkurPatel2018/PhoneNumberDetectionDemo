using PhoneNumberDetectionDemo.Models;
using PhoneNumberDetectionDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhoneNumberDetectionDemo.Common
{
    public static class PhoneNumberUtility
    {
        // Read phone number from file or read from textbox
        public static string FetchData(TextInputModel model)
        {
            string inputText = "";

            switch (model.SelectedSource)
            {
                case TextInputModel.InputSource.TextFile:

                    if (model.File != null && model.File.ContentLength > 0)
                    {
                        // Read the file content and store it in inputText
                        using (var reader = new StreamReader(model.File.InputStream))
                        {
                            inputText = reader.ReadToEnd();
                        }
                    }
                    
                    break;
                case TextInputModel.InputSource.DirectInput:
                    inputText = model.Text;
                    break;
            }

            return inputText;
        }

        //This method takes a string containing one or more phone numbers separated by commas as input. It iterates over each phone number, formats it, 
        //and returns a list of DetectedPhoneNumberViewModel objects containing the formatted phone numbers.
        public static List<DetectedPhoneNumberViewModel> FormatPhoneNumber(string phoneNumbers)
        {
            List<DetectedPhoneNumberViewModel> detectedPhoneNumbers = new List<DetectedPhoneNumberViewModel>();

            foreach (string phoneNumber in phoneNumbers.Split(','))
            {
                DetectedPhoneNumberViewModel detectedPhoneNumber = new DetectedPhoneNumberViewModel();

                string strNo = ConcatenateDigits(phoneNumber);

                string PhoneNo = phoneNumber;

                if(string.IsNullOrWhiteSpace(strNo))
                {
                    PhoneNo = ConvertWordToNumber(phoneNumber);
                }

                string countryCode = ExtractCountryCode(PhoneNo.Replace("\"", "").TrimStart().TrimEnd());
                string formattedNumber = ExtractPhoneNumber(PhoneNo.Replace("\"", "").TrimStart().TrimEnd(), countryCode);

                if (!string.IsNullOrEmpty(countryCode))
                {
                    detectedPhoneNumber.CountryCode = countryCode;
                    detectedPhoneNumber.PhoneNumber = formattedNumber;
                }
                else
                {
                    detectedPhoneNumber.PhoneNumber = formattedNumber;
                }

                detectedPhoneNumbers.Add(detectedPhoneNumber);
            }
            return detectedPhoneNumbers;
        }

        //This method extracts all the digits from a given input string and concatenates them into a single string. It uses a regular expression to match and extract digits from the input string.
        public static string ConcatenateDigits(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"\d+");
            string concatenatedDigits = "";

            foreach (Match match in matches)
            {
                concatenatedDigits += match.Value;
            }

            return concatenatedDigits;
        }

        //This method extracts the country code from a phone number string. It first checks if the phone number contains more than 10 digits. If it does, it attempts to extract the country code using regular expressions based on various patterns.
        public static string ExtractCountryCode(string phoneNumber)
        {
            string str = ConcatenateDigits(phoneNumber);
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (str.Length > 10 && ((str.Length - 10) > 1 || (str.Length - 10) == 1 && str.First() != '0'))
                {
                    Match match = Regex.Match(phoneNumber, @"\+(\d{1," + Convert.ToString(str.Length - 10) + "})-?");
                    if (match.Success)
                    {
                        return "+" + match.Groups[1].Value;
                    }

                    string countryCodeRegex = @"\((\d{1," + (phoneNumber.Length - 10) + "})\\)";
                    match = Regex.Match(phoneNumber, countryCodeRegex);

                    if (match.Success)
                    {
                        return "(" + match.Groups[1].Value + ")";
                    }

                    match = Regex.Match(phoneNumber, @"(\d{1," + Convert.ToString(str.Length - 10) + "})-?");
                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }

                }
            }
            return "";
        }

        //This method removes a dynamic string from the beginning of the input string if it matches the specified dynamic string. It checks if the input string starts with the dynamic string and removes it along with any leading spaces or hyphens.
        public static string RemoveDynamicString(string input, string dynamicString)
        {
            if (input.StartsWith(dynamicString))
            {
                string phoneNo = input.Substring(dynamicString.Length);

                if (phoneNo.StartsWith("-") || phoneNo.StartsWith(" "))
                {
                    return phoneNo.Substring(1);
                }
                else
                {
                    return phoneNo;
                }
            }
            else
            {
                return input;
            }
        }

        //This method extracts the phone number from the input string, excluding the country code if present. If a country code is present, it uses the RemoveDynamicString method to remove it. 
        //If no country code is present and the extracted digits form a 10-digit number, it considers it a valid phone number; otherwise, it appends a validation message to indicate that the input is not a valid phone number.
        public static string ExtractPhoneNumber(string phoneNumber, string countryCode)
        {
            string[] separators = { countryCode };

            if (!string.IsNullOrWhiteSpace(countryCode))
            {
                return RemoveDynamicString(phoneNumber, countryCode);
            }
            else
            {
                string strNo = ConcatenateDigits(phoneNumber);

                if (strNo.Length == 10)
                {
                    return phoneNumber;
                }
                else
                {
                    return phoneNumber + " is not a valid phone number";
                }
            }

        }

        //This method converts Hindi/English number words to numerical digits in the input phone number string. 
        //It uses a regular expression to find matches for Hindi/English number words and replaces them with their numerical equivalent using a dictionary mapping.
        public static string ConvertWordToNumber(string phoneNumber)
        {
            string hindiEnglishPattern = @"(?:एक|ONE) (?:दो|TWO) (?:तीन|THREE) (?:चार|FOUR) (?:पांच|FIVE) (?:छह|SIX) (?:सात|SEVEN) (?:आठ|EIGHT) (?:नौ|NINE) (?:शूɊ|ZERO)";

            // Map Hindi/English number words to their numerical equivalent
            Dictionary<string, string> numberWordMap = new Dictionary<string, string>
            {
                {"एक", "1"},
                {"ONE", "1"},
                {"दो", "2"},
                {"TWO", "2"},
                {"तीन", "3"},
                {"THREE", "3"},
                {"चार", "4"},
                {"FOUR", "4"},
                {"पांच", "5"},
                {"FIVE", "5"},
                {"छह", "6"},
                {"SIX", "6"},
                {"सात", "7"},
                {"SEVEN", "7"},
                {"आठ", "8"},
                {"EIGHT", "8"},
                {"नौ", "9"},
                {"NINE", "9"},
                {"शूɊ", "0"},
                {"ZERO", "0"}
            };

            // Replace Hindi/English number words with numerical digits
            MatchCollection matches = Regex.Matches(phoneNumber, hindiEnglishPattern);

            phoneNumber = string.Empty;

            foreach (Match match in matches)
            {
                string[] wholeWord = match.Value.Split(' ');

                foreach (string str in wholeWord)
                {
                    if (numberWordMap.ContainsKey(str))
                    {
                        phoneNumber += numberWordMap[str];
                    }
                }
            }

            return phoneNumber;
        }

    }
}