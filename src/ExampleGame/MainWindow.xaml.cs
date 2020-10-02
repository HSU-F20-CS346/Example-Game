using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExampleGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Player LocalPlayer { get; set; }
        Player RemotePlayer { get; set; }
        string ClientIpAddress { get; set; }
        Thread serverThread = null;
        int ListenPort = 3461;

        public MainWindow()
        {
            InitializeComponent();
            LocalPlayer = new Player();
            RemotePlayer = new Player();

            this.KeyDown += KeyPressDetected;

            InitializePlayer(LocalPlayer);
            InitializePlayer(RemotePlayer);

            StartServerLoop();
        }

        void InitializePlayer(Player p)
        {
            GameScene.Children.Add(p);
            Canvas.SetLeft(p, 0);
            Canvas.SetTop(p, 0);
        }

        private void KeyPressDetected(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    Canvas.SetTop(LocalPlayer, Canvas.GetTop(LocalPlayer) - 1);
                    break;

                case Key.Down:
                    Canvas.SetTop(LocalPlayer, Canvas.GetTop(LocalPlayer) + 1);
                    break;

                case Key.Left:
                    Canvas.SetLeft(LocalPlayer, Canvas.GetLeft(LocalPlayer) - 1);
                    break;

                case Key.Right:
                    Canvas.SetLeft(LocalPlayer, Canvas.GetLeft(LocalPlayer) + 1);
                    break;
            }

            //TODO: update with real coords
            SendUpdate(0, 0);
        }

        private void SetRemoteIpClicked(object sender, RoutedEventArgs e)
        {
            Settings s = new Settings();
            s.ShowDialog();
            ClientIpAddress = s.IpAddress;
        }

        private void SendUpdate(int xPos, int yPos)
        {
            UdpClient client = new UdpClient();
            byte[] package = new byte[1024];

            //TODO: build package payload
            try
            {
                client.Send(package, package.Length, ClientIpAddress, ListenPort);
            }
            catch(Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }

        private void StartServerLoop()
        {
            ThreadStart ts = () => { ServerLoop(); };
            serverThread = new Thread(ts);
            serverThread.Start();
        }

        private void ServerLoop()
        {
            UdpClient client = null;
            try
            {
                client = new UdpClient(ListenPort);
                IPEndPoint remoteClient = new IPEndPoint(IPAddress.Any, 0);
                while(true)
                {
                    byte[] clientData = client.Receive(ref remoteClient);
                    Debug.Write("data received");
                    //TODO: process data
                }
            }
            catch(Exception ex)
            {
                Debug.Write(ex.Message);

                //restart server thread on error
                Thread.Sleep(500);
                StartServerLoop();
            }
        }
    }
}
