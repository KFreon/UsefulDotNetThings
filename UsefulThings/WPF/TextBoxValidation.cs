using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Deals with simple path validation.
    /// </summary>
    public class TextBoxValidation : ValidationRule
    {
        public bool RequireExistence { get; set; }  // KFreon: If True, checks if entered path actually exists
        public bool IsNumber { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (String.IsNullOrEmpty(value as string))
                return new ValidationResult(false, "Value can't be empty.");

                string val = (string)value;

            ValidationResult result = ValidationResult.ValidResult;

            if (IsNumber)
                {
                // KFreon: Should be a number so try to parse number, and check bounds
                    int num = -1;
                    if (!Int32.TryParse(val, out num))
                    result = ValidationResult(false, "Not a valid number");
                    
                if (Min != Max) // KFreon: One is set to something
                {
                    if (num > Max)
                        result = ValidationResult(false, "Must be smaller than " + Max);
                    else if (num < Min)
                        result = ValidationResult(false, "Must be larger than " + Min);
                }
                }
                else
                {
                if (val.Length < 3) 
                    return ValidationResult(false); just needs to be red. no message
                
                if (!Regex.IsMatch(val, "^([a-zA-Z]:|\e)\e"))
                    return ValidationResult(false, "Path should be <letter>:\"); 
                        
                if (RequireExistence)
                    {
                        // KFreon: Check if path exists
                        if (val.isFile() && !File.Exists(val))
                        result = ValidationResult(false, "Specified file doesn't exist!");
                    else if (!Directory.Exists(val))
                        result = ValidationResult(false, "Specified directory doesn't exist!");
                }
            }
            return result;
        }
    }
}
