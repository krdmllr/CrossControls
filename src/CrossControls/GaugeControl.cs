using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CrossControls.Utility;
using Xamarin.Forms;

namespace CrossControls
{
    public class GaugeControl : SKCanvasView
    {
        #region events

        /// <summary>
        /// Raised when the <see cref="Progress"/> changes.
        /// </summary>
        public event EventHandler<double> ProgressChanged;

        #endregion

        #region properties

        /// <summary>
        /// Identifies the <see cref="Padding"/> property.
        /// </summary>
        public static readonly BindableProperty PaddingProperty =
            BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(GaugeControl), new Thickness(0),
                validateValue: (_, value) => value != null,
                propertyChanged: InvalidateSurfaceOnChange);

        /// <summary>
        /// View padding.
        /// This is a bindable property.
        /// </summary>
        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set
            {
                SetValue(PaddingProperty, value);
                InvalidateSurface();
            }
        }

        /// <summary>
        /// Identifies the <see cref="Progress"/> property.
        /// </summary>
        public static readonly BindableProperty ProgressProperty =
           BindableProperty.Create(nameof(Progress), typeof(double), typeof(GaugeControl), 0.0, validateValue: (_, value) => (double)value <= 1 && (double)value >= 0,
               propertyChanged: InvalidateSurfaceOnChange);

        /// <summary>
        /// Gauge progress min = 0; max = 1.
        /// This is a bindable property.
        /// </summary>
        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set
            {
                if (Progress == value)
                    return;

                SetValue(ProgressProperty, value);
                ProgressChanged?.Invoke(this, value);
                InvalidateSurface();
            }
        }

        /// <summary>
        /// Identifies the <see cref="GaugeWidth"/> property.
        /// </summary>
        public static readonly BindableProperty GaugeWidthProperty =
            BindableProperty.Create(nameof(GaugeWidth), typeof(double), typeof(GaugeControl), 20.0,
                propertyChanged: InvalidateSurfaceOnChange);

        /// <summary>
        /// Width of the gauge line.
        /// This is a bindable property.
        /// </summary>
        public double GaugeWidth
        {
            get => (double)GetValue(GaugeWidthProperty);
            set
            {
                SetValue(GaugeWidthProperty, value);
                InvalidateSurface();
            }
        }

        /// <summary>
        /// Identifies the <see cref="GaugeForegroundColors"/> property.
        /// </summary>
        public static readonly BindableProperty GaugeColorsProperty =
            BindableProperty.Create(nameof(GaugeForegroundColors), typeof(IEnumerable<Color>), typeof(GaugeControl), new[] { Color.Blue },
                propertyChanged: InvalidateSurfaceOnChange);

        /// <summary>
        /// Foreground color of the gauge.
        /// Can set multiple colors to create a gradient.
        /// This is a bindable property.
        /// </summary>
        public IEnumerable<Color> GaugeForegroundColors
        {
            get => (IEnumerable<Color>)GetValue(GaugeColorsProperty);
            set
            {
                SetValue(GaugeColorsProperty, value);
                InvalidateSurface();
            }
        }

        /// <summary>
        /// Identifies the <see cref="HandleDiameter"/> property.
        /// </summary>
        public static readonly BindableProperty HandleDiameterProperty =
            BindableProperty.Create(nameof(HandleDiameter), typeof(double), typeof(GaugeControl), 18.0,
                propertyChanged: InvalidateSurfaceOnChange);

        /// <summary>
        /// Diameter of the gauge highlight.
        /// This is a bindable property.
        /// </summary>
        public double HandleDiameter
        {
            get => (double)GetValue(HandleDiameterProperty);
            set
            {
                SetValue(HandleDiameterProperty, value);
                InvalidateSurface();
            }
        }

        /// <summary>
        /// Identifies the <see cref="GaugeBackgroundColors"/> property.
        /// </summary>
        public static readonly BindableProperty GaugeBackgroundColorsProperty =
            BindableProperty.Create(nameof(GaugeBackgroundColors), typeof(IEnumerable<Color>), typeof(GaugeControl), new[] { Color.White },
                propertyChanged: InvalidateSurfaceOnChange);

        /// <summary>
        /// Background color of the gauge.
        /// Can set multiple colors to create a gradient.
        /// This is a bindable property.
        /// </summary>
        public IEnumerable<Color> GaugeBackgroundColors
        {
            get => (IEnumerable<Color>)GetValue(GaugeBackgroundColorsProperty);
            set
            {
                SetValue(GaugeBackgroundColorsProperty, value);
                InvalidateSurface();
            }
        }

        /// <summary>
        /// Identifies the <see cref="HandleColor"/> property.
        /// </summary>
        public static readonly BindableProperty HandleColorProperty =
            BindableProperty.Create(nameof(HandleColor), typeof(Color), typeof(GaugeControl), Color.White,
                propertyChanged: InvalidateSurfaceOnChange);

        /// <summary>
        /// Color of the gauge handle.
        /// This is a bindable property.
        /// </summary>
        public Color HandleColor
        {
            get => (Color)GetValue(HandleColorProperty);
            set
            {
                SetValue(HandleColorProperty, value);
                InvalidateSurface();
            }
        }
        #endregion

        /// <summary>
        /// Current point the user touches.
        /// </summary>
        private SKPoint? CurrentTouchPoint { get; set; }

        public GaugeControl()
        {
            EnableTouchEvents = true;
        }

        /// <summary>
        /// Invalidates the surface when fired.
        /// </summary> 
        private static void InvalidateSurfaceOnChange(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (GaugeControl)bindable;

            // Avoid unnecessary invalidation
            if (oldValue != newValue)
                control.InvalidateSurface();
        }

        /// <summary>
        /// Called when the user interacts with the surface.
        /// </summary>
        protected override void OnTouch(SKTouchEventArgs e)
        {
            // Check if the surface is touched and set the CurrentTouchPoint
            if (!e.InContact)
            {
                CurrentTouchPoint = null;
            }
            else
            {
                CurrentTouchPoint = e.Location;
            }

            InvalidateSurface();
            e.Handled = true;
        }

        /// <summary>
        /// Called on when the surface is drawn.
        /// </summary> 
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var info = e.Info;
            var surface = e.Surface;
            var canvas = surface.Canvas;
            canvas.Clear(); 

            var circleWidth = (int)this.IndependentToPixelSize(GaugeWidth * 2);
            var circlePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = circleWidth,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true
            };

            var scaledPadding = this.GetScaledThickness(Padding);
            var middleGap = 100;
            var offset = 270 - middleGap / 2;
            var maxAngle = 360 - middleGap;
            var lastAngle = (float)(maxAngle * Progress);
            var angle = lastAngle;
            var bottomGaugePoint = PointInCircle(new SKPoint(1f, 1f), 1, -offset);
            var gaugeHighOfTotalCircle = bottomGaugePoint.Y / 2;

            var totalWidth = info.Width - (float)scaledPadding.Right - (float)scaledPadding.Left - circleWidth;
            var totalHeight = (info.Height - (float)scaledPadding.Top - (float)scaledPadding.Bottom - circleWidth) * 1 / gaugeHighOfTotalCircle;
            var size = totalWidth < totalHeight ? totalWidth : totalHeight;
            var offsetLeft = totalWidth < totalHeight ? scaledPadding.Left + circleWidth / 2 : scaledPadding.Left + (info.Width - size) / 2 + circleWidth / 2;
            var offsetTop = totalHeight < totalWidth ? scaledPadding.Top + circleWidth / 2 : scaledPadding.Top + (info.Height - size) / 2 + circleWidth / 2 + (size * (1 - gaugeHighOfTotalCircle) / 2);
            var bounds = new SKRect(0, 0, size, size);
            bounds.Offset((float)offsetLeft, (float)offsetTop);
            var center = new SKPoint(bounds.MidX, bounds.MidY);
            var radius = (int)bounds.Width / 2;
            var highlightDiameter = (int)this.IndependentToPixelSize(HandleDiameter);

            if (GaugeForegroundColors != null && GaugeForegroundColors.Any())
            {
                var matrix = SKMatrix.MakeRotationDegrees(360 - offset + - 15, center.X, center.Y);
                if (GaugeForegroundColors.Count() > 1)
                    circlePaint.Shader = SKShader.CreateSweepGradient(center,
                        GaugeForegroundColors.Select(x => x.ToSKColor()).ToArray(), null, SKShaderTileMode.Repeat, 0, maxAngle + 30, matrix);
                else
                    circlePaint.Color = GaugeForegroundColors.First().ToSKColor();
            }

            // Draw background
            if (GaugeBackgroundColors != null && GaugeBackgroundColors.Any())
            {
                var backgroundCircle = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = circleWidth,
                    StrokeCap = SKStrokeCap.Round,
                    IsAntialias = true
                };

                var bac = GaugeBackgroundColors.Count();

                if (bac > 1)
                    backgroundCircle.Shader = SKShader.CreateSweepGradient(center,
                        GaugeBackgroundColors.Select(x => x.ToSKColor()).ToArray(), null, SKShaderTileMode.Repeat, 90, 180);
                else
                    backgroundCircle.Color = GaugeBackgroundColors.First().ToSKColor();


                using (SKPath path = new SKPath())
                {
                    path.AddArc(bounds, -offset, maxAngle);
                    e.Surface.Canvas.DrawPath(path, backgroundCircle);
                }
            }

            if (CurrentTouchPoint.HasValue)
            {
                // Calculate the closest point in the circle to the current touch position 
                var pointInCircle = VectorMath.ClosestPointInCircle(center, radius, CurrentTouchPoint.Value);

                var radian = Math.Atan2(pointInCircle.Y - center.Y, pointInCircle.X - center.X); 
                angle = (float)(radian * (180 / Math.PI));
                if (angle < 90)
                    angle += 360;
                angle -= 140;

                Debug.WriteLine(angle);

                if (angle - lastAngle > 50 || angle - lastAngle < -50)
                {
                    angle = lastAngle;
                }

                if (angle > maxAngle)
                {
                    angle = maxAngle;
                }
                if (angle < 0)
                {
                    angle = 0;
                }

                Progress = angle == 0 ? 0 : angle / maxAngle;
            }

            if (angle < 1)
                angle = 0.1f;

            using (SKPath path = new SKPath())
            {
                path.AddArc(bounds, -offset, angle);
                canvas.DrawPath(path, circlePaint);
            }

            if (HandleColor != null)
            { 
                var currentPositon = PointInCircle(center, radius, angle - offset);

                var highlightColor = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = HandleColor.ToSKColor(),
                    IsAntialias = true,
                    BlendMode = SKBlendMode.Src
                };

                // Color.Transparent will erase all layers beneath but we only want to erase the gauge layer, not the background
                // In order to do that, we assign the background gauge color if the highlight Color is transparent and a background color is set
                if (HandleColor == Color.Transparent
                    && GaugeBackgroundColors != null
                    && GaugeBackgroundColors.Any())
                {
                    highlightColor.Color = GaugeBackgroundColors.First().ToSKColor();
                }

                canvas.DrawCircle(currentPositon, highlightDiameter, highlightColor);
            }
        }

        private SKPoint PointInCircle(SKPoint center, int radius, double angleInDegree)
        {
            var angleInRadian = VectorMath.DegreeToRadian((float)angleInDegree);
            return VectorMath.PointInCircleAtAngle(center, radius, angleInRadian);
        }
    }
}
