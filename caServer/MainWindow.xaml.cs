using SimpleTcp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace caServer
{
    
    public partial class MainWindow : Window
    {
        string path = @"./dbuser.csv";
        public string[,] clients = new string[2, 1000];
        public int counter = 0;
        public MainWindow()
        {
            InitializeComponent();
            tcp_server.reff = this;
        }

        private void btnstart_Click(object sender, RoutedEventArgs e)
        {
            tcp_server.startServer("127.0.0.1");
            note.Text = "Server has been started successfully";
            btnstart.IsEnabled = false;
        }

        private void btnstop_Click(object sender, RoutedEventArgs e)
        {
           
        }

        public void receive(string e, string message)
        {
            


            if (message[0] == 's')
            {
                message = message.Substring(1);
                bool res = true;
                string[] UP = message.Split(' ');
                string UPl = message + Environment.NewLine;
                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        string[] line = reader.ReadLine().Split(' ');
                        if (line[0] == UP[0])
                        {
                            res = false;
                            break;
                        }

                    }
                }
                if (res)
                {
                    File.AppendAllText(path, UPl);
                    tcp_server.server.Send(e, "s0");


                }
                else
                {
                    tcp_server.server.Send(e, "s1");
                }

            }
            else if (message[0] == 'l')
            {
                message = message.Substring(1);
                bool res = false;
                string[] UP = message.Split(' ');
                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line == message)
                        {
                            res = true;
                            break;
                        }

                    }
                }
                if (res)
                {
                    clients[0, counter] = UP[0];
                    clients[1, counter] = e;
                    counter++;
                    tcp_server.server.Send(e, "l0");


                }
                else
                {
                    tcp_server.server.Send(e, "l1");

                }
            }
            else if (message[0] == 'a')
            {
                message = message.Substring(1);
                bool res = false;
                string[] UP = message.Split(' ');
                for (int i = 0; i < counter; i++)
                {
                    if (clients[0, i] == UP[1])
                    {
                        res = true;
                        break;
                    }
                }
                if (res)
                {
                    tcp_server.server.Send(e, "a0");
                }
                else
                {
                    tcp_server.server.Send(e, "a1");
                }
            }
            else if (message[0] == 'm')
            {
                message = message.Substring(1);
                string ipn="";
                string[] UP = message.Split(' ');
                for (int i = 0; i < counter; i++)
                {
                    if (clients[0, i] == UP[1])
                    {
                        ipn = clients[1, i];
                        break;
                    }
                }
                message = "m" + message;
                if (ipn!="")
                {
                    tcp_server.server.Send(ipn, message);
                   
                }
                
            }
        }
    }

    //____________________________________________________________________________________________________________

    public class tcp_server
    {
        public static MainWindow reff;
        public static SimpleTcpServer server;

        public static void startServer(String IP)
        {
            server = new SimpleTcpServer("*:600");

            server.Events.ClientConnected += ClientConnected;
            server.Events.ClientDisconnected += ClientDisconnected;
            server.Events.DataReceived += DataReceived;

            try
            {
                server.Start();
            }
            catch (Exception e)
            {

                Console.WriteLine("Exception in Server constructor" + e.Message);
            }

        }
        public static void ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                reff.note.Text = "client conected";

            });
            
        }

        public static void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {

           for(int i = 0; i < reff.counter; i++)
            {
                if (reff.clients[1, i] == e.IpPort)
                {
                    reff.clients[0, i] = "";
                    reff.clients[1, i] = "";
                    break;
                }
            }
                
            //Console.WriteLine("[" + e.IpPort + "] client disconnected: " + e.Reason.ToString());
           
        }

        public static void DataReceived(object sender, DataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                reff.note.Text = Encoding.UTF8.GetString(e.Data);

            });

            reff.receive(e.IpPort,Encoding.UTF8.GetString(e.Data));

        }
    }

}
