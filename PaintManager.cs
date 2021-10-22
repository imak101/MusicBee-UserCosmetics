using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MusicBeePlugin.Form.Popup;


namespace MusicBeePlugin
{
    public class PaintManager
    {
        private PaintEventArgs _eventArgs;
        private readonly Plugin.MusicBeeApiInterface _mbAPI;
        private readonly PluginSettings _settings;

        private static readonly Size _pfpSize = new Size(60,60);

        private Color _bgColor;
        private Color _fgColor;

        private string _pfpPath;
        private string _username;
        private string _oldPfpPath;

        private Point _usernamePoint; //= new Point(105,83);     < -- Fallback values
        private Point _pfpPoint; //= new Point(100, 20);

        private Image _pfp;

        private Control _controlMain;

        private PictureBox _picBox;

        private bool _drawRounded;
        
        public PaintManager(ref Plugin.MusicBeeApiInterface mbAPI, ref PluginSettings settings)
        {
            _mbAPI = mbAPI;
            _settings = settings;

            SetColorsFromSkin();
            LoadSettings();

        }

        public void SetArgs(ref PaintEventArgs args)
        {
            _eventArgs = args;
        }

        private void SetColorsFromSkin()
        {
            _bgColor = Color.FromArgb(_mbAPI.Setting_GetSkinElementColour.Invoke(Plugin.SkinElement.SkinSubPanel,Plugin.ElementState.ElementStateDefault, Plugin.ElementComponent.ComponentBackground)); 
            _fgColor = Color.FromArgb(_mbAPI.Setting_GetSkinElementColour.Invoke(Plugin.SkinElement.SkinSubPanel,Plugin.ElementState.ElementStateDefault,Plugin.ElementComponent.ComponentForeground));
        }

        private void LoadSettings()
        {
            _pfpPath = _settings.GetFromKey("pfpPath");
            _username = _settings.GetFromKey("username");

            try
            {
                _drawRounded = Convert.ToBoolean(_settings.GetFromKey("roundPfpCheck"));
            }
            catch (FormatException)
            {
                _settings.SetFromKey("roundPfpCheck", false.ToString(), true); //TODO: safety key
                _drawRounded = false;
            }
        }

        public void MainPainter()
        {
            _oldPfpPath = _pfpPath;
            LoadSettings();

            _eventArgs.Graphics.Clear(_bgColor);

            CalculateCenter_Point();
            
            TextRenderer.DrawText(_eventArgs.Graphics, _username, SystemFonts.CaptionFont, _usernamePoint, _fgColor);
            //_eventArgs.Graphics.DrawImage(ImageHandler(),_pfpPoint);
            _picBox.Image = ImageHandler();
        }

        private Image ImageHandler()
        {
            string currentPath = _pfpPath;

            if (ImageAnimator.CanAnimate(new Bitmap(_pfpPath)))
            {
                return new Bitmap(_pfpPath);
            }

            if (currentPath == null || _pfp == null || _pfpPath != _oldPfpPath || !_drawRounded)
            {
                _pfp = ResizeImage(Image.FromFile(_pfpPath), _pfpSize.Width, _pfpSize.Height);
            }

            if (_drawRounded)
            {
                if ((string)_pfp.Tag != currentPath) _pfp = ApplyRoundCorners();
            }
            
            return _pfp;
        }
        
        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            
            //destImage.SetResolution(image.HorizontalResolution >= 72? 95 : image.HorizontalResolution, image.VerticalResolution >= 72? 95 : image.VerticalResolution);
            //destImage.SetResolution(image.HorizontalResolution,image.VerticalResolution); //TODO: Remove if above solution suffices
            destImage.SetResolution(96,96);
            
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
        
            return destImage;
        }

        private Bitmap ApplyRoundCorners()
        {
            Rectangle plaster = new Rectangle(0, 0, _pfpSize.Width, _pfpSize.Height);
            Bitmap pfpBmp = ResizeImage(Image.FromFile(_pfpPath), _pfpSize.Width,_pfpSize.Height);
            Bitmap targetBmp = new Bitmap(_pfpSize.Width, _pfpSize.Height);

            
            Point pPlasterCenterRelative = new Point(plaster.Width / 2, plaster.Height / 2);
            Point pImageCenterRelative = new Point(pfpBmp.Width / 2, pfpBmp.Height / 2);
            Point pOffSetRelative = new Point(pPlasterCenterRelative.X - pImageCenterRelative.X, pPlasterCenterRelative.Y - pImageCenterRelative.Y);

            Point xAbsolutePixel = pOffSetRelative + new Size(plaster.Location); //Find the absolute location

            using (Graphics graphics = Graphics.FromImage(targetBmp))
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    using (TextureBrush texture = new TextureBrush(pfpBmp, WrapMode.Clamp))
                    {
                        graphics.CompositingMode = CompositingMode.SourceOver;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        
                        graphics.FillRectangle(new SolidBrush(_bgColor), plaster);

                        texture.TranslateTransform(xAbsolutePixel.X, xAbsolutePixel.Y);
                        
                        path.AddEllipse(plaster);
                        graphics.FillEllipse(texture, plaster);

                        path.CloseFigure();
                        
                        targetBmp.Tag = _pfpPath;

                        return targetBmp;
                    }
                }
            }
        }

        public static Image P_ResizeImage(Image image, int width, int height)
        {
            if (ImageAnimator.CanAnimate(image) && (string)image.Tag != "gifRun")
            {
                
                //return new Bitmap((string) image.Tag);
                GifHandler handler = new GifHandler((string)image.Tag);

                //Image[] dd = handler.MakeGifArray();

                //new Form_Popup(dd.Length.ToString(), "dd");

                Bitmap ss = new Bitmap(handler.ReassembleGif());

                return ss;

                //return after;
            }
            
            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap destImage = new Bitmap(width, height);
            
            destImage.SetResolution(image.HorizontalResolution >= 72? 95 : image.HorizontalResolution, image.VerticalResolution >= 72? 95 : image.VerticalResolution);
            
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            destImage.Tag = image.Tag;
            
            return destImage;
        }

        public static Image P_ApplyRoundedCorners(Image image, int width, int height)
        {
            Rectangle plaster = new Rectangle(0, 0, width, height);
            Image pfpBmp = P_ResizeImage(image, width, height);
            Bitmap targetBmp = new Bitmap(width, height);

            
            Point pPlasterCenterRelative = new Point(plaster.Width / 2, plaster.Height / 2);
            Point pImageCenterRelative = new Point(pfpBmp.Width / 2, pfpBmp.Height / 2);
            Point pOffSetRelative = new Point(pPlasterCenterRelative.X - pImageCenterRelative.X, pPlasterCenterRelative.Y - pImageCenterRelative.Y);

            Point xAbsolutePixel = pOffSetRelative + new Size(plaster.Location); //Find the absolute location

            using (Graphics graphics = Graphics.FromImage(targetBmp))
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    using (TextureBrush texture = new TextureBrush(pfpBmp, WrapMode.Clamp))
                    {
                        graphics.CompositingMode = CompositingMode.SourceOver;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        
                        graphics.FillRectangle(new SolidBrush(SystemColors.Menu), plaster);

                        texture.TranslateTransform(xAbsolutePixel.X, xAbsolutePixel.Y);
                        
                        path.AddEllipse(plaster);
                        graphics.FillEllipse(texture, plaster);

                        path.CloseFigure();
                        
                        targetBmp.Tag = image.Tag;

                        return targetBmp;
                    }
                }
            }
        }

        public void MakePicBox()
        {
            _controlMain = Plugin.FormControlMain;

            void PicBoxMake()
            {
                _picBox = new PictureBox { Parent = _controlMain, Name = "picBox" };
                _controlMain.Controls.Add(_picBox);
            }

            _controlMain.Invoke((Action)PicBoxMake);
        }

        private void CalculateCenter_Point()
        {
            _picBox = (PictureBox) _controlMain.Controls["picBox"];
            
            if (_pfp == null)
            {
                _pfp = ResizeImage(Image.FromFile(_pfpPath), _pfpSize.Width,_pfpSize.Height);
            }
            
            _pfpPoint.X = _controlMain.Size.Width / 2 - _pfp.Width / 2;
            _pfpPoint.Y = _controlMain.Size.Height / 2 - _pfp.Height / 2;

            _usernamePoint.X = Convert.ToInt32((_controlMain.Size.Width - _eventArgs.Graphics.MeasureString(_username, SystemFonts.CaptionFont).Width) / 2); // calculate text center relative to text and control size
            _usernamePoint.Y = (_controlMain.Height / 2) + _pfp.Height / 2 + 3;

            _picBox.Location = _pfpPoint;
            _picBox.Size = _pfpSize;
            _picBox.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}