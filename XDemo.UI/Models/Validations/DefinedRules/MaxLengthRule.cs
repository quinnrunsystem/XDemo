using System.Collections;
using XDemo.Core.Shared;
using XDemo.UI.Models.Validations.Base;

namespace XDemo.UI.Models.Validations.DefinedRules
{
    /// <summary>
    /// [String | Collection] Max length rule.
    /// </summary>
    public class MaxLengthRule<T> : IValidationRule<T>
    {
        private readonly int _maxLength = 0;
        public MaxLengthRule(int maxLength)
        {
            _maxLength = maxLength;
        }

        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            var inputType = typeof(T);
            /* ==================================================================================================
             * for string value. return directly
             * ================================================================================================*/
            if (inputType == typeof(string))
            {
                var str = value as string;
                return str != null && str.Length <= _maxLength;
            }

            /* ==================================================================================================
             * for collection type values.
             * ================================================================================================*/
            if (TypeHelper.IsEnumerableType(inputType))
            {
                var enumerable = value as IEnumerable;
                return enumerable != null && TypeHelper.Count(enumerable) <= _maxLength;
            }
            /* ==================================================================================================
             * other wise => false
             * ================================================================================================*/
            return false;
        }
    }
}
