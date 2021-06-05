using System.ComponentModel;

namespace MusicBeePlugin.Form.Popup
{
    partial class Form_Popup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label_main = new System.Windows.Forms.Label();
            this.button_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_main
            // 
            this.label_main.Location = new System.Drawing.Point(64, 9);
            this.label_main.Name = "label_main";
            this.label_main.Size = new System.Drawing.Size(213, 60);
            this.label_main.TabIndex = 0;
            this.label_main.Text = "Popup";
            this.label_main.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(132, 86);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(75, 23);
            this.button_ok.TabIndex = 1;
            this.button_ok.Text = "Confirm";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // Form_Popup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 136);
            this.ControlBox = false;
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.label_main);
            this.Name = "Form_Popup";
            this.ShowIcon = false;
            this.Text = "Popup";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form_Popup_Load);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label label_main;
        private System.Windows.Forms.Button button_ok;

        #endregion
    }
}