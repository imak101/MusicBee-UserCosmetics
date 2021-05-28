using System;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace MusicBeePlugin
{
    public partial class Form_Configure : Form
    {
        private string _filePath = string.Empty;
        private string _fileName = string.Empty;

        public Form_Configure()
        {
            InitializeComponent();
        }

        private void Form_Configure_Load(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void button_submit_Click(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void button_pfp_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = openFileDialog_pfp)
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _filePath = dialog.FileName;
                    _fileName = dialog.SafeFileName;
                    
                    var picStream = dialog.OpenFile();
                    
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
                        
                        throw new Exception("This file is invalid. Your file's dimensions may be too large or not in valid .jpg or .png form.");
                    }
                }

            }
        }

        private void button_pfp_DragDrop(object sender, DragEventArgs e)
        {
            throw new System.NotImplementedException();
        }
        
    }
}