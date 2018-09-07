using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using PropertyChanged;

namespace XDemo.UI.Models.Validations.Base
{
    [AddINotifyPropertyChangedInterface]
    public class ValidatableObject<T> : BindableObject, IValidity
    {
        public List<IValidationRule<T>> Validations { get; }

        public List<string> Errors { get; set; }

        public T Value { get; set; }

        public bool IsValid { get; private set; }

        public ValidatableObject()
        {
            IsValid = true;
            Errors = new List<string>();
            Validations = new List<IValidationRule<T>>();
        }

        public bool Validate()
        {
            Errors.Clear();

            var errors = Validations.Where(v => !v.Check(Value))
                .Select(v => v.ValidationMessage);

            Errors = errors.ToList();
            IsValid = !Errors.Any();

            return this.IsValid;
        }
    }
}
