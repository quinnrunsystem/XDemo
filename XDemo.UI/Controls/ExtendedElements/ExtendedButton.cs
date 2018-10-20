using System;
using Xamarin.Forms;
namespace XDemo.UI.Controls.ExtendedElements
{
    public class ExtendedButton : Button
    {
        #region GradientColors property

        public static readonly BindableProperty GradientColorsProperty = BindableProperty.Create(nameof(GradientColors), typeof(Color[]), typeof(ExtendedButton), default(Color[]));
        /// <summary>
        /// Gets or sets the gradient colors. <para/>
        /// This colors will replace the old background color
        /// </summary>
        /// <value>The gradient colors.</value>
        public Color[] GradientColors
        {
            get => (Color[])GetValue(GradientColorsProperty);
            set => SetValue(GradientColorsProperty, value);
        }

        #endregion

        #region GradientFlow property

        public static readonly BindableProperty GradientFlowProperty = BindableProperty.Create(nameof(GradientFlow), typeof(Flows), typeof(ExtendedButton), default(Flows));

        public Flows GradientFlow
        {
            get => (Flows)GetValue(GradientFlowProperty);
            set => SetValue(GradientFlowProperty, value);
        }

        #endregion

        public enum Flows
        {
            Horizontal,
            Vertical
        }
    }
}
