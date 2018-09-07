using System.Text.RegularExpressions;
using XDemo.UI.Models.Validations.Base;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace XDemo.UI.Models.Validations.DefinedRules
{
    /// <summary>
    /// [String] Email rule.
    /// </summary>
    public class EmailRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return false;

            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var match = regex.Match(str);
            return match.Success;
        }
    }
}
