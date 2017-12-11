using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UsefulThings.WPF.ValidationRules
{
    public abstract class ValidationRuleBase : ValidationRule
    {
        // Denotes whether rule is active or not (may not be active when parent is disabled, etc)
        public bool IsActive { get; set; }
        protected ValidationRuleBase()
        {
            IsActive = true;
        }

        protected abstract ValidationResult DoValidate(object value, System.Globalization.CultureInfo cultureInfo);
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Return 'all good' if disabled to simulate inactivity.
            if (!IsActive)
                return ValidationResult.ValidResult;

            return DoValidate(value, cultureInfo);
        }
    }
}
