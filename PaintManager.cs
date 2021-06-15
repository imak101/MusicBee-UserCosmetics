using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;


namespace MusicBeePlugin
{
    public class PaintManager
    {
        private PaintEventArgs _eventArgs;
        private Plugin.MusicBeeApiInterface _mbAPI;
        private PluginSettings _settings;

        private Color _bgColor;
        private Color _fgColor;

        private string _pfpPath;
        private string _username;
        
        public PaintManager(ref Plugin.MusicBeeApiInterface mbAPI, ref PluginSettings settings)
        {
            _mbAPI = mbAPI;
            _settings = settings;
            
            SetColorsFromSkin();
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

        private void SetCredentials()
        {
            _pfpPath = _settings.GetFromKey("pfpPath");
            _username = _settings.GetFromKey("username");
        }

        public void test()
        {
            SetCredentials();
            
            
            _eventArgs.Graphics.Clear(_bgColor);
            TextRenderer.DrawText(_eventArgs.Graphics, _username, SystemFonts.CaptionFont, new Point(10,10), _fgColor);
        }
        
        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
        
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
        
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
        
            return destImage;
        }
    }
}