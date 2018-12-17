using System;
using Xamarin.Forms;
using System.Linq;

namespace XDemo.UI.Controls.GroupedElements
{
    public class AnimatedText : StackLayout
    {
        private Animation _animation;
        public const string AnimationName = "AnimatedTextAnimation";

        public AnimatedText()
        {
            Orientation = StackOrientation.Horizontal;
            Spacing = -1;
        }

        #region IsRunning property

        public static readonly BindableProperty IsRunningProperty = BindableProperty.Create(nameof(IsRunning), typeof(bool), typeof(AnimatedText), default(bool), BindingMode.TwoWay);

        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        #endregion

        #region Text property

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(AnimatedText), default(string), BindingMode.TwoWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion

        #region FontAttributes property

        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(AnimatedText), default(FontAttributes), BindingMode.TwoWay);

        public FontAttributes FontAttributes
        {
            get => (FontAttributes)GetValue(FontAttributesProperty);
            set => SetValue(FontAttributesProperty, value);
        }

        #endregion

        #region FontSize property

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(AnimatedText), 13d, BindingMode.TwoWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        #endregion

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(IsRunning) when IsEnabled:
                    if (IsRunning)
                        StartAnimation();
                    else
                        StopAnimation();
                    break;
                case nameof(Text):
                    InitAnimation();
                    break;
                case nameof(IsEnabled):
                    StopAnimation();
                    break;
                case nameof(FontAttributes):
                case nameof(FontSize):
                    if (Children == null || !Children.Any())
                        break;
                    foreach (var child in Children)
                        if (child is Label label)
                        {
                            label.FontSize = FontSize;
                            label.FontAttributes = FontAttributes;
                        }
                    break;
            }
        }

        private void InitAnimation()
        {
            _animation = new Animation();
            Children?.Clear();
            if (string.IsNullOrWhiteSpace(Text))
                return;

            var index = 0;
            foreach (var textChar in Text)
            {
                var label = new Label
                {
                    Text = textChar.ToString(),
                    FontAttributes = FontAttributes,
                    FontSize = FontSize
                };

                Children.Add(label);

                var oneCharAnimationLength = (double)1 / (Text.Length + 1);
                // animation for Scale property
                _animation.Add(index * oneCharAnimationLength, (index + 1) * oneCharAnimationLength,
                    new Animation(v => label.Scale = v, 1, 1.75, Easing.Linear));
                _animation.Add((index + 1) * oneCharAnimationLength, (index + 2) * oneCharAnimationLength,
                    new Animation(v => label.Scale = v, 1.75, 1, Easing.Linear));

                // animation for TranslationY property
                _animation.Add(index * oneCharAnimationLength, (index + 1) * oneCharAnimationLength,
                    new Animation(v => label.TranslationY = v, 0, -10, Easing.Linear));
                _animation.Add((index + 1) * oneCharAnimationLength, (index + 2) * oneCharAnimationLength,
                    new Animation(v => label.TranslationY = v, -10, 0, Easing.Linear));

                index++;
            }
        }

        private void StartAnimation()
        {
            _animation.Commit(this, AnimationName, 16,
                (uint)Children.Count * 150,
                Easing.Linear,
                null, () => true);
        }

        private void StopAnimation()
        {
            this.AbortAnimation(AnimationName);
        }
    }
}
