using System.Collections;
using XDemo.Core.Shared;
using XDemo.UI.Models.Validations.Base;

namespace XDemo.UI.Models.Validations.DefinedRules
{
    public class MinLengthRule<T> : IValidationRule<T>
    {
        private readonly int _minLength;

        public MinLengthRule(int minLength)
        {
            _minLength = minLength;
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
                return str != null && str.Length >= _minLength;
            }

            /* ==================================================================================================
             * for collection type values.
             * ================================================================================================*/
            if (TypeHelper.IsEnumerableType(inputType))
            {
                var enumerable = value as IEnumerable;
                return enumerable != null && TypeHelper.Count(enumerable) >= _minLength;
            }
            /* ==================================================================================================
             * other wise => false
             * ================================================================================================*/
            return false;
        }
    }

}
