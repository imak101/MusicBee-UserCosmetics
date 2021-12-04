using  System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using MusicBeePlugin.Drawing;
using MusicBeePlugin.Form.Popup;
using MusicBeePlugin.Updater.Form;

namespace MusicBeePlugin.Form.Configure
{
    public partial class Form_Configure : System.Windows.Forms.Form
    {
        private string _filePath = string.Empty;
        private string _username = string.Empty;
        private bool _roundPfpChecked;
        private static readonly Size _picBoxSize = new Size(200, 200); 
        private PluginSettings _settings;
        private Plugin.MusicBeeApiInterface _musicBeeApiInterface;
        private System.Timers.Timer _timer = new System.Timers.Timer();
        private SettingsBackup _settingsBackup;
        private bool _disablingGifPanel;
        private bool _timerDrawCheck;
        private int _customGifSpeed;
        private int _originalGifSpeed;
        private bool _anyValueCurrentlyChanged = false;

        
        private Bitmap[] _gifFrames;
        private int _currentGifFrame;
        
        public Form_Configure(ref PluginSettings settings, ref Plugin.MusicBeeApiInterface mbInterface)
        {
            InitializeComponent();
            _musicBeeApiInterface = mbInterface;
            _settings = settings;
            _timer.Elapsed += Timer_Elapsed;
            _settings.ValueChanged += settings_ValueChanged;
        }

        private async void Form_Configure_Load(object sender, EventArgs e)
        {
            if (_settings.GetFromKey("username") == string.Empty || _settings.GetFromKey("pfpPath") == string.Empty)
            {
                goto FetchVersion;
            }

            _filePath = _settings.GetFromKey("pfpPath");
            _username = _settings.GetFromKey("username");
            _roundPfpChecked = Convert.ToBoolean(_settings.GetFromKey("roundPfpCheck"));

            _customGifSpeed = int.Parse(_settings.GetFromKey("customGifSpeed") ?? ((int)numericUpDown_gifSpeed.Value).ToString());
            numericUpDown_gifSpeed.Value = _customGifSpeed;
            _timer.Interval = _customGifSpeed;
            
            _timerDrawCheck = Convert.ToBoolean(_settings.GetFromKey("useTimerDrawing") ?? "false");
            checkBox_useTimerDrawing.Checked = _timerDrawCheck;
            ToggleGifSpeedControls(_timerDrawCheck);
            
            checkBox_roundPfp.Checked = _roundPfpChecked;
            textbox_username.Text = _username;
            
            _settingsBackup = new SettingsBackup(_roundPfpChecked.ToString(), _customGifSpeed.ToString(), _timerDrawCheck.ToString(), ref _settings);
            
            picbox_pfp.Image = ImageHandler();
            
            FetchVersion:
                label_versionInfo.Text = await new Form_Updater().ConfigureFormLabelHandler();
        }

        private void button_submit_Click(object sender, EventArgs e)
        {
            if (textbox_username.Text == string.Empty)
            {
                SystemSounds.Asterisk.Play();
                new Form_Popup("Please enter a username", "Error");
                return;
            }
            if (picbox_pfp.Image == null)
            {
                SystemSounds.Asterisk.Play();
                new Form_Popup("Please select a picture", "Error");
                return;
            }
            
            _username = textbox_username.Text;
            
            _settings.SetFromKey("pfpPath", _filePath);
            _settings.SetFromKey("username",_username);
            
            Close();

            try
            {
                Plugin.FormControlMain.Invalidate();
            }
            catch (NullReferenceException)
            {
                new Form_Popup("Your account has been set! To see it in the application, add the panel 'user cosmetics' to your view with MusicBee's 'Arrange Panels' menu option.", "Set Panel");
            }
        }
        
        
        private void button_cancel_Click(object sender, EventArgs e)
        {
            _settingsBackup.RestoreSettings();
            Close();
        }
        
        private void button_pfp_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = openFileDialog_pfp)
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _filePath = dialog.FileName;

                    using (var picStream = dialog.OpenFile())
                    {
                        try
                        {
                            using (var picBitmap = new Bitmap(picStream)) {}
                        }
                        catch (ArgumentException)
                        {
                            SystemSounds.Asterisk.Play();

                            _filePath = string.Empty;
                            picbox_pfp.Image = null;
                        
                            new Form_Popup("This file is invalid. Your file's dimensions may be too large or not in valid .jpg or .png format.", "Error");
                            return;
                        }
                        
                        if (picStream.Length >= 5000000) new Form_Popup("Images (especially gifs) over 5MB may cause performance issues/crashes.", "Warning");
                        
                        picStream.Close();
                    }

                    _customGifSpeed = -1;

                    picbox_pfp.Image = ImageHandler();
                }
            }
        }
        

        private Image ImageHandler2()
        {
            picbox_pfp.Image?.Dispose();
            try
            {
                using (Image pfp = new Bitmap(_filePath))
                {
                    pfp.Tag = _filePath;
                    
                    if (_roundPfpChecked)
                    {
                        return PaintManager.P_ApplyRoundedCorners(pfp, _picBoxSize.Width, _picBoxSize.Height);
                    }

                    return PaintManager.P_ResizeImage(pfp, _picBoxSize.Width, _picBoxSize.Height);
                }
            }
            catch (FileNotFoundException e)
            {
                new Form_Popup($"The file {e.Message} was not found.", "File not found");
                _settings.SetFromKey("pfpPath", string.Empty);
            }
            catch (ArgumentException)
            {
                new Form_Popup("There was an error loading your profile. Please re-enter your details.", "Error");
            }

            return null;
        }
        
        private Image ImageHandler()
        {
            picbox_pfp.Image?.Dispose();
            try
            {
                using (GifHandler handler = new GifHandler(_filePath, GifHandler.GifScope.Form))
                {
                    handler.Bitmap.Tag = _filePath;
                    // if (_filePath != _settings.GetFromKey("pfpPath")) button_restore.Enabled = true;

                    _timer.Stop();
                    if (handler.IsGif)
                    {
                        checkBox_useTimerDrawing.Enabled = true;
                        
                        _originalGifSpeed = handler.GetFrameDelayMs();
                        numericUpDown_gifSpeed.Value = _customGifSpeed == -1? _originalGifSpeed : _customGifSpeed;
                        
                        ToggleGifSpeedControls(_timerDrawCheck);
                        if (_timerDrawCheck)
                        {
                            if (_gifFrames != null) foreach(Bitmap bitmap in _gifFrames) bitmap.Dispose();
                            _currentGifFrame = 0;
                            _gifFrames = handler.RawFramesResizeGif(_picBoxSize.Width, _picBoxSize.Height);
                            picbox_pfp.Image = null;
                            _timer.Start();
                            return null;
                        }

                        numericUpDown_gifSpeed.Value = _originalGifSpeed;
                        return new Bitmap(handler.ResizeGif(_picBoxSize.Width, _picBoxSize.Height));
                    }
                    else
                    {
                        _disablingGifPanel = true;
                        checkBox_useTimerDrawing.Checked = false;
                        checkBox_useTimerDrawing.Enabled = false;
                        numericUpDown_gifSpeed.Value = 10;
                        ToggleGifSpeedControls(_timerDrawCheck);
                        _disablingGifPanel = false;
                    }
                    
                    if (_roundPfpChecked)
                    {
                        return PaintManager.P_ApplyRoundedCorners(handler.Bitmap, _picBoxSize.Width, _picBoxSize.Height);
                    }

                    return PaintManager.P_ResizeImage(handler.Bitmap, _picBoxSize.Width, _picBoxSize.Height);
                }
            }
            catch (FileNotFoundException e)
            {
                new Form_Popup($"The file {e.Message} was not found.", "File not found");
                _settings.SetFromKey("pfpPath", string.Empty);
            }
            catch (ArgumentException)
            {
                new Form_Popup("There was an error loading your profile. Please re-enter your details.", "Error");
            }

            return null;
        }
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            picbox_pfp.Image = _gifFrames[_currentGifFrame];
            _currentGifFrame++;
            if (_currentGifFrame >= _gifFrames.Length) _currentGifFrame = 0;
        }
        
        private void picBox_Paint(object sender, PaintEventArgs e)
        {
            if (_roundPfpChecked)
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                    gp.AddEllipse(0, 0, picbox_pfp.Size.Width, picbox_pfp.Size.Height);
                    picbox_pfp.Region = new Region(gp);
                    e.Graphics.DrawEllipse(new Pen(new SolidBrush(SystemColors.Menu)), 0, 0, picbox_pfp.Size.Width, picbox_pfp.Size.Height);
                    
                    return;
                }
            }

            picbox_pfp.Region = null;
        }

        public static bool CheckOpened(string name)
        {
            FormCollection fc = Application.OpenForms;

            foreach (System.Windows.Forms.Form frm in fc)
            {
                if (frm.Text == name)
                {
                    return true; 
                }
            }
            return false;
        }

        private void button_about_Click(object sender, EventArgs e)
        {
            if (CheckOpened("About"))
            {
                SystemSounds.Asterisk.Play();
                return;
            }
            
            Plugin.PluginInfo pluginAbout = Plugin.About;
            
            new Form_Popup($"Plugin Title: {pluginAbout.Name}\nAuthor: {pluginAbout.Author}\nVersion: {pluginAbout.VersionToString()}", "About");
        }

        private void checkBox_roundPfp_CheckedChanged(object sender, EventArgs e)
        {
            _settings.SetFromKey("roundPfpCheck", checkBox_roundPfp.Checked.ToString(), true); // TODO: SAFETY KEY, REMOVE NEXT VERSION INCREMENT
            _roundPfpChecked = checkBox_roundPfp.Checked;

            if (_filePath == string.Empty) return;

            picbox_pfp.Image = ImageHandler();
        }

        private void button_updater_Click(object sender, EventArgs e)
        {
            if (CheckOpened("Update Menu"))
            {
                SystemSounds.Asterisk.Play();
                return;
            }
            
            Form_Updater updater = new Form_Updater();

            updater.StartPosition = FormStartPosition.CenterParent;
            
            updater.Show();
        }

        private void Form_Configure_FormClosing(object sender, FormClosingEventArgs e)
        {
            picbox_pfp.Image?.Dispose();
            _timer?.Dispose();
            
            if (new System.Diagnostics.StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close")) return;
            _settingsBackup.RestoreSettings(); // only restore settings on X click 
        }

        private void checkBox_useTimerDrawing_CheckedChanged(object sender, EventArgs e)
        {
            _settings.SetFromKey("useTimerDrawing", checkBox_useTimerDrawing.Checked.ToString(), true); // TODO: safety key
            _timerDrawCheck = checkBox_useTimerDrawing.Checked;

            if (!_timerDrawCheck)
            {
                ToggleGifSpeedControls();
                numericUpDown_gifSpeed.Value = _originalGifSpeed;
            }
            
            if (_disablingGifPanel) return;

            picbox_pfp.Image = ImageHandler();
        }

        private void button_GetCurrentAlbum_Click(object sender, EventArgs e)
        {
            string coverUrl = _musicBeeApiInterface.NowPlaying_GetArtworkUrl.Invoke();
            string newPath = null;
                
            if (Path.GetExtension(coverUrl) == ".tmp")
            {
                newPath = _musicBeeApiInterface.NowPlaying_GetFileTag.Invoke(Plugin.MetaDataType.Album);
                if (newPath.Any(c => Path.GetInvalidFileNameChars().Contains(c))) newPath = $"cover{new Random().Next(1,1000)}.png";
                newPath = Path.Combine(Plugin.About.CoversStorageFolder, newPath);
                File.Copy(coverUrl, newPath, true);
            }
            
            _filePath = newPath ?? coverUrl;
            
            picbox_pfp.Image = ImageHandler();
        }

        private void numericUpDown_gifSpeed_ValueChanged(object sender, EventArgs e)
        {
            _customGifSpeed = (int)decimal.Round(numericUpDown_gifSpeed.Value);
            _settings.SetFromKey("customGifSpeed", ((int)decimal.Round(numericUpDown_gifSpeed.Value)).ToString(), true); //TODO: safety key
            _timer.Interval = (int)decimal.Round(numericUpDown_gifSpeed.Value);
        }

        private void ToggleGifSpeedControls(bool toggleOn = false)
        {
            if (toggleOn)
            {
                label_cutsomGifSpeed.Enabled = true;
                numericUpDown_gifSpeed.Enabled = true;
                button_gifSpeedOriginal.Enabled = true;
                return;
            }
            label_cutsomGifSpeed.Enabled = false;
            numericUpDown_gifSpeed.Enabled = false;
            button_gifSpeedOriginal.Enabled = false;
        }

        private void button_gifSpeedOriginal_Click(object sender, EventArgs e) => numericUpDown_gifSpeed.Value = _originalGifSpeed;

        private void button_restore_Click(object sender, EventArgs e)
        {
            _settingsBackup.RestoreSettings();
            Form_Configure_Load(this, EventArgs.Empty);
            button_restore.Enabled = false;
            Focus();
        }

        private void settings_ValueChanged(object sender, PluginSettings.ValueChangedEventArgs e)
        {
            //if (_settingsBackup != null && !_settingsBackup.ReadOnlyValueDictionary.Keys.All(key => key != e.KeyName)) return; 
            if (_settingsBackup != null && _settingsBackup.ReadOnlyValueDictionary.ContainsKey(e.KeyName) && e.Value != _settingsBackup.ReadOnlyValueDictionary[e.KeyName])
            {
                button_restore.Enabled = true;
                _anyValueCurrentlyChanged = true;
                return;
            }
            // if (_anyValueCurrentlyChanged) return;
            button_restore.Enabled = false;
            _anyValueCurrentlyChanged = false;            
        }
        
        private class SettingsBackup
        {
            public string RoundPfpCheck { get; }
            public string CustomGifSpeed { get; }
            public string UseTimerDrawing { get; }
            public ReadOnlyDictionary<string, string> ReadOnlyValueDictionary { get; } 
            
            private PluginSettings _settings;
            
            public SettingsBackup(string roundPfpCheck, string customGifSpeed, string useTimerDrawing, ref PluginSettings pluginSettings)
            {
                RoundPfpCheck = roundPfpCheck;
                CustomGifSpeed = customGifSpeed;
                UseTimerDrawing = useTimerDrawing;
                _settings = pluginSettings;
                ReadOnlyValueDictionary = new ReadOnlyDictionary<string,string>(new Dictionary<string,string>
                {
                    {"roundPfpCheck", roundPfpCheck}, 
                    {"customGifSpeed", customGifSpeed}, 
                    {"useTimerDrawing", useTimerDrawing}
                });
            }

            public void RestoreSettings()
            {
                _settings.SetFromKey("roundPfpCheck", RoundPfpCheck, true);
                _settings.SetFromKey("customGifSpeed", CustomGifSpeed, true);
                _settings.SetFromKey("useTimerDrawing",UseTimerDrawing, true);
            }
        }
    }
}