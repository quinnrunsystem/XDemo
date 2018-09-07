using XDemo.UI.Models.Validations.Base;
using System;
using XDemo.Core.Infrastructure.Logging;

namespace XDemo.UI.Models.Validations.DefinedRules
{
    /// <summary>
    /// Max value rule.
    /// </summary>
    public class MaxValueRule<T> : IValidationRule<T> where T : IComparable
    {
        private readonly T _maxValue;
        public MaxValueRule(T maxValue)
        {
            _maxValue = maxValue;
        }

        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            try
            {
                var compared = value.CompareTo(_maxValue);
                return compared < 0;
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
