using PBIXInspectorImageLibrary.Drawing.Palette;
using SkiaSharp;

namespace PBIXInspectorImageLibrary.Drawing
{
    internal class ReportPage(string pageName, string pageDisplayName, ReportPage.PageSize pageSize, List<ReportPage.VisualContainer> visualContainers) : IDisposable
    {
        private string _pageName = pageName;
        private string _pageDisplayName = pageDisplayName;
        private PageSize _pageSize = pageSize;
        private List<VisualContainer> _visualContainers = visualContainers;
        private SKCanvas? _canvas;
        private SKBitmap? _bitmap;


        public string PageDisplayName
        {
            get
            {
                return _pageDisplayName;
            }
        }

        public void Dispose()
        {
            _bitmap?.Dispose();
            _canvas?.Dispose();
        }

        public void Draw()
        {
            const int PAGEMARGIN = 10;
            const int VISOFFSET = 5;
            const int TEXTPADDINGX = VISOFFSET + 2;
            const int TEXTPADDINGY = VISOFFSET + 12;
            const string FONT = "Arial";
            const int FONTSIZE = 12;

            int[] iconSizes = [32, 64, 128];

            int pageWidth = _pageSize.Width + PAGEMARGIN;
            int pageHeight = _pageSize.Height + PAGEMARGIN;

            _bitmap = new SKBitmap(pageWidth, pageHeight);
            _canvas = new SKCanvas(_bitmap);

            //bitmap canvas
            var canvas = new SKRect() { Left = 0, Top = 0, Size = new SKSize(_bitmap.Width, _bitmap.Height) };
            _canvas.DrawRect(canvas, PaintPalette.BlackFillStroke);

            //page outline
            var pageOutline = new SKRect() { Left = 0 + VISOFFSET, Top = 0 + VISOFFSET, Size = new SKSize(_pageSize.Width, _pageSize.Height) };
            _canvas.DrawRect(pageOutline, PaintPalette.WhiteStroke);


            var iconBitmap = SKBitmap.Decode(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.PBIInspectorICO));

            var font = new SKFont(SKTypeface.FromFamilyName(FONT), FONTSIZE);

            foreach (var vc in _visualContainers.OrderBy(_ => !_.Pass))
            {

                var paint = vc.Pass ? PaintPalette.WhiteStroke : PaintPalette.FailStroke;
                var textPaint = vc.Pass ? PaintPalette.WhiteFill : PaintPalette.FailFill;
                if (!vc.Visible) paint = paint.AddDash();

                var rect = new SKRect() { Left = vc.X + VISOFFSET, Top = vc.Y + VISOFFSET, Size = new SKSize(vc.Width, vc.Height) };
                _canvas.DrawRect(rect, paint);

                var iconSize = (from s in iconSizes where s <= Math.Max(rect.Width, rect.Height) / 2 orderby s descending select s).FirstOrDefault();

                var ico = SKImage.FromBitmap(iconBitmap.Resize(new SKSizeI(iconSize, iconSize), SKFilterQuality.Medium));

                if (!vc.Pass)
                {
                    _canvas.DrawImage(ico, rect.Left + VISOFFSET + (rect.Width - iconSize) / 2, rect.Top + VISOFFSET + (rect.Height - iconSize) / 2);
                };

                _canvas.DrawText(string.Concat(vc.VisualType), vc.X + TEXTPADDINGX, vc.Y + TEXTPADDINGY, font, textPaint);
                _canvas.DrawText(string.Concat(vc.Name), vc.X + TEXTPADDINGX, vc.Y + TEXTPADDINGY + FONTSIZE, font, textPaint);
            }

        }

        public void Save(string path)
        {
            using var fs = new FileStream(path, FileMode.Create);
            _bitmap?.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fs);
        }

        public class PageSize
        {
            public int Height;
            public int Width;
        }

        public class VisualContainer
        {
            public string? Name;
            public string? VisualType;
            public int X;
            public int Y;
            public int Height;
            public int Width;
            public bool Visible;
            public bool Pass;
        }
    }
}
