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
        public bool CheckIfExists { get; set; }  // KFreon: If True, checks if entered path actually exists
        public bool isNumbers { get; set; }  // KFreon: True = not a path, but numbers

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (String.IsNullOrEmpty(value as string))
                return new ValidationResult(false, "Value can't be empty.");
            else
            {
                string val = (string)value;

                if (isNumbers)
                {
                    // KFreon: Should be a number so trys to parse number
                    int num = -1;
                    if (!Int32.TryParse(val, out num))
                        return new ValidationResult(false, "Not a valid number");
                }
                else
                {
                    if (CheckIfExists)
                    {
                        // KFreon: Check if path exists
                        if (val.isFile() && !File.Exists(val))
                            return new ValidationResult(false, "Specified file doesn't exist!");
                        else
                            if (!Directory.Exists(val))
                                return new ValidationResult(false, "Specified directory doesn't exist!");
                    }
                    else
                    {
                        // KFreon: Valid path = letter:\
                        if (val.Length < 3 || !(val[0].isLetter() && val[1] == ':' && val[2] == '\\'))
                            return new ValidationResult(false, "Path is in invalid format.");
                    }
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}
