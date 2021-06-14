using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using MusicBeePlugin.Form.Popup;

namespace MusicBeePlugin.Form.Configure
{
    public partial class Form_Configure : System.Windows.Forms.Form
    {
        private string _filePath = string.Empty;
        private string _fileName = string.Empty;
        private string _username = string.Empty;
        private PluginSettings _settings;


        public Form_Configure(ref PluginSettings settings)
        {
            InitializeComponent();
            _settings = settings;
        }

        private void Form_Configure_Load(object sender, EventArgs e)
        {
            if (_settings.GetFromKey("username") == string.Empty)
            {
                return;
            }
            _filePath = _settings.GetFromKey("pfpPath");
            _username = _settings.GetFromKey("username");

            textbox_username.Text = _username;
            picbox_pfp.Image = Image.FromFile(_filePath);
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
                    _fileName = dialog.SafeFileName;
                    
                    var picStream = dialog.OpenFile(); // ONLY PictureBoxSizeMode.Zoom WORKS

                    try
                    {
                        using (var picBitmap = new Bitmap(picStream))
                        {
                            picbox_pfp.Image = Image.FromHbitmap(picBitmap.GetHbitmap());
                        }
                    }
                    catch (ArgumentException)
                    {
                        SystemSounds.Asterisk.Play();

                        _filePath = string.Empty;
                        _fileName = string.Empty;

                        picbox_pfp.Image = null;
                        
                        new Form_Popup("This file is invalid. Your file's dimensions may be too large or not in valid .jpg or .png format.", "Error");
                    }
                }
            }
        }

        private void button_pfp_DragDrop(object sender, DragEventArgs e)
        {
            throw new System.NotImplementedException();
        }
        
        public bool CheckOpened(string name)
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
    }
}