using SkiaSharp;

namespace PBIXInspectorImageLibrary.Drawing.Palette
{
    internal static class PaintPalette
    {
        private const float STROKE_WIDTH = 2;
        private static readonly float[] STROKE_DASH = [2.5f, 2.5f];
        public static SKPaint BlackFillStroke => new() { Color = ColorPalette.Black, Style = SKPaintStyle.StrokeAndFill, StrokeWidth = STROKE_WIDTH };
        public static SKPaint WhiteStroke => new() { Color = ColorPalette.White, Style = SKPaintStyle.Stroke, StrokeWidth = STROKE_WIDTH };
        public static SKPaint FailStroke => new() { Color = ColorPalette.Fail, Style = SKPaintStyle.Stroke, StrokeWidth = STROKE_WIDTH };
        public static SKPaint WhiteFill => new() { Color = ColorPalette.White, Style = SKPaintStyle.Fill };
        public static SKPaint FailFill => new() { Color = ColorPalette.Fail, Style = SKPaintStyle.Fill};
        public static SKPaint AddDash(this SKPaint paint)
        {
            paint.PathEffect = SKPathEffect.CreateDash(STROKE_DASH, 0);
            return paint;
        }
    }
}
