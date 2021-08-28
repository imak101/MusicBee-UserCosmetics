using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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


        public Form_Configure(ref PluginSettings settings, ref Plugin.MusicBeeApiInterface mbInterface)
        {
            InitializeComponent();
            _musicBeeApiInterface = mbInterface;
            _settings = settings;
        }

        private async void Form_Configure_Load(object sender, EventArgs e)
        {
            if (_settings.GetFromKey("username") == string.Empty || _settings.GetFromKey("pfpPath") == string.Empty)
            {
                return;
            }
            _filePath = _settings.GetFromKey("pfpPath");
            _username = _settings.GetFromKey("username");
            _roundPfpChecked = Convert.ToBoolean(_settings.GetFromKey("roundPfpCheck"));

            checkBox_roundPfp.Checked = _roundPfpChecked;
            textbox_username.Text = _username;
            
            picbox_pfp.Image = ImageHandler();

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
                new Form_Popup("Your account has been set! To see it in the application, add the panel 'user account' to your view with MusicBee's 'Arrange Panels' menu option.", "Set Panel");
            }
        }
        
        
        private void button_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        private void button_pfp_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = openFileDialog_pfp)
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _filePath = dialog.FileName;
                    
                    var picStream = dialog.OpenFile();

                    try
                    {
                        using (var picBitmap = new Bitmap(picStream))
                        {
                            picBitmap.Tag = _filePath;
                        }
                    }
                    catch (ArgumentException)
                    {
                        SystemSounds.Asterisk.Play();

                        _filePath = string.Empty;
                        picbox_pfp.Image = null;
                        
                        new Form_Popup("This file is invalid. Your file's dimensions may be too large or not in valid .jpg or .png format.", "Error");
                        return;
                    }

                    picbox_pfp.Image = ImageHandler();
                }
            }
        }

        private Image ImageHandler()
        {
            try
            {
                using (Bitmap pfp = new Bitmap(_filePath))
                {
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
    }
}