using System.ComponentModel;

namespace MusicBeePlugin.Updater.Form
{
    partial class Form_Updater
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Updater));
            this.label_localVer = new System.Windows.Forms.Label();
            this.label_GHCurrentVer = new System.Windows.Forms.Label();
            this.link_GHRepo = new System.Windows.Forms.LinkLabel();
            this.toolTip_GHLink = new System.Windows.Forms.ToolTip(this.components);
            this.link_GHCurrent = new System.Windows.Forms.LinkLabel();
            this.button_update = new System.Windows.Forms.Button();
            this.button_ok = new System.Windows.Forms.Button();
            this.label_verCompare = new System.Windows.Forms.Label();
            this.button_refresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_localVer
            // 
            this.label_localVer.Location = new System.Drawing.Point(295, 51);
            this.label_localVer.Name = "label_localVer";
            this.label_localVer.Size = new System.Drawing.Size(84, 35);
            this.label_localVer.TabIndex = 0;
            this.label_localVer.Text = "Local Version";
            this.label_localVer.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label_GHCurrentVer
            // 
            this.label_GHCurrentVer.Location = new System.Drawing.Point(158, 28);
            this.label_GHCurrentVer.Name = "label_GHCurrentVer";
            this.label_GHCurrentVer.Size = new System.Drawing.Size(84, 80);
            this.label_GHCurrentVer.TabIndex = 1;
            this.label_GHCurrentVer.Text = "Fetching most recent version...";
            this.label_GHCurrentVer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // link_GHRepo
            // 
            this.link_GHRepo.Location = new System.Drawing.Point(12, 9);
            this.link_GHRepo.Name = "link_GHRepo";
            this.link_GHRepo.Size = new System.Drawing.Size(70, 19);
            this.link_GHRepo.TabIndex = 2;
            this.link_GHRepo.TabStop = true;
            this.link_GHRepo.Text = "GitHub Repo";
            this.link_GHRepo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_GHRepo_LinkClicked);
            this.link_GHRepo.MouseHover += new System.EventHandler(this.link_GHRepo_MouseHover);
            // 
            // link_GHCurrent
            // 
            this.link_GHCurrent.Location = new System.Drawing.Point(12, 28);
            this.link_GHCurrent.Name = "link_GHCurrent";
            this.link_GHCurrent.Size = new System.Drawing.Size(111, 19);
            this.link_GHCurrent.TabIndex = 3;
            this.link_GHCurrent.TabStop = true;
            this.link_GHCurrent.Text = "Most Current Release";
            this.link_GHCurrent.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_GHCurrent_LinkClicked);
            this.link_GHCurrent.MouseHover += new System.EventHandler(this.link_GHCurrent_MouseHover);
            // 
            // button_update
            // 
            this.button_update.Location = new System.Drawing.Point(158, 123);
            this.button_update.Name = "button_update";
            this.button_update.Size = new System.Drawing.Size(84, 36);
            this.button_update.TabIndex = 4;
            this.button_update.Text = "Update";
            this.button_update.UseVisualStyleBackColor = true;
            this.button_update.Click += new System.EventHandler(this.button_update_Click);
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(295, 123);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(84, 36);
            this.button_ok.TabIndex = 5;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // label_verCompare
            // 
            this.label_verCompare.Location = new System.Drawing.Point(12, 78);
            this.label_verCompare.Name = "label_verCompare";
            this.label_verCompare.Size = new System.Drawing.Size(122, 81);
            this.label_verCompare.TabIndex = 6;
            this.label_verCompare.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_refresh
            // 
            this.button_refresh.Location = new System.Drawing.Point(158, 5);
            this.button_refresh.Name = "button_refresh";
            this.button_refresh.Size = new System.Drawing.Size(84, 23);
            this.button_refresh.TabIndex = 7;
            this.button_refresh.Text = "Refresh...";
            this.button_refresh.UseVisualStyleBackColor = true;
            this.button_refresh.Click += new System.EventHandler(this.button_refresh_Click);
            // 
            // Form_Updater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 171);
            this.Controls.Add(this.button_refresh);
            this.Controls.Add(this.label_verCompare);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.button_update);
            this.Controls.Add(this.link_GHCurrent);
            this.Controls.Add(this.link_GHRepo);
            this.Controls.Add(this.label_GHCurrentVer);
            this.Controls.Add(this.label_localVer);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_Updater";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update Menu";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form_Updater_Load);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button button_refresh;
        
        private System.Windows.Forms.Label label_verCompare;
        
        private System.Windows.Forms.Button button_ok;

        private System.Windows.Forms.Button button_update;

        private System.Windows.Forms.LinkLabel link_GHCurrent;
        
        private System.Windows.Forms.ToolTip toolTip_GHLink;

        private System.Windows.Forms.LinkLabel link_GHRepo;
        
        private System.Windows.Forms.Label label_GHCurrentVer;
        
        private System.Windows.Forms.Label label_localVer;
        
        #endregion
    }
}