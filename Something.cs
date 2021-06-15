using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Threading;
using MusicBeePlugin.Form.Configure;
using MusicBeePlugin.Form.Popup;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        private MusicBeeApiInterface _mbApiInterface;
        private PluginInfo _about = new PluginInfo();
        private PluginSettings _settings;
        private PaintManager _paintManager;
        
        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            _mbApiInterface = new MusicBeeApiInterface();
            _mbApiInterface.Initialise(apiInterfacePtr);
            
            _about.PluginInfoVersion = PluginInfoVersion;
            _about.Name = "User Account";
            _about.Description = "Add a user account with a username and profile picture.";
            _about.Author = "imak101";
            _about.TargetApplication = "User";   //  the name of a Plugin Storage device or panel header for a dockable panel
            _about.Type = PluginType.General;
            _about.VersionMajor = 0;  // your plugin version
            _about.VersionMinor = 0;
            _about.Revision = 1;
            _about.MinInterfaceVersion = MinInterfaceVersion;
            _about.MinApiRevision = MinApiRevision;
            _about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
            _about.ConfigurationPanelHeight = 0;   // height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function

            _settings = new PluginSettings(ref _mbApiInterface);
            _paintManager = new PaintManager(ref _mbApiInterface, ref _settings);
            
            return _about;
        }

        public bool Configure(IntPtr panelHandle)
        {
            // save any persistent settings in a sub-folder of this path
            string dataPath = _mbApiInterface.Setting_GetPersistentStoragePath();
            // panelHandle will only be set if you set about.ConfigurationPanelHeight to a non-zero value
            // keep in mind the panel width is scaled according to the font the user has selected
            // if about.ConfigurationPanelHeight is set to 0, you can display your own popup window
            if (panelHandle != IntPtr.Zero)
            {
                Panel configPanel = (Panel)Panel.FromHandle(panelHandle);
                Label prompt = new Label();
                prompt.AutoSize = true;
                prompt.Location = new Point(0, 0);
                prompt.Text = "username:";
                TextBox textBox = new TextBox();
                textBox.Bounds = new Rectangle(70, 0, 100, textBox.Height);
                configPanel.Controls.AddRange(new Control[] { prompt, textBox });
            }

            //var settings = new PluginSettings(ref _mbApiInterface);

            Form_Configure form = new Form_Configure(ref _settings);
            
            if (form.CheckOpened("User Account"))
            {
                SystemSounds.Asterisk.Play();
                form.Close();
                new Form_Popup("Configuration menu already open!", "Error");
                return true;
            }
            
            form.Show();
            
            return true;
        }
       
        // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        // its up to you to figure out whether anything has changed and needs updating
        public void SaveSettings() {}

        // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
        public void Close(PluginCloseReason reason)
        {
        }

        // uninstall this plugin - clean up any persisted files
        public void Uninstall()
        {
            File.Delete(_mbApiInterface.Setting_GetPersistentStoragePath.Invoke() + "mb_Something1.xml");
        }

        // receive event notifications from MusicBee
        // you need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, and not just the startup event
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            // perform some action depending on the notification type
            switch (type)
            {
                case NotificationType.PluginStartup:
                    // perform startup initialisation
                    switch (_mbApiInterface.Player_GetPlayState())
                    {
                        case PlayState.Playing:
                        case PlayState.Paused:
                            // ...
                            break;
                    }
                    break;
                case NotificationType.TrackChanged:
                    string artist = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Artist);
                    // ...
                    break;
            }
        }

        // return an array of lyric or artwork provider names this plugin supports
        // the providers will be iterated through one by one and passed to the RetrieveLyrics/ RetrieveArtwork function in order set by the user in the MusicBee Tags(2) preferences screen until a match is found
        //public string[] GetProviders()
        //{
        //    return null;
        //}

        // return lyrics for the requested artist/title from the requested provider
        // only required if PluginType = LyricsRetrieval
        // return null if no lyrics are found
        //public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album, bool synchronisedPreferred, string provider)
        //{
        //    return null;
        //}

        // return Base64 string representation of the artwork binary data from the requested provider
        // only required if PluginType = ArtworkRetrieval
        // return null if no artwork is found
        //public string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album, string provider)
        //{
        //    //Return Convert.ToBase64String(artworkBinaryData)
        //    return null;
        //}

        //  presence of this function indicates to MusicBee that this plugin has a dockable panel. MusicBee will create the control and pass it as the panel parameter
        //  you can add your own controls to the panel if needed
        //  you can control the scrollable area of the panel using the mbApiInterface.MB_SetPanelScrollableArea function
        //  to set a MusicBee header for the panel, set about.TargetApplication in the Initialise function above to the panel header text
        private static Control _formPanel;

        public static Control FormControl
        {
            get { return _formPanel; }
            private set { _formPanel = SetPanel(ref value); } //TODO: Format properly
        }

        private static Control SetPanel(ref Control panel)
        {
            _formPanel = panel;
            return _formPanel;
        }
        
        public int OnDockablePanelCreated(Control panel)
        {
          //    return the height of the panel and perform any initialisation here
          //    MusicBee will call panel.Dispose() when the user removes this panel from the layout configuration
          //    < 0 indicates to MusicBee this control is resizable and should be sized to fill the panel it is docked to in MusicBee
          //    = 0 indicates to MusicBee this control resizeable
          //    > 0 indicates to MusicBee the fixed height for the control.Note it is recommended you scale the height for high DPI screens(create a graphics object and get the DpiY value)
            float dpiScaling = 0;
            using (Graphics g = panel.CreateGraphics())
            {
                dpiScaling = g.DpiY / 96f;
            }
            panel.Paint += temp_Paint;
            FormControl = panel;
            return Convert.ToInt32(100 * dpiScaling);
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        { 
            var bgColorFromMb = Color.FromArgb(_mbApiInterface.Setting_GetSkinElementColour.Invoke(SkinElement.SkinSubPanel,ElementState.ElementStateDefault, ElementComponent.ComponentBackground)); 
            var fgColorFromMb = Color.FromArgb(_mbApiInterface.Setting_GetSkinElementColour.Invoke(SkinElement.SkinSubPanel,ElementState.ElementStateDefault,ElementComponent.ComponentForeground));
            
            //var imageb = ResizeImage(Image.FromFile("F:\\Code\\Misc\\Asset\\pfp\\20210430_133900.png"), 60, 60);
            
            e.Graphics.Clear(bgColorFromMb);
           // e.Graphics.DrawImage(imageb, new Point(100,20));
            TextRenderer.DrawText(e.Graphics, "imak101",SystemFonts.CaptionFont, new Point(106,83),fgColorFromMb);
        }

        private void temp_Paint(object sender, PaintEventArgs e)
        {
            _paintManager.SetArgs(ref e);
            
            _paintManager.test();
        }

        // presence of this function indicates to MusicBee that the dockable panel created above will show menu items when the panel header is clicked
        // return the list of ToolStripMenuItems that will be displayed
        public List<ToolStripItem> GetHeaderMenuItems()
        {
            List<ToolStripItem> list = new List<ToolStripItem>();
            list.Add(new ToolStripMenuItem("A menu item"));
            return list;
        }
    }
}