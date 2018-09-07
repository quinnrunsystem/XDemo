using XDemo.UI.Models.Validations.Base;
using System;
using XDemo.Core.Infrastructure.Logging;

namespace XDemo.UI.Models.Validations.DefinedRules
{
    /// <summary>
    /// Minimum value rule.
    /// </summary>
    public class MinValueRule<T> : IValidationRule<T> where T : IComparable
    {
        private readonly T _minValue;
        public MinValueRule(T minValue)
        {
            _minValue = minValue;
        }

        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            try
            {
                var compared = value.CompareTo(_minValue);
                return compared >= 0;
            }
            catch (Exception ex)
            {
                /* ==================================================================================================
                 * in case of any exception caught => the rule failed
                 * ================================================================================================*/
                LogCommon.Error(ex);
                return false;
            }
        }
    }
}
