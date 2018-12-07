using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entry = Microcharts.Entry;
namespace Gauge
{
    public class GaugeChart : Chart
    {
        public GaugeChart()
        {
            BackgroundColor = SKColor.Parse("#F2F2F2");
            LabelTextSize = 24f;
        }

        public float LineSize { get; set; } = 28;
        public float CaptionMargin { get; set; } = 12;
        public object ResourceObject { get; set; }
        public string CheckPointOnImage { get; set; }
        public string CheckPointOffImage { get; set; }

        float AbsoluteMaximum => Entries.Select(x => x.Value).Concat(new[] { MaxValue, MinValue, InternalMinValue ?? 0 }).Max(x => Math.Abs(x));
        float AbsoluteMinimum => Entries.Select(x => x.Value).Concat(new[] { MaxValue, MinValue, InternalMinValue ?? 0 }).Min(x => Math.Abs(x));
        float ValueRange => AbsoluteMaximum - AbsoluteMinimum;
        /// <summary>
        /// Counter wise 
        /// </summary>
        public float StartAngle { get; set; } = 150;
        /// <summary>
        /// Counter wise 
        /// </summary>
        public float EndAngle { get; set; } = 30;
        public IEnumerable<float> CheckPoint { get; set; }

        public Entry CurrentValueEntry { get; set; }

        public Entry DesiredValueEntry { get; set; }

        SKBitmap onBitmap;
        SKBitmap offBitmap;
        public override void DrawContent(SKCanvas canvas, int width, int height)
        {
            canvas.Clear(BackgroundColor);
            System.Reflection.Assembly assembly = ResourceObject.GetType().GetTypeInfo().Assembly;
            var assemblyName = assembly.GetName().Name;
            string onImageId = $"{assemblyName}.{CheckPointOnImage}";
            string offImageId = $"{assemblyName}.{CheckPointOffImage}";

            using (System.IO.Stream stream = assembly.GetManifestResourceStream(onImageId))
            {
                onBitmap = SKBitmap.Decode(stream);
            }
            using (System.IO.Stream stream = assembly.GetManifestResourceStream(offImageId))
            {
                offBitmap = SKBitmap.Decode(stream);
            }

            //var relativeScaleWidth = width / 465.0f;
            //var strokeWidth = relativeScaleWidth * LineSize;

            //var radius = (width) * 2.0f / 4f;
            //var cx = (int)(radius);

            var size = Math.Min(width, height);
            var relativeScaleWidth = size / 465.0f;

            var strokeWidth = relativeScaleWidth * LineSize;
            var radius = size * 2.0f / 4f;

            var cx = (int)((width) * 2.0f / 4f);

            var cy = Convert.ToInt32((height / 2.0f) /*+ radius / 3.7f*/);
            var radiusSpace = radius - 0.75f * strokeWidth;

            DrawChart(canvas, width, height, cx, cy, radiusSpace, strokeWidth, relativeScaleWidth);
        }
        protected void DrawChart(SKCanvas canvas, int width, int height, int cx, int cy, float radiusSpace, float strokeWidth, float relativeScaleWidth)
        {

            foreach (var entry in Entries)
            {
                DrawChart(canvas, entry, radiusSpace, cx, cy, strokeWidth);
            }

            DrawCaption(canvas, cx, cy, radiusSpace, relativeScaleWidth, strokeWidth);
        }
        protected virtual void DrawChart(SKCanvas canvas, Entry entry, float radius, int cx, int cy, float strokeWidth)
        {
            using (var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                StrokeCap = SKStrokeCap.Round,
                Color = entry.Color,
                IsAntialias = true
            })
            {
                using (var path = new SKPath())
                {
                    //var sweepAngle = 180 * (Math.Abs(entry.Value) - AbsoluteMinimum) / ValueRange;
                    //path.AddArc(SKRect.Create(cx - radius, cy - radius, 2 * radius, 2 * radius), StartAngle, sweepAngle);
                    var sweepAngle = (360 - (StartAngle - EndAngle)) * (Math.Abs(entry.Value) - AbsoluteMinimum) / ValueRange;
                    path.AddArc(SKRect.Create(cx - radius, cy - radius, 2 * radius, 2 * radius), StartAngle, sweepAngle);

                    canvas.DrawPath(path, paint);
                }
            }
            var checkpointSize = strokeWidth * 1.33f;
            foreach (var checkPoint in CheckPoint)
            {
                var bitmap = checkPoint <= CurrentValueEntry.Value ? onBitmap : offBitmap;
                GetPositionFromPoint(checkPoint, cx, cy, radius, out float xAngle, out float yAngle);
                var xLoc = xAngle - checkpointSize / 2;
                var yLoc = yAngle - checkpointSize / 2;
                var location = new SKPoint(xLoc, yLoc);
                var size = new SKSize(checkpointSize, checkpointSize);
                canvas.DrawBitmap(bitmap, SKRect.Create(location, size));
            }
        }
        void GetPositionFromPoint(float point, float cx, float cy, float radius, out float xAngle, out float yAngle)
        {
            var sweepangle = (360 - (StartAngle - EndAngle)) * (Math.Abs(point) - AbsoluteMinimum) / ValueRange;
            var angle = StartAngle + sweepangle;
            var angleRad = angle / 180 * Math.PI;
            var dx = Math.Cos(angleRad) * radius;
            var dy = Math.Sin(angleRad) * radius;
            xAngle = cx + (float)dx;
            yAngle = cy + (float)dy;
        }

        protected virtual void DrawCaption(SKCanvas canvas, int cx, int cy, float radius, float relativeScaleWidth,
    float strokeWidth)
        {
            var medium = AbsoluteMinimum + ((AbsoluteMaximum - AbsoluteMinimum) / 2);
            //var degreeSign = '°';
            var degreeSign = "";
            void GetCaptionPos(float point, out float xPos, out float yPos, out SKTextAlign align)
            {
                GetPositionFromPoint(point, cx, cy, radius, out float xAngle, out float yAngle);
                var deltaX = strokeWidth + CaptionMargin;
                if (point < medium)
                {
                    xPos = xAngle - deltaX;
                    align = SKTextAlign.Right;
                }
                else if (point > medium)
                {
                    xPos = xAngle + deltaX;
                    align = SKTextAlign.Left;

                }
                else
                {
                    xPos = xAngle;
                    align = SKTextAlign.Center;
                }
                yPos = yAngle;
            }
            //#region Show point caption
            //float xPosOut;
            //float yPosOut;
            //SKTextAlign alignOut;
            //GetCaptionPos(AbsoluteMinimum, out xPosOut, out yPosOut, out alignOut);
            //canvas.DrawCaptionLabels(string.Empty, SKColor.Empty, $"{AbsoluteMinimum}{degreeSign}", SKColors.Black, LabelTextSize * relativeScaleWidth, new SKPoint(xPosOut, yPosOut), alignOut);
            //GetCaptionPos(AbsoluteMaximum, out xPosOut, out yPosOut, out alignOut);
            //canvas.DrawCaptionLabels(string.Empty, SKColor.Empty, $"{AbsoluteMaximum}{degreeSign}", SKColors.Black, LabelTextSize * relativeScaleWidth, new SKPoint(xPosOut, yPosOut), alignOut);

            //canvas.DrawCaptionLabels(string.Empty, SKColor.Empty, $"{medium}{degreeSign}", SKColors.Black, LabelTextSize * relativeScaleWidth, new SKPoint(cx, cy - radius - strokeWidth - 2 * relativeScaleWidth), SKTextAlign.Center); 
            //#endregion



            if (CurrentValueEntry != null)
            {
                canvas.DrawCaptionLabels(string.Empty, SKColor.Empty, $"Current: {CurrentValueEntry.Value}{degreeSign}",
                    SKColor.Parse("#174A51"), LabelTextSize * relativeScaleWidth, new SKPoint(cx, cy - radius * 0.9f / 4f),
                    SKTextAlign.Center);
            }

            if (DesiredValueEntry != null)
            {
                canvas.DrawCaptionLabels(string.Empty, SKColor.Empty, $"Desired: {DesiredValueEntry?.Value}{degreeSign}",
                    SKColor.Parse("#378D93"), LabelTextSize * relativeScaleWidth, new SKPoint(cx, cy - radius * 0.0f / 4f),
                    SKTextAlign.Center);
            }
        }

    }
}
