using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MusicBeePlugin
{
    public class GifHandler
    {
        private Image _gif;
        private FrameDimension _dimension;
        private int _frameCount;
        
        public GifHandler(string gifPath)
        {
            _gif = new Bitmap(gifPath);
            _dimension = new FrameDimension(_gif.FrameDimensionsList[0]);
            _frameCount = _gif.GetFrameCount(_dimension);
        }
        
        public Image ResizeGif()
        {
            // using (Stream resizeStream = new MemoryStream())
            // {
            //     _gif.Save(resizeStream, ImageFormat.Gif);
            //     
            //     
            //
            //     new Bitmap(96, 96);
            //     EncoderParameters encoderParam = new EncoderParameters();
            //     encoderParam.Param[0] = new EncoderParameter(Encoder.SaveFlag, 1);
                Image newGif = (Image)_gif.Clone();
                
                for (int i = 0; i < _frameCount; i++)
                {
                    _gif.Tag = "gifRun"; // to avoid infinite recursion
                    
                    _gif.SelectActiveFrame(_dimension, i);
                    newGif.SelectActiveFrame(_dimension, i);

                    newGif = PaintManager.P_ResizeImage(_gif, 96, 96);
                
                    //_gif.SaveAdd(encoderParam);
                }

                newGif.SelectActiveFrame(_dimension, 0);
                
                return newGif;
                //}
        }

        public Image Resize2()
        {
            //for (int i = 0; i < _frameCount; i++)
            //{
                _gif.Tag = "gifRun"; // to avoid infinite recursion

                _gif.SelectActiveFrame(_dimension, 3);
            

                // Image copy = (Image)_gif.Clone();
                // copy.Tag = "gifRun";
                //
                // var x = PaintManager.P_ApplyRoundedCorners( copy, 60, 60);
                //
                // _gif = x;
                //
                // Debug.Assert(ImageAnimator.CanAnimate(_gif));
                
                return new Bitmap(_gif);
                //}

        }
    }
}