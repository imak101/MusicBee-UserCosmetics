using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using MusicBeePlugin.Form.Popup;

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
        

        public Image[] MakeGifArray()
        {
            Image[] gifFrames = new Image[_frameCount];

            for (int i = 0; i < gifFrames.Length; i++)
            {
                _gif.SelectActiveFrame(_dimension, i);
                gifFrames[i] = new Bitmap(_gif) ;
            }
            
            return gifFrames;
        }

        public string ResizeGif(int width, int height)
        {
            Image[] gifFrames = MakeGifArray();

            foreach (Image bmp in gifFrames)
            {
                
            }

        }
        

/// <returns>Path to new gif</returns>
        private string ReassembleGif(Image[] gifFrames)
        {
            // Gdi+ constants absent from System.Drawing.
            const int PropertyTagFrameDelay = 0x5100;
            const int PropertyTagLoopCount = 0x5101;
            const short PropertyTagTypeLong = 4;
            const short PropertyTagTypeShort = 3;

            const int UintBytes = 4;
            
//...
            var gifEncoder = GetEncoder(ImageFormat.Gif);
// Params of the first frame.
            var encoderParams1 = new EncoderParameters(1);
            encoderParams1.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
// Params of other frames.
            var encoderParamsN = new EncoderParameters(1);
            encoderParamsN.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionTime);
// Params for the finalizing call.
            var encoderParamsFlush = new EncoderParameters(1);
            encoderParamsFlush.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush);

// PropertyItem for the frame delay (apparently, no other way to create a fresh instance).
            var frameDelay = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
            frameDelay.Id = PropertyTagFrameDelay;
            frameDelay.Type = PropertyTagTypeLong;
// Length of the value in bytes.
            frameDelay.Len = gifFrames.Length * UintBytes;
// The value is an array of 4-byte entries: one per frame.
// Every entry is the frame delay in 1/100-s of a second, in little endian.
            var delay = BitConverter.ToInt32(_gif.GetPropertyItem(20736).Value, 0);
            frameDelay.Value = new byte[_frameCount * 4];
// E.g., here, we're setting the delay of every frame to 1 second.

            var frameDelayBytes = BitConverter.GetBytes((uint)delay);
            for (int j = 0; j < gifFrames.Length; ++j)
                try
                {
                    Array.Copy(frameDelayBytes, 0, frameDelay.Value, j * UintBytes, UintBytes);
                }
                catch (Exception)
                {
                    break;
                }

// PropertyItem for the number of animation loops.
            var loopPropertyItem = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
            loopPropertyItem.Id = PropertyTagLoopCount;
            loopPropertyItem.Type = PropertyTagTypeShort;
            loopPropertyItem.Len = 0;
// 0 means to animate forever.
            loopPropertyItem.Value = BitConverter.GetBytes((ushort)0);

            string filePath = Path.Combine(Plugin.About.PersistentStoragePath, "processed.gif");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                bool first = true;
                Bitmap firstBitmap = null;
                // Bitmaps is a collection of Bitmap instances that'll become gif frames.
                foreach (var bitmap in gifFrames)
                {
                    if (first)
                    {
                        firstBitmap = (Bitmap) bitmap;
                        firstBitmap.SetPropertyItem(frameDelay);
                        firstBitmap.SetPropertyItem(loopPropertyItem);
                        firstBitmap.Save(stream, gifEncoder, encoderParams1);
                        first = false;
                    }
                    else
                    {
                        firstBitmap.SaveAdd(bitmap, encoderParamsN);
                    }
                }

                firstBitmap?.SaveAdd(encoderParamsFlush);
                firstBitmap?.Dispose();

                return filePath;
            }

            
            ImageCodecInfo GetEncoder(ImageFormat format)
            {
                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.FormatID == format.Guid)
                    {
                        return codec;
                    }
                }
                return null;
            }
        }
    }
}