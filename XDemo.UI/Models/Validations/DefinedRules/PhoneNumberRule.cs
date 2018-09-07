
using System.Text.RegularExpressions;
using XDemo.UI.Models.Validations.Base;

namespace XDemo.UI.Models.Validations.DefinedRules
{
    /// <summary>
    /// [String] Phone number rule.
    /// </summary>
    public class PhoneNumberRule<T> : IValidationRule<T>
    {
        private readonly string _nationCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XDemo.UI.Models.Validations.DefinedRules.PhoneNumberRule`1"/> class. <para/>
        /// "84": default phone area code of VietNam
        /// </summary>
        /// <param name="nationPhoneAreaCode">Nation phone area code. Default is VietNam (+84)</param>
        public PhoneNumberRule(string nationPhoneAreaCode = "84")
        {
            _nationCode = nationPhoneAreaCode;
        }
        public string ValidationMessage { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public bool Check(T value)
        {
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return false;

            var regex = new Regex(@"^([\+]?" + _nationCode + "[-]?|[0])?[1-9][0-9]{8}$");
            var match = regex.Match(str);
            return match.Success;
        }
    }
}
