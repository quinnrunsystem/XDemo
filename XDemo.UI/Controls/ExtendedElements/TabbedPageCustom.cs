using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace XDemo.UI.Controls.ExtendedElements
{
    public class TabbedPageCustom : TabbedPage
    {
        /// <summary>
        /// Gets or sets the text color selected.
        /// </summary>
        /// <value>The text color selected.</value>
        public Color TextColorSelected
        {
            get { return (Color)GetValue(TextColorSelectedProperty); }
            set { SetValue(TextColorSelectedProperty, value); }
        }
        public static readonly BindableProperty TextColorSelectedProperty =
            BindableProperty.Create(
                nameof(TextColorSelected),
                typeof(Color),
                typeof(TabbedPageCustom),
                default(Color));

        /// <summary>
        /// Gets or sets the text color unselected.
        /// </summary>
        /// <value>The text color unselected.</value>
        public Color TextColorUnselected
        {
            get { return (Color)GetValue(TextColorUnselectedProperty); }
            set { SetValue(TextColorUnselectedProperty, value); }
        }
        public static readonly BindableProperty TextColorUnselectedProperty =
            BindableProperty.Create(
                nameof(TextColorUnselected),
                typeof(Color),
                typeof(TabbedPageCustom),
                default(Color));
    }
}
