using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using MusicBeePlugin.Form.Popup;

namespace MusicBeePlugin
{
    public class GifHandler
    {
        private Image _gif;
        private FrameDimension _dimension;
        private int _frameCount;
        private string _gifPath;
        
        private bool _roundCorners;
        
        private GifScope _scope;

        public static string GifPathMainPanel { get; private set; }
        public static string GifPathForm { get; private set; }
        
        public static List<string> OldFileNamesForDeletion = new List<string>();
        
        public GifHandler(string gifPath, GifScope scope, bool roundCorners = false)
        {
            _gif = new Bitmap(gifPath);
            _dimension = new FrameDimension(_gif.FrameDimensionsList[0]);
            _frameCount = _gif.GetFrameCount(_dimension);
            _roundCorners = roundCorners;
            _scope = scope;
            _gifPath = gifPath;
        }
        

        /// <summary>Used in to specify the usage scope of the gif</summary>
        public enum GifScope
        {
            Form = 0,
            MainPanel = 1
            
        }

        private Bitmap[] MakeGifArray()
        {
            Bitmap[] gifFrames = new Bitmap[_frameCount];

            for (int i = 0; i < gifFrames.Length; i++)
            {
                _gif.SelectActiveFrame(_dimension, i);
                gifFrames[i] = new Bitmap(_gif);
            }
            
            return gifFrames;
        }

        public string ResizeGif(int width, int height)
        {
            Image[] gifFrames = MakeGifArray();

            for (int i = 0; i < gifFrames.Length; i++)
            {
                gifFrames[i].Tag = "gifFrame";
                gifFrames[i] = PaintManager.P_ResizeImage(gifFrames[i], width, height);
            }

            return ReassembleGif(gifFrames);
        }

        public string ResizeAndRoundGifCorners(int width, int height)
        {
            Image[] gifFrames = MakeGifArray();

            for (int i = 0; i < gifFrames.Length; i++)
            {
                gifFrames[i].Tag = "gifFrame";
                gifFrames[i] = PaintManager.P_ApplyRoundedCorners(gifFrames[i], width, height, _scope == GifScope.Form);
            }

            return ReassembleGif(gifFrames);
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

            string filePath = Path.Combine(SwitchPath(_scope),  $"{FilePathRNG().ToString()}processed.gif");

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
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
                }
            }
            catch(IOException)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            

            return filePath;
            

            
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

        public string Reassemble2()
        {
            using (FileStream gifStream = new FileStream(_gifPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                string filePath = Path.Combine(SwitchPath(_scope),  $"{FilePathRNG().ToString()}processed.gif");
                
                GifBitmapDecoder decoder = new GifBitmapDecoder(gifStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                
                GifBitmapEncoder encoder = new GifBitmapEncoder();
                encoder.Frames = Resize2();
                
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    byte[] fileBytes = ms.ToArray();
                    byte[] applicationExtension = { 33, 255, 11, 78, 69, 84, 83, 67, 65, 80, 69, 50, 46, 48, 3, 1, 0, 0, 0 };
                    
                    List<byte> newBytes = new List<byte>();
                    
                    newBytes.AddRange(fileBytes.Take(13));
                    newBytes.AddRange(applicationExtension);
                    newBytes.AddRange(fileBytes.Skip(13));
                    
                    WriteGraphicControlBlock(ref newBytes);
                    
                    File.WriteAllBytes( filePath, newBytes.ToArray());
                    return filePath;
                }
                
                
            }
        }

        private List<BitmapFrame> Resize2()
        {
            Bitmap[] gifFrames = MakeGifArray();
            List<BitmapFrame> bitmapFrames = new List<BitmapFrame>();

            for (int i = 0; i < gifFrames.Length; i++)
            {
                gifFrames[i].Tag = "gifFrame";
                gifFrames[i] = (Bitmap)PaintManager.P_ApplyRoundedCorners(gifFrames[i], 200, 200, _scope == GifScope.Form);
                
                bitmapFrames.Add(BitmapFrame.Create(Imaging.CreateBitmapSourceFromHBitmap(gifFrames[i].GetHbitmap(), IntPtr.Zero, new Int32Rect(0,0, 60,60), BitmapSizeOptions.FromEmptyOptions())));

            }
            
            return bitmapFrames;
        }

        private void WriteGraphicControlBlock(ref List<byte> byteList)
        {
            byte[] frameDelay = GetFrameDelay();
            
            for (int i = 0; i < byteList.Count; i++)
            {
                if (byteList[i] != 33) continue;

                if (byteList[i + 1] == 249 && byteList[i + 7] == 0)
                {
                    byteList[i + 3] = TryTransparency();

                    byteList[i + 4] = frameDelay[0];
                    byteList[i + 5] = frameDelay[1];
                }
            }
        }

        private byte[] GetFrameDelay()
        { 
            byte[] delay = _gif.GetPropertyItem(20736).Value.Take(4).ToArray();
            
            string byte1 = delay[1].ToString() + delay[0].ToString();
            string byte2 = delay[2].ToString() + delay[3].ToString();
            
            return new[] { byte.Parse(byte1), byte.Parse(byte2) };
        }

        private byte TryTransparency()
        {
            try { _gif.GetPropertyItem(20740); }
            catch (ArgumentException) { return 4; }
            
            return 5;
        }

        public static void InitiateGifDirectories()
        {
            GifPathForm = Directory.CreateDirectory(Path.Combine(Plugin.About.PersistentStorageFolder, "Form\\")).FullName;
            GifPathMainPanel = Directory.CreateDirectory(Path.Combine(Plugin.About.PersistentStorageFolder, "MainPanel\\")).FullName;
        }

        private static string SwitchPath(GifScope scope)
        {
            switch (scope)
            {
                case GifScope.Form: return GifPathForm;
                case GifScope.MainPanel: return GifPathMainPanel;
                default: throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }
        }
        
        private int FilePathRNG()
        {
            List<int> usedNum = new List<int>();
            
            DeleteFilesInList(_scope);

            foreach (int usedNumber in PopulateFileList(_scope))
            {
                usedNum.Add(usedNumber);
            }
            
            if (usedNum.Count == 0) return 1;
            
            Random random = new Random();
            int randInt = random.Next(0,10);
            for (int i = 0; i < usedNum.Count; i++)
            {
                if (randInt == usedNum[i])
                {
                    randInt = random.Next(0, 10);
                    i = 0;
                }
            }

            return randInt;
        }
        
        /// <param name="scope">If null, delete contents in ALL gif folders, else, delete given</param>
        public static void DeleteFilesInList(GifScope? scope)
        {
            if (OldFileNamesForDeletion.Count == 0)
            {
                foreach (GifScope gifScope in (GifScope[]) Enum.GetValues(typeof(GifScope)))
                {
                    if (scope != null)
                    {
                        PopulateFileListVoid(scope.Value);
                        break;
                    } 
                    
                    PopulateFileListVoid(gifScope);
                }

            }
            
            foreach (string fileName in OldFileNamesForDeletion.ToArray())
            {
                try
                {
                    File.Delete(fileName);
                    OldFileNamesForDeletion.Remove(fileName);
                }
                catch (IOException)
                {
                    
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }
        
        /// <returns>yield returns file numbers that are being used</returns>
        private static IEnumerable<int> PopulateFileList(GifScope scope)
        {
            foreach (string fileName in Directory.GetFiles(SwitchPath(scope)))
            {
                Match regexMatch = Regex.Match(fileName, @"\dprocessed");
                if (regexMatch.Success)
                {
                    OldFileNamesForDeletion.Add(fileName);
                    yield return int.Parse(regexMatch.Value[0].ToString());
                }
            }
        }

        private static void PopulateFileListVoid(GifScope scope)
        {
            foreach (var VARIABLE in PopulateFileList(scope));
        }
    }
}