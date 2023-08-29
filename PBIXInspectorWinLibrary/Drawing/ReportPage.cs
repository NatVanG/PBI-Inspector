using System.Drawing;
using System.Drawing.Imaging;
using static PBIXInspectorWinLibrary.Drawing.ReportPage;

#pragma warning disable CA1416 // Validate platform compatibility
namespace PBIXInspectorWinLibrary.Drawing
{
    internal class ReportPage
    {
        private string _pageName;
        private string _pageDisplayName;
        private PageSize _pageSize;
        private List<VisualContainer> _visualContainers;
        private Bitmap _bitmap;

        public ReportPage(string pageName, string pageDisplayName, PageSize pageSize, List<VisualContainer> visualContainers)
        {
            _pageName = pageName;
            _pageDisplayName = pageDisplayName;
            _pageSize = pageSize;
            _visualContainers = visualContainers;
        }

        public string PageDisplayName
        {
            get
            {
                return _pageDisplayName;
            }
        }

        public void Draw()
        {
            Pen whitePen = new Pen(Color.FromArgb(255, 255, 255));
            //Pen grayPen = new Pen(Color.Gray);
            Pen failPen = new Pen(Color.FromArgb(237, 19, 93));
            Pen blackPen = new Pen(Color.FromArgb(0, 0, 0));

            //var Ico128 = new Icon("icon/pbiinspector.ico", 128, 128);
            //var Ico64 = new Icon("icon/pbiinspector.ico", 64, 64);
            //var Ico32 = new Icon("icon/pbiinspector.ico", 32, 32);
            int[] iconSizes = { 32, 64, 128 };

            const int PAGEMARGIN = 10;
            const int VISOFFSET = 5;
            const string FONT = "Arial";
            const int FONTSIZE = 12;
            const string ICONPATH = @"Files\icon\pbiinspector.ico";

            _bitmap = new Bitmap(_pageSize.Width + PAGEMARGIN, _pageSize.Height + PAGEMARGIN);
            Graphics g = Graphics.FromImage(_bitmap);

            try
            {
                //bitmap canvas
                var canvas = new Rectangle() { X = 0, Y = 0, Width = _bitmap.Width, Height = _bitmap.Height };
                g.FillRectangle(blackPen.Brush, canvas);
                g.DrawRectangle(blackPen, canvas);

                //page outline
                var outline = new Rectangle() { X = 0 + VISOFFSET, Y = 0 + VISOFFSET, Width = _pageSize.Width, Height = _pageSize.Height };
                g.FillRectangle(blackPen.Brush, outline);
                g.DrawRectangle(whitePen, outline);

                foreach (var vc in _visualContainers.OrderBy(_ => !_.Pass))
                {
                    var rect = new Rectangle() { X = vc.X + 5, Y = vc.Y + 5, Width = vc.Width, Height = vc.Height };
                    var pen = vc.Pass ? whitePen : failPen;

                    if (!vc.Visible)
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                    }
                    else
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    }

                    g.DrawRectangle(pen, rect);
                    
                    var iconSize = (from s in iconSizes where s <= Math.Max(rect.Width, rect.Height) / 2 orderby s descending select s).FirstOrDefault();
                    using (Icon ico = new(ICONPATH, iconSize, iconSize))
                    {
                        if (!vc.Pass) { g.DrawIcon(ico, rect.X + VISOFFSET + (rect.Width - iconSize) / 2, rect.Y + VISOFFSET +(rect.Height - iconSize) / 2); };

                    }

                    using (Font f = new Font(FONT, FONTSIZE, FontStyle.Regular, GraphicsUnit.Point))
                    {
                        g.DrawString(string.Concat(vc.VisualType, "\r\n", vc.Name), f, pen.Brush, rect);
                    }
                }
            }
            finally
            {
                //Ico32.Dispose();
                //Ico64.Dispose();
                //Ico128.Dispose();
                g.Dispose();
            }
        }

        public void Save(string fileName)
        {
            _bitmap.Save(fileName, ImageFormat.Png);
        }

        public class PageSize
        {
            public int Height;
            public int Width;
        }

        public class VisualContainer
        {
            public string Name;
            public string VisualType;
            public int X;
            public int Y;
            public int Height;
            public int Width;
            public bool Visible;
            public bool Pass;
        }
    }
}
#pragma warning restore CA1416 // Validate platform compatibility
