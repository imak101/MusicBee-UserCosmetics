using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;

namespace MusicBeePlugin.Drawing
{
    public class PaintManager
    {
        private PaintEventArgs _eventArgs;
        private readonly Plugin.MusicBeeApiInterface _mbAPI;
        private readonly PluginSettings _settings;

        private const string PROFILE_NOT_SET_ERR_MSG = "Please configure a user profile.";

        private static readonly Size _pfpSize = new Size(60,60);

        private Color _bgColor;
        private Color _fgColor;

        private string _pfpPath;
        private string _username;
        private string _oldPfpPath;

        private int _customGifSpeed;
        
        private Point _usernamePoint; //= new Point(105,83);     < -- Fallback values
        private Point _pfpPoint; //= new Point(100, 20);

        private Image _pfp;

        private Control _controlMain;

        private PictureBox _picBox;

        private bool _drawRounded;
        private bool _useTimerDrawing;
        private bool _oldUseTimerDrawing;
        
        private static System.Timers.Timer _timer = new System.Timers.Timer();
        private static Bitmap[] _gifFrames;
        private static int _currentGifFrame;
        
        public PaintManager(ref Plugin.MusicBeeApiInterface mbAPI, ref PluginSettings settings)
        {
            _mbAPI = mbAPI;
            _settings = settings;
            _timer.Elapsed += Timer_Elapsed;

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
                _drawRounded = bool.Parse(_settings.GetFromKey("roundPfpCheck"));
                _customGifSpeed = int.Parse(_settings.GetFromKey("customGifSpeed"));
                _useTimerDrawing = bool.Parse(_settings.GetFromKey("useTimerDrawing"));
            }
            catch (Exception) 
            {
                _settings.SetFromKey("roundPfpCheck", _drawRounded.ToString() ?? "False", true); //TODO: safety key
                _settings.SetFromKey("customGifSpeed", 10.ToString(), true);
                _settings.SetFromKey("useTimerDrawing", true.ToString(), true);
                _drawRounded = false;
                _customGifSpeed = 10;
                _useTimerDrawing = false;
            }
        }

        public void MainPainter()
        {
            _oldPfpPath = _pfpPath;
            _oldUseTimerDrawing = _useTimerDrawing;
            LoadSettings();

            _eventArgs.Graphics.Clear(_bgColor);

            CalculateCenter_Point();
            
            TextRenderer.DrawText(_eventArgs.Graphics, _username==string.Empty? PROFILE_NOT_SET_ERR_MSG : _username, SystemFonts.CaptionFont, _usernamePoint, _fgColor);
            
            _picBox.Image = ImageHandler(); // Make async somehow 
        }
        
        private Image ImageHandler()
        {
            string currentPath = _pfpPath;
            _picBox.Image?.Dispose();

            using (GifHandler handler = new GifHandler(currentPath, GifHandler.GifScope.MainPanel))
            {
                _picBox.Invalidate();
                _timer.Stop();
                if (currentPath == null || _pfp == null || _pfpPath != _oldPfpPath || _useTimerDrawing != _oldUseTimerDrawing)
                {
                    if (handler.IsGif)
                    {
                        if (_gifFrames != null) foreach(Bitmap bitmap in _gifFrames) bitmap.Dispose();
                        
                        if (_useTimerDrawing)
                        {
                            _currentGifFrame = 0;
                            _gifFrames = handler.RawFramesResizeGif(_picBox.Size.Width, _picBox.Size.Height);
                            _picBox.Image = null;
                            _timer.Interval = _customGifSpeed;
                            _timer.Start();
                            return _pfp = null;
                        }
                        
                        _pfp = new Bitmap(handler.ResizeGif(_pfpSize.Width, _pfpSize.Height));
                        return _pfp;
                    }

                    _pfp = ResizeImage(Image.FromFile(_pfpPath), _pfpSize.Width, _pfpSize.Height);
                }
            }
            return _pfp;
        }
        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            
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
            
            image.Dispose();
            return destImage;
        }
        

        public static Image P_ResizeImage(Image image, int width, int height)
        {
            if (ImageAnimator.CanAnimate(image) && (string)image.Tag != "gifFrame")
            {
                using (GifHandler handler = new GifHandler((string)image.Tag, GifHandler.GifScope.Form))
                {
                    return new Bitmap(handler.ResizeGif(width,height));
                }
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
            
            image.Dispose();
            return destImage;
        }

        public static Image P_ApplyRoundedCorners(Image image, int width, int height, bool useMenuColor = true)
        {
            if (ImageAnimator.CanAnimate(image) && (string)image.Tag != "gifFrame")
            {
                using (GifHandler handler = new GifHandler((string)image.Tag, GifHandler.GifScope.Form, true))
                {
                    return new Bitmap(handler.ResizeAndRoundGifCorners(width, height));
                }
            }
            
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

                        graphics.FillRectangle(!useMenuColor ? new SolidBrush(Color.FromArgb(Plugin._mbApiInterface.Setting_GetSkinElementColour.Invoke(Plugin.SkinElement.SkinSubPanel, Plugin.ElementState.ElementStateDefault, Plugin.ElementComponent.ComponentBackground))) : new SolidBrush(SystemColors.Menu), plaster);

                        texture.TranslateTransform(xAbsolutePixel.X, xAbsolutePixel.Y);
                        
                        path.AddEllipse(plaster);
                        graphics.FillEllipse(texture, plaster);

                        path.CloseFigure();
                        
                        targetBmp.Tag = image.Tag;
                        pfpBmp.Dispose();

                        return targetBmp;
                    }
                }
            }
        }

        private bool _isPicBoxInitialized = false;
        public void MakePicBox()
        {
            if (_isPicBoxInitialized) return;
            
            _controlMain = Plugin.FormControlMain;

            void PicBoxMake()
            {
                _picBox = new PictureBox { Parent = _controlMain, Name = "picBox" };
                _controlMain.Controls.Add(_picBox);
            }

            _controlMain.Invoke((Action)PicBoxMake);
            _picBox.Paint += picBox_Paint;
            _isPicBoxInitialized = true;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _picBox.Image = _gifFrames[_currentGifFrame];
            _currentGifFrame++;
            if (_currentGifFrame >= _gifFrames.Length) _currentGifFrame = 0;
        }

        private void picBox_Paint(object sender, PaintEventArgs e)
        {
            if (_drawRounded)
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                    gp.AddEllipse(0, 0, _picBox.Size.Width, _picBox.Size.Height);
                    _picBox.Region = new Region(gp);
                    e.Graphics.DrawEllipse(new Pen(new SolidBrush(_bgColor)), 0, 0, _picBox.Size.Width, _picBox.Size.Height);
                    
                    return;
                }
            }

            _picBox.Region = null;
        }

        private void CalculateCenter_Point()
        {
            _picBox = _picBox ?? (PictureBox) _controlMain.Controls["picBox"];
            
            if (_pfp == null || !_timer.Enabled)
            {
                _pfp = !_timer.Enabled && _pfpPath != string.Empty ? ResizeImage(Image.FromFile(_pfpPath), _pfpSize.Width,_pfpSize.Height) : new Bitmap(_pfpSize.Width, _pfpSize.Height);
            }
            
            _pfpPoint.X = _controlMain.Size.Width / 2 - _pfp.Width / 2;
            _pfpPoint.Y = _controlMain.Size.Height / 2 - _pfp.Height / 2;

            _usernamePoint.X = Convert.ToInt32((_controlMain.Size.Width - _eventArgs.Graphics.MeasureString(string.IsNullOrEmpty(_username) ? PROFILE_NOT_SET_ERR_MSG : _username, SystemFonts.CaptionFont).Width) / 2); // calculate text center relative to text and control size
            _usernamePoint.Y = (_controlMain.Height / 2) + _pfp.Height / 2 + 3;
            
            _picBox.Location = _pfpPoint;
            _picBox.Size = _pfpSize;
            _picBox.SizeMode = PictureBoxSizeMode.Zoom;

            _pfp = null;
        }
    }
}