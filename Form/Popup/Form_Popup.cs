using System;
using System.Windows.Forms;

namespace MusicBeePlugin.Form.Popup
{
    public partial class Form_Popup : System.Windows.Forms.Form
    {
        private string _msg;
        private string _title;
        
        public Form_Popup(string msg, string title)
        {
            InitializeComponent();
            _msg = msg;
            _title = title;
            
            Show();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form_Popup_Load(object sender, EventArgs e)
        {
            Text = _title;
            label_main.Text = _msg;
        }
    }
}