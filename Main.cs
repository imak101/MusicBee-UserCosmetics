using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using MusicBeePlugin.Form.Configure;
using MusicBeePlugin.Form.Popup;
using MusicBeePlugin.Updater.Form;
using MusicBeePlugin.Updater;

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
            _about.ProjectName = "mb_UserCosmetics";
            _about.Name = "User Cosmetics";
            _about.Description = "Add a user 'account' with a username and profile picture.";
            _about.Author = "imak101";
            _about.TargetApplication = "User";   //  the name of a Plugin Storage device or panel header for a dockable panel
            _about.Type = PluginType.PanelView;
            _about.VersionMajor = 0;  // your plugin version
            _about.VersionMinor = 2;
            _about.Revision = 3;
            _about.MinInterfaceVersion = MinInterfaceVersion;
            _about.MinApiRevision = MinApiRevision;
            _about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
            _about.ConfigurationPanelHeight = 0;   // height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function
            _about.PersistentStoragePath = _mbApiInterface.Setting_GetPersistentStoragePath.Invoke();
            About = _about;
            
            _settings = new PluginSettings(ref _mbApiInterface);
            _paintManager = new PaintManager(ref _mbApiInterface, ref _settings);

            if (File.Exists(Application.StartupPath + "/Plugins/old.dll"))
            {
                File.Delete(Application.StartupPath + "/Plugins/old.dll");
            }

            _mbApiInterface.MB_AddMenuItem.Invoke("mnuTools/User Configure", "User Account: Configure", (sender, args) => Configure(IntPtr.Zero));


            return _about;
        }

        public bool Configure(IntPtr panelHandle)
        {
            Form_Configure form = new Form_Configure(ref _settings, ref _mbApiInterface);
            
            if (Form_Configure.CheckOpened("User Cosmetics Configuration"))
            {
                SystemSounds.Asterisk.Play();
                form.Close();
                new Form_Popup("Configuration menu already open!", "Error");
                return true;
            }

            form.StartPosition = FormStartPosition.CenterScreen;
            
            form.Show();
            
            return true;
        }

        private static PluginInfo _About;

        private static void SetAbout(ref PluginInfo about)
        {
            _About = about;
        }

        public static PluginInfo About
        {
            get => _About;
            private set => SetAbout(ref value);
        }

        // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        // its up to you to figure out whether anything has changed and needs updating
        public void SaveSettings() {}

        // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
        public void Close(PluginCloseReason reason)
        {
        }

        // uninstall this plugin - clean up any persisted files
        public static void Uninstall()
        {
            //File.Delete(_mbApiInterface.Setting_GetPersistentStoragePath.Invoke() + "mb_Something1.xml");
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


        private static Control _formControlMain;

        public static Control FormControlMain
        {
            get => _formControlMain;
            private set => _formControlMain = SetControl(ref value);
        }

        private static Control SetControl(ref Control panel)
        {
            _formControlMain = panel;
            return _formControlMain;
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
            panel.Paint += panel_Paint;
            FormControlMain = panel;
            return Convert.ToInt32(100 * dpiScaling);
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            _paintManager.SetArgs(ref e);
            
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            
            _paintManager.MainPainter();
        }

        // presence of this function indicates to MusicBee that the dockable panel created above will show menu items when the panel header is clicked
        // return the list of ToolStripItems that will be displayed
        public List<ToolStripItem> GetMenuItems()
        {
            List<ToolStripItem> list = new List<ToolStripItem> {new ToolStripMenuItem("Configure")};
            list[0].Click += (sender, args) => Configure(IntPtr.Zero);
            return list;
        }
    }
}