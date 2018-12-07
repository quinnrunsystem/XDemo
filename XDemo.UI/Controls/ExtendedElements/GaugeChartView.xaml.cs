using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;
using Entry = Microcharts.Entry;
using Gauge;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace XDemo.UI.Controls.ExtendedElements
{
    public partial class GaugeChartView : ContentView
    {
        readonly string[] chartProperty = new string[]
        {
            nameof(CurrentValue),
            nameof(DesiredValue),
            nameof(MaxValue),
            nameof(CurrentColor),
            nameof(DesiredColor),
            nameof(MaxColor),
            nameof(ChartBackgroundColor),
            nameof(MinValue),
            nameof(StartAngle),
            nameof(EndAngle),
            nameof(LineSize),
            nameof(ResourceObject),
            nameof(CheckPointOnImage),
            nameof(CheckPointOffImage),
            nameof(CheckPoint),
        };
        public GaugeChartView()
        {
            InitializeComponent();
            PropertyChanged += ChartPropertyChanged;
            DrawGaugeChart();
        }
        private void ChartPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (chartProperty.Contains(e.PropertyName))
            {
                DrawGaugeChart();
            }
        }
        public int CurrentValue
        {
            get { return (int)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }
        public static readonly BindableProperty CurrentValueProperty =
  BindableProperty.Create(nameof(CurrentValue), typeof(int), typeof(GaugeChartView), 3000);

        public int DesiredValue
        {
            get { return (int)GetValue(DesiredValueProperty); }
            set { SetValue(DesiredValueProperty, value); }
        }
        public static readonly BindableProperty DesiredValueProperty =
  BindableProperty.Create(nameof(DesiredValue), typeof(int), typeof(GaugeChartView), 2500);

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly BindableProperty MaxValueProperty =
  BindableProperty.Create(nameof(MaxValue), typeof(int), typeof(GaugeChartView), 5000);

        public Color CurrentColor
        {
            get { return (Color)GetValue(CurrentColorProperty); }
            set { SetValue(CurrentColorProperty, value); }
        }
        public static readonly BindableProperty CurrentColorProperty =
  BindableProperty.Create(nameof(CurrentColor), typeof(Color), typeof(GaugeChartView), Color.FromHex("#08BBF1"));
        public Color DesiredColor
        {
            get { return (Color)GetValue(DesiredColorProperty); }
            set { SetValue(DesiredColorProperty, value); }
        }
        public static readonly BindableProperty DesiredColorProperty =
  BindableProperty.Create(nameof(DesiredColor), typeof(Color), typeof(GaugeChartView), Color.FromHex("#378D93"));
        public Color MaxColor
        {
            get { return (Color)GetValue(MaxColorProperty); }
            set { SetValue(MaxColorProperty, value); }
        }
        public static readonly BindableProperty MaxColorProperty =
  BindableProperty.Create(nameof(MaxColor), typeof(Color), typeof(GaugeChartView), Color.FromHex("#3D4344"));
        public Color ChartBackgroundColor
        {
            get { return (Color)GetValue(ChartBackgroundColorProperty); }
            set { SetValue(ChartBackgroundColorProperty, value); }
        }
        public static readonly BindableProperty ChartBackgroundColorProperty =
  BindableProperty.Create(nameof(ChartBackgroundColor), typeof(Color), typeof(GaugeChartView), Color.Transparent);
        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        public static readonly BindableProperty MinValueProperty =
  BindableProperty.Create(nameof(MinValue), typeof(int), typeof(GaugeChartView), 0);
        public int StartAngle
        {
            get { return (int)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }
        public static readonly BindableProperty StartAngleProperty =
  BindableProperty.Create(nameof(StartAngle), typeof(int), typeof(GaugeChartView), 130);

        public int EndAngle
        {
            get { return (int)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }
        public static readonly BindableProperty EndAngleProperty =
  BindableProperty.Create(nameof(EndAngle), typeof(int), typeof(GaugeChartView), 50);
        public int LineSize
        {
            get { return (int)GetValue(LineSizeProperty); }
            set { SetValue(LineSizeProperty, value); }
        }
        public static readonly BindableProperty LineSizeProperty =
  BindableProperty.Create(nameof(LineSize), typeof(int), typeof(GaugeChartView), 50);
        public object ResourceObject
        {
            get { return GetValue(ResourceObjectProperty); }
            set { SetValue(ResourceObjectProperty, value); }
        }
        public static readonly BindableProperty ResourceObjectProperty =
  BindableProperty.Create(nameof(ResourceObject), typeof(object), typeof(GaugeChartView), null);
        public string CheckPointOnImage
        {
            get { return (string)GetValue(CheckPointOnImageProperty); }
            set { SetValue(CheckPointOnImageProperty, value); }
        }
        public static readonly BindableProperty CheckPointOnImageProperty =
  BindableProperty.Create(nameof(CheckPointOnImage), typeof(string), typeof(GaugeChartView), "EmbeddedResource.pink_circle_2.png");
        public string CheckPointOffImage
        {
            get { return (string)GetValue(CheckPointOffImageProperty); }
            set { SetValue(CheckPointOffImageProperty, value); }
        }
        public static readonly BindableProperty CheckPointOffImageProperty =
  BindableProperty.Create(nameof(CheckPointOffImage), typeof(string), typeof(GaugeChartView), "EmbeddedResource.gray_circle_2.png");

        public IEnumerable<float> CheckPoint
        {
            get { return (IEnumerable<float>)GetValue(CheckPointProperty); }
            set { SetValue(CheckPointProperty, value); }
        }
        public static readonly BindableProperty CheckPointProperty =
  BindableProperty.Create(nameof(CheckPoint), typeof(IEnumerable<float>), typeof(GaugeChartView), new[] { 2000f, 3000, 4000, 4900 });

        public void DrawGaugeChart()
        {
            var currentChartValue = new Entry(CurrentValue) { Color = CurrentColor.ToSKColor() };
            var desiredChartValue = new Entry(DesiredValue) { Color = DesiredColor.ToSKColor() };
            var maxChartValue = new Entry(MaxValue) { Color = MaxColor.ToSKColor() };

            var chart = new GaugeChart
            {
                MinValue = MinValue,
                MaxValue = MaxValue,
                StartAngle = StartAngle,
                EndAngle = EndAngle,
                LineSize = LineSize,
                BackgroundColor = ChartBackgroundColor.ToSKColor(),
                ResourceObject = ResourceObject,
                CheckPointOnImage = CheckPointOnImage,
                CheckPointOffImage = CheckPointOffImage,
                CurrentValueEntry = currentChartValue,
                DesiredValueEntry = desiredChartValue,
                CheckPoint = CheckPoint,
            };

            if (CurrentValue > DesiredValue)
                chart.Entries = new[] { maxChartValue, currentChartValue, desiredChartValue };
            else if (CurrentValue < DesiredValue)
                chart.Entries = new[] { maxChartValue, desiredChartValue, currentChartValue };
            else
                chart.Entries = new[] { maxChartValue, desiredChartValue, currentChartValue };

            this.chartView.Chart = chart;
        }
    }
}
