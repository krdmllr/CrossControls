using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace CrossControls.Utility
{
    /// <summary>
    /// Helper for device/independent size scaling
    /// </summary>
    public static class ScalingHelper
    {
        /// <summary>
        /// Converts a device independent size to the equivalent in device pixels.
        /// </summary>
        /// <param name="canvasView">Canvas view the size will be scaled for.</param>
        /// <param name="independentSize">The device independent size reported by Xamarin.Forms</param>
        /// <returns>The scaled size.</returns>
        public static double IndependentToPixelSize(this SKCanvasView canvasView, double independentSize)
        {
            // Calculate how much the independent size needs to be scaled
            var canvasSize = canvasView.CanvasSize;
            var scale = canvasSize.Width / canvasView.Width;
            return independentSize * scale;
        }

        /// <summary>
        /// Converts a device independent <see cref="Thickness"/> to the equivalent in device pixels.
        /// </summary>
        /// <param name="canvasView">Canvas view the size will be scaled for.</param>
        /// <param name="independentThickness"></param>
        /// <returns>The scaled thickness.</returns>
        public static Thickness GetScaledThickness(this SKCanvasView canvasView, Thickness independentThickness)
        {
            return new Thickness(IndependentToPixelSize(canvasView, independentThickness.Left),
                IndependentToPixelSize(canvasView, independentThickness.Top),
                IndependentToPixelSize(canvasView, independentThickness.Right),
                IndependentToPixelSize(canvasView, independentThickness.Bottom));
        }
    }
}