using System;
using SkiaSharp;

namespace CrossControls.Utility
{
    public static class VectorMath
    {
        /// <summary>
        /// Calculate the distance between two points.
        /// </summary> 
        /// <returns>Distance between the two points.</returns>
        public static float DistanceBetweenPoints(SKPoint p1, SKPoint p2)
        {
            var xDistanceCenterPoint = (p1.X - p2.X);
            var yDistanceCenterPoint = (p1.Y - p2.Y);
            return (float)Math.Sqrt(Math.Pow(xDistanceCenterPoint, 2) + Math.Pow(yDistanceCenterPoint, 2));
        }

        /// <summary>
        /// Calculates the closest point in a circle to a point out- or inside the circle.
        /// </summary>
        /// <param name="circleCenter">Center of the circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="point">Point out- or inside the circle.</param>
        /// <returns>Closest point of the circle to the <paramref name="point"/>.</returns>
        public static SKPoint ClosestPointInCircle(SKPoint circleCenter, float radius, SKPoint point)
        {
            var xDistanceCenterPoint = (point.X - circleCenter.X);
            var yDistanceCenterPoint = (point.Y - circleCenter.Y);

            var distanceCenterPoint = DistanceBetweenPoints(circleCenter, point);

            var x = circleCenter.X + radius * (xDistanceCenterPoint / distanceCenterPoint);
            var y = circleCenter.Y + radius * (yDistanceCenterPoint / distanceCenterPoint);
            return new SKPoint(x, y);
        }

        /// <summary>
        /// Converts from degree to radian.
        /// </summary>
        /// <param name="degree">Value in degree.</param>
        /// <returns>Value in radian.</returns>
        public static float DegreeToRadian(float degree)
        {
            return (float)(degree * Math.PI / 180);
        }

        /// <summary>
        /// Calculates the point on a circle at the given <paramref name="angleInRadian"/>.
        /// </summary>
        /// <param name="circleCenter">Center of the circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="angleInRadian">Angle of the point in radian.</param>
        /// <returns></returns>
        public static SKPoint PointInCircleAtAngle(SKPoint circleCenter, float radius, float angleInRadian)
        {
            var x = circleCenter.X + radius * Math.Cos(angleInRadian);
            var y = circleCenter.Y + radius * Math.Sin(angleInRadian);
            return new SKPoint((float)x, (float)y);
        }
    }
}