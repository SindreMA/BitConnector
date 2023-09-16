using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MonoTorrent.Client;
using MonoTorrent;
using MonoTorrent.Client.Connections;
using MonoTorrent.Common;
using MonoTorrent.Client.Encryption;
using System.Net;

namespace BitConnector
{
    public partial class Form1 : Form
    {
        public string pathTorrent;
        public string pathShow;
        public string pathStatus;
        public string pathUrl;
        public string b;
        public List<string> list;
        public TorrentManager manager;
        public Form1()
        {

            InitializeComponent();
            timer1.Stop();
            list = new List<string>();
            pathTorrent = @"c:\temp\myfile.torrent";
            pathShow = @"c:\serier\";
            pathStatus = @"c:\temp\status.txt";
            pathUrl = @"C:\temp\URL.txt";

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (File.Exists(pathUrl))
            {
                if (File.ReadAllLines(pathUrl).Last().Contains("http") && File.ReadAllLines(pathUrl).Last().Contains(".torrent"))
                {
                    try
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(File.ReadAllLines(pathUrl).Last(), pathTorrent);
                    }
                    catch { MessageBox.Show("error downloading torrent file"); }
                    try
                    {
                        //DownloadTorrent();
                        LoadTorrent();
                    }
                    catch { MessageBox.Show("error at loading torrent"); }
                    try
                    {
                        StartTorrents();
                    }
                    catch { MessageBox.Show("error starting torrent"); }
                    try
                    {
                        File.Delete(pathUrl);
                        File.Delete(pathTorrent);
                    }
                    catch { MessageBox.Show("error deleting"); }
                }
            }
        }
        ClientEngine engine;
        List<TorrentManager> managers = new List<TorrentManager>();
        public void DownloadTorrent()
        {
            this.engine = new ClientEngine(new EngineSettings());

            Torrent torrent = Torrent.Load(pathTorrent);
            TorrentManager manager = new TorrentManager(torrent, pathShow, new TorrentSettings());
            engine.Register(manager);
            manager.Start();
        }
        void LoadTorrent()
        {
            EngineSettings settings = new EngineSettings();
            settings.AllowedEncryption = EncryptionTypes.All;
            settings.SavePath = Path.Combine(Environment.CurrentDirectory, pathShow);
            if (!Directory.Exists(settings.SavePath))
                Directory.CreateDirectory(settings.SavePath);
            engine = new ClientEngine(settings);
            engine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Any, 30805));
            Torrent torrent = Torrent.Load(pathTorrent);
            manager = new TorrentManager(torrent, engine.Settings.SavePath, new TorrentSettings());
            engine.Register(manager);
            manager.Start();
            /*
            // Load a .torrent file into memory
            Torrent torrent = Torrent.Load("c:\\temp\\myfile.torrent");
            foreach (TorrentFile file in torrent.Files)
                file.Priority = Priority.DoNotDownload;
            torrent.Files[0].Priority = Priority.Highest;
            TorrentManager manager = new TorrentManager(torrent, "d:\\test\\", new TorrentSettings());
            managers.Add(manager);
            PiecePicker picker = new StandardPicker();
            picker = new PriorityPicker(picker);
            manager.ChangePicker(picker);
            */
        }
        void StartTorrents()
        {
            engine.StartAll();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
        public void status()
        {
            File.Delete(pathStatus);
            int ss;
            int.TryParse(manager.Monitor.DataBytesDownloaded.ToString(), out ss);
            ss = ss / 8;
            ss = ss / 8;
            ss = ss / 8;
            ss = ss / 8;
            ss = ss / 8;
            ss = ss / 8;
            string fs = ss.ToString() + "mb";
            File.AppendAllText(pathStatus, manager.Progress.ToString() + "#" + manager.State.ToString());
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                status();
            }
            catch { }
            try
            {
                if (manager.State.ToString().Contains("ownloa"))
                {
                }
                else if (manager.State.ToString().Contains("eedin"))
                {
                    StopTorrent(manager);
                }
                else if (manager.State.ToString().Contains("mple"))
                {
                    StopTorrent(manager);
                }
            }
            catch { }
        }
        private void button5_Click(object sender, EventArgs e)
        {
        }
        public void button4_Click(object sender, EventArgs e)
        {
        }
        public void StopTorrent(TorrentManager manager)
        {
            manager.TorrentStateChanged += delegate (object o, TorrentStateChangedEventArgs e)
            {
                if (e.NewState == TorrentState.Stopping)
                {
                }
                else if (e.NewState == TorrentState.Stopped)
                {
                    engine.Unregister(manager);
                    manager.Dispose();
                }
            };
            manager.Stop();
        }

    }
}
