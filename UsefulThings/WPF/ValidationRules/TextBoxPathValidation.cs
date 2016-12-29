using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UsefulThings.WPF.ValidationRules
{
    /// <summary>
    /// Provides IO Path validation 
    /// </summary>
    public class TextBoxPathValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string arg = value as string;
            if (String.IsNullOrEmpty(arg))
                return new ValidationResult(false, "Argument is null or empty.");

            if ((arg.isDirectory() && Directory.Exists(arg)) || (arg.isFile() && File.Exists(arg)))
            {
                foreach (var invalid in UsefulThings.General.InvalidPathingChars)
                    if (arg.Contains(invalid))
                        return new ValidationResult(false, $"Path contains invalid path char: {invalid}");

                return new ValidationResult(true, null);
            }

            return new ValidationResult(false, "File or folder doesn't exist.");
        }
    }
}
