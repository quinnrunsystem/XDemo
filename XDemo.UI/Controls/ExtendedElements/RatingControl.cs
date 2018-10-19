using System.Collections.Generic;
using Xamarin.Forms;

namespace XDemo.UI.Controls.ExtendedElements
{
    /// <summary>
    /// Rating control. (no renderer)
    /// </summary>
    public class RatingControl : ContentView
    {
        List<Image> _starImages;

        public RatingControl()
        {
            this.WidthRequest = 150;
            this.HeightRequest = 20;
            this.HorizontalOptions = LayoutOptions.Start;
            var grid = new Grid()
            {
                ColumnSpacing = 4
            };

            // Create Star Image Placeholders 
            _starImages = new List<Image>();
            for (var i = 0; i < 5; i++)
            {
                _starImages.Add(new Image()
                {
                    WidthRequest = grid.WidthRequest / 5,
                    Aspect = Aspect.AspectFit
                });

                // Add image
                grid.Children.Add(_starImages[i], i, 0);
            }

            Content = grid;
        }

        #region Rating
        public static readonly BindableProperty RatingProperty =
          BindableProperty.Create(nameof(Rating),
              typeof(int),
              typeof(RatingControl),
              default(int),
              BindingMode.TwoWay,
              propertyChanged: UpdateStarsDisplay);

        public int Rating
        {
            get => (int)GetValue(RatingProperty);
            set => SetValue(RatingProperty, value);
        }
        #endregion

        #region Precision
        public static readonly BindableProperty PrecisionProperty =
          BindableProperty.Create(nameof(Precision),
              typeof(PrecisionType),
              typeof(RatingControl),
              PrecisionType.Half,
              propertyChanged: OnPrecisionPropertyChanged);

        public PrecisionType Precision
        {
            get => (PrecisionType)GetValue(PrecisionProperty);
            set => SetValue(PrecisionProperty, value);
        }

        static void OnPrecisionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = ((RatingControl)bindable);
            control.UpdateStarsDisplay();
        }
        #endregion
        #region FullStarImage property

        public static readonly BindableProperty FullStarImageProperty = BindableProperty.Create(nameof(FullStarImage), typeof(ImageSource), typeof(RatingControl), default(ImageSource), BindingMode.TwoWay, propertyChanged: UpdateStarsDisplay);

        public ImageSource FullStarImage
        {
            get => (ImageSource)GetValue(FullStarImageProperty);
            set => SetValue(FullStarImageProperty, value);
        }

        #endregion

        #region EmptyStarImage property

        public static readonly BindableProperty EmptyStarImageProperty = BindableProperty.Create(nameof(EmptyStarImage), typeof(ImageSource), typeof(RatingControl), default(ImageSource), BindingMode.TwoWay, propertyChanged: UpdateStarsDisplay);

        public ImageSource EmptyStarImage
        {
            get => (ImageSource)GetValue(EmptyStarImageProperty);
            set => SetValue(EmptyStarImageProperty, value);
        }

        #endregion

        #region HalfStarImage property

        public static readonly BindableProperty HalfStarImageProperty = BindableProperty.Create(nameof(HalfStarImage), typeof(ImageSource), typeof(RatingControl), default(ImageSource), BindingMode.TwoWay, propertyChanged: UpdateStarsDisplay);

        public ImageSource HalfStarImage
        {
            get => (ImageSource)GetValue(HalfStarImageProperty);
            set => SetValue(HalfStarImageProperty, value);
        }

        #endregion

        static void UpdateStarsDisplay(BindableObject bindable, object oldValue, object newValue)
        {
            ((RatingControl)bindable).UpdateStarsDisplay();
        }

        // Fill the star based on the Rating value
        void UpdateStarsDisplay()
        {
            for (var i = 0; i < _starImages.Count; i++)
            {
                _starImages[i].Source = GetStarSource(i);
            }
        }

        ImageSource GetStarSource(int position)
        {
            var currentStarMaxRating = (position + 1);

            if (Precision.Equals(PrecisionType.Half))
            {
                currentStarMaxRating *= 2;
            }

            if (Rating >= currentStarMaxRating)
            {
                return FullStarImage;
            }
            if (Rating >= currentStarMaxRating - 1 && Precision.Equals(PrecisionType.Half))
            {
                return HalfStarImage;
            }

            return EmptyStarImage;
        }

        public enum PrecisionType
        {
            Full,
            Half
        }
    }
}