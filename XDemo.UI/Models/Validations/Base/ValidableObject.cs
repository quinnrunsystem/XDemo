using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using PropertyChanged;

namespace XDemo.UI.Models.Validations.Base
{
    [AddINotifyPropertyChangedInterface]
    public class ValidableObject<T> : BindableObject, IValidity
    {
        public ValidableObject()
        {
            IsValid = true;
            Errors = new List<string>();
            Rules = new List<IValidationRule<T>>();
        }

        /// <summary>
        /// Gets all rules of this instance.
        /// </summary>
        /// <value>The rules.</value>
        public List<IValidationRule<T>> Rules { get; }

        /// <summary>
        /// Get all errors message of this instance (from last validations)
        /// </summary>
        /// <value>The errors.</value>
        public List<string> Errors { get; private set; }

        /// <summary>
        /// Main value of your wrapped property
        /// </summary>
        /// <value>The value.</value>
        public T Value { get; set; }

        public bool IsValid { get; private set; }

        /// <summary>
        /// Validate this instance.
        /// </summary>
        /// <returns>The validate.</returns>
        public bool Validate()
        {
            Errors = Rules.Where(v => !v.Check(Value))
                .Select(v => v.ValidationMessage).ToList();
            IsValid = !Errors.Any();
            return this.IsValid;
        }
    }
}
