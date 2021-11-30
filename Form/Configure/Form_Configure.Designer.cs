using System;
using System.ComponentModel;

namespace MusicBeePlugin.Form.Configure
{
    partial class Form_Configure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Configure));
            this.label_username = new System.Windows.Forms.Label();
            this.textbox_username = new System.Windows.Forms.TextBox();
            this.label_pfp = new System.Windows.Forms.Label();
            this.button_pfp = new System.Windows.Forms.Button();
            this.picbox_pfp = new System.Windows.Forms.PictureBox();
            this.button_submit = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.openFileDialog_pfp = new System.Windows.Forms.OpenFileDialog();
            this.button_about = new System.Windows.Forms.Button();
            this.checkBox_roundPfp = new System.Windows.Forms.CheckBox();
            this.button_updater = new System.Windows.Forms.Button();
            this.label_versionInfo = new System.Windows.Forms.Label();
            this.checkBox_useTimerDrawing = new System.Windows.Forms.CheckBox();
            this.button_GetCurrentAlbum = new System.Windows.Forms.Button();
            this.numericUpDown_gifSpeed = new System.Windows.Forms.NumericUpDown();
            this.label_cutsomGifSpeed = new System.Windows.Forms.Label();
            this.button_gifSpeedOriginal = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_pfp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_gifSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // label_username
            // 
            this.label_username.Location = new System.Drawing.Point(12, 9);
            this.label_username.Name = "label_username";
            this.label_username.Size = new System.Drawing.Size(72, 33);
            this.label_username.TabIndex = 0;
            this.label_username.Text = "Username:";
            this.label_username.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textbox_username
            // 
            this.textbox_username.Location = new System.Drawing.Point(77, 16);
            this.textbox_username.MaxLength = 30;
            this.textbox_username.Name = "textbox_username";
            this.textbox_username.Size = new System.Drawing.Size(135, 20);
            this.textbox_username.TabIndex = 1;
            // 
            // label_pfp
            // 
            this.label_pfp.Location = new System.Drawing.Point(12, 79);
            this.label_pfp.Name = "label_pfp";
            this.label_pfp.Size = new System.Drawing.Size(80, 35);
            this.label_pfp.TabIndex = 2;
            this.label_pfp.Text = "Profile Picture:";
            this.label_pfp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button_pfp
            // 
            this.button_pfp.AllowDrop = true;
            this.button_pfp.Location = new System.Drawing.Point(98, 85);
            this.button_pfp.Name = "button_pfp";
            this.button_pfp.Size = new System.Drawing.Size(101, 23);
            this.button_pfp.TabIndex = 3;
            this.button_pfp.Text = "Upload Picture";
            this.button_pfp.UseVisualStyleBackColor = true;
            this.button_pfp.Click += new System.EventHandler(this.button_pfp_Click);
            // 
            // picbox_pfp
            // 
            this.picbox_pfp.Location = new System.Drawing.Point(12, 114);
            this.picbox_pfp.Name = "picbox_pfp";
            this.picbox_pfp.Size = new System.Drawing.Size(200, 200);
            this.picbox_pfp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_pfp.TabIndex = 4;
            this.picbox_pfp.TabStop = false;
            this.picbox_pfp.Paint += new System.Windows.Forms.PaintEventHandler(this.picBox_Paint);
            // 
            // button_submit
            // 
            this.button_submit.Location = new System.Drawing.Point(542, 265);
            this.button_submit.Name = "button_submit";
            this.button_submit.Size = new System.Drawing.Size(90, 40);
            this.button_submit.TabIndex = 5;
            this.button_submit.Text = "Apply";
            this.button_submit.UseVisualStyleBackColor = true;
            this.button_submit.Click += new System.EventHandler(this.button_submit_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(429, 265);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(90, 40);
            this.button_cancel.TabIndex = 6;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // openFileDialog_pfp
            // 
            this.openFileDialog_pfp.DefaultExt = "jpg";
            this.openFileDialog_pfp.Filter = "*.jpg *.png *.gif| *.jpg; *.png; *.gif";
            this.openFileDialog_pfp.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            this.openFileDialog_pfp.Title = "Select Picture";
            // 
            // button_about
            // 
            this.button_about.Location = new System.Drawing.Point(542, 12);
            this.button_about.Name = "button_about";
            this.button_about.Size = new System.Drawing.Size(90, 40);
            this.button_about.TabIndex = 7;
            this.button_about.Text = "About";
            this.button_about.UseVisualStyleBackColor = true;
            this.button_about.Click += new System.EventHandler(this.button_about_Click);
            // 
            // checkBox_roundPfp
            // 
            this.checkBox_roundPfp.Location = new System.Drawing.Point(218, 274);
            this.checkBox_roundPfp.Name = "checkBox_roundPfp";
            this.checkBox_roundPfp.Size = new System.Drawing.Size(134, 40);
            this.checkBox_roundPfp.TabIndex = 8;
            this.checkBox_roundPfp.Text = "Round picture\'s edges\r\n";
            this.checkBox_roundPfp.UseVisualStyleBackColor = true;
            this.checkBox_roundPfp.CheckedChanged += new System.EventHandler(this.checkBox_roundPfp_CheckedChanged);
            // 
            // button_updater
            // 
            this.button_updater.Location = new System.Drawing.Point(542, 68);
            this.button_updater.Name = "button_updater";
            this.button_updater.Size = new System.Drawing.Size(90, 40);
            this.button_updater.TabIndex = 9;
            this.button_updater.Text = "Update Menu";
            this.button_updater.UseVisualStyleBackColor = true;
            this.button_updater.Click += new System.EventHandler(this.button_updater_Click);
            // 
            // label_versionInfo
            // 
            this.label_versionInfo.Location = new System.Drawing.Point(542, 111);
            this.label_versionInfo.Name = "label_versionInfo";
            this.label_versionInfo.Size = new System.Drawing.Size(90, 42);
            this.label_versionInfo.TabIndex = 10;
            this.label_versionInfo.Text = "Awaiting data...";
            this.label_versionInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // checkBox_useTimerDrawing
            // 
            this.checkBox_useTimerDrawing.Enabled = false;
            this.checkBox_useTimerDrawing.Location = new System.Drawing.Point(218, 114);
            this.checkBox_useTimerDrawing.Name = "checkBox_useTimerDrawing";
            this.checkBox_useTimerDrawing.Size = new System.Drawing.Size(189, 40);
            this.checkBox_useTimerDrawing.TabIndex = 11;
            this.checkBox_useTimerDrawing.Text = "Use alternate GIF drawing method";
            this.checkBox_useTimerDrawing.UseVisualStyleBackColor = true;
            this.checkBox_useTimerDrawing.CheckedChanged += new System.EventHandler(this.checkBox_useTimerDrawing_CheckedChanged);
            // 
            // button_GetCurrentAlbum
            // 
            this.button_GetCurrentAlbum.Location = new System.Drawing.Point(205, 85);
            this.button_GetCurrentAlbum.Name = "button_GetCurrentAlbum";
            this.button_GetCurrentAlbum.Size = new System.Drawing.Size(101, 23);
            this.button_GetCurrentAlbum.TabIndex = 12;
            this.button_GetCurrentAlbum.Text = "Get Current Album";
            this.button_GetCurrentAlbum.UseVisualStyleBackColor = true;
            this.button_GetCurrentAlbum.Click += new System.EventHandler(this.button_GetCurrentAlbum_Click);
            // 
            // numericUpDown_gifSpeed
            // 
            this.numericUpDown_gifSpeed.Enabled = false;
            this.numericUpDown_gifSpeed.Location = new System.Drawing.Point(337, 153);
            this.numericUpDown_gifSpeed.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numericUpDown_gifSpeed.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            this.numericUpDown_gifSpeed.Name = "numericUpDown_gifSpeed";
            this.numericUpDown_gifSpeed.Size = new System.Drawing.Size(61, 20);
            this.numericUpDown_gifSpeed.TabIndex = 13;
            this.numericUpDown_gifSpeed.Value = new decimal(new int[] { 10, 0, 0, 0 });
            this.numericUpDown_gifSpeed.ValueChanged += new System.EventHandler(this.numericUpDown_gifSpeed_ValueChanged);
            // 
            // label_cutsomGifSpeed
            // 
            this.label_cutsomGifSpeed.Enabled = false;
            this.label_cutsomGifSpeed.Location = new System.Drawing.Point(218, 155);
            this.label_cutsomGifSpeed.Name = "label_cutsomGifSpeed";
            this.label_cutsomGifSpeed.Size = new System.Drawing.Size(116, 23);
            this.label_cutsomGifSpeed.TabIndex = 14;
            this.label_cutsomGifSpeed.Text = "Set custom GIF speed:";
            // 
            // button_gifSpeedOriginal
            // 
            this.button_gifSpeedOriginal.Enabled = false;
            this.button_gifSpeedOriginal.Location = new System.Drawing.Point(404, 153);
            this.button_gifSpeedOriginal.Name = "button_gifSpeedOriginal";
            this.button_gifSpeedOriginal.Size = new System.Drawing.Size(56, 21);
            this.button_gifSpeedOriginal.TabIndex = 15;
            this.button_gifSpeedOriginal.Text = "Original";
            this.button_gifSpeedOriginal.UseVisualStyleBackColor = true;
            this.button_gifSpeedOriginal.Click += new System.EventHandler(this.button_gifSpeedOriginal_Click);
            // 
            // Form_Configure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 316);
            this.Controls.Add(this.button_gifSpeedOriginal);
            this.Controls.Add(this.label_cutsomGifSpeed);
            this.Controls.Add(this.numericUpDown_gifSpeed);
            this.Controls.Add(this.button_GetCurrentAlbum);
            this.Controls.Add(this.checkBox_useTimerDrawing);
            this.Controls.Add(this.label_versionInfo);
            this.Controls.Add(this.button_updater);
            this.Controls.Add(this.checkBox_roundPfp);
            this.Controls.Add(this.button_about);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_submit);
            this.Controls.Add(this.picbox_pfp);
            this.Controls.Add(this.button_pfp);
            this.Controls.Add(this.label_pfp);
            this.Controls.Add(this.textbox_username);
            this.Controls.Add(this.label_username);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_Configure";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "User Cosmetics Configuration";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Configure_FormClosing);
            this.Load += new System.EventHandler(this.Form_Configure_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picbox_pfp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_gifSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label_cutsomGifSpeed;
        
        private System.Windows.Forms.NumericUpDown numericUpDown_gifSpeed;
        
        private System.Windows.Forms.Button button_GetCurrentAlbum;

        private System.Windows.Forms.Button button_gifSpeedOriginal;

        private System.Windows.Forms.CheckBox checkBox_useTimerDrawing;
        
        private System.Windows.Forms.Label label_versionInfo;
        
        private System.Windows.Forms.Button button_updater;
        
        private System.Windows.Forms.CheckBox checkBox_roundPfp;
        
        private System.Windows.Forms.Button button_about;

        private System.Windows.Forms.OpenFileDialog openFileDialog_pfp;

        private System.Windows.Forms.Button button_cancel;

        private System.Windows.Forms.Button button_submit;

        private System.Windows.Forms.PictureBox picbox_pfp;

        private System.Windows.Forms.Button button_pfp;

        private System.Windows.Forms.Label label_pfp;

        private System.Windows.Forms.TextBox textbox_username;

        private System.Windows.Forms.Label label_username;

        #endregion
    }
}