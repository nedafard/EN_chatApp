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

namespace chatAppC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string susername;
        string chosenuser;
        string[] chats = new string[10];
        int count;
        public MainWindow()
        {
            InitializeComponent();
            tcp_client.reff = this;
            tcp_client.initiateClient("127.0.0.1");
            


        }

        private void btnSighin_Click(object sender, RoutedEventArgs e)
        {
            tcp_client.client.Send("s"+username2.Text + " " + password2.Password.GetHashCode());
        }

        private void btnlogin_Click(object sender, RoutedEventArgs e)
        {
            tcp_client.client.Send("l" + username2.Text + " " + password2.Password.GetHashCode());
            
        }

        private void Btnexit_Click(object sender, RoutedEventArgs e)
        {
            gridlogin2.Visibility = Visibility.Visible;
            gridchat.Visibility = Visibility.Collapsed;
        }

        private void Btnadd_Click(object sender, RoutedEventArgs e)
        {
            tcp_client.client.Send("a" + susername + " " + user.Text);
        }

        private void Btnsend_Click(object sender, RoutedEventArgs e)
        {
            tcp_client.client.Send("m" + susername + " " + chosenuser+" "+message.Text);
            showm(message.Text,0);
        }

        public void receive(string message)
        {
           
            if (message[0] == 's')
            {
                if (message[1] == '0')
                {
                    MessageBox.Show("new acount ceated.");
                }
                else if (message[1] == '1')
                {
                    MessageBox.Show("this username is already taken! please use another one.");
                }
            }
            else if (message[0] == 'l')
            {
                if (message[1] == '0')
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        susername = username2.Text;
                        gridlogin2.Visibility = Visibility.Collapsed;
                        gridchat.Visibility = Visibility.Visible;

                    });
                    
                }
                else if (message[1] == '1')
                {
                    MessageBox.Show("Wrong password or username! please try again.");
                }
            }
            else if (message[0] == 'a')
            {
                if (message[1] == '0')
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        showchat(user.Text);
                        chats[count] = user.Text;
                        count++;
                    });
                    
                }
                else if (message[1] == '1')
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        MessageBox.Show(user.Text + "is not online");
                    });
                }
            }
            else if (message[0] == 'm')
            {
                message = message.Substring(1);
                string[] UP = message.Split(' ');
                bool res = false;
                for(int i = 0; i < count; i++)
                {
                    if(chats[i] == UP[0])
                    {
                        res = true;
                    }
                }
                if (!res)
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        showchat(UP[0]);
                        chats[count] = UP[0];
                        count++;
                    });
                    
                }
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    showm(UP[2], 1);
                });
                
                
            }
        }

        public void showchat(string user)
        {
            TextBlock txtb = new TextBlock();
            txtb.Text = user;
            txtb.FontSize = 18;
            txtb.Foreground = Brushes.White;
            txtb.Margin = new Thickness(50, 8, 0, 0);

            TextBlock txtb1 = new TextBlock();
            txtb1.Text = user[0]+"";
            txtb1.VerticalAlignment = VerticalAlignment.Center;
            txtb1.HorizontalAlignment = HorizontalAlignment.Center;
            txtb1.FontSize = 22;
            txtb1.Foreground = Brushes.White;

            Border border1 = new Border() 
            {
                Height = 42,
                Width = 42,
                Background = Brushes.Orange,
                HorizontalAlignment= HorizontalAlignment.Left,
                CornerRadius = new CornerRadius(30),
            };
            border1.Child=txtb1;
            Grid grid1 = new Grid()
            {
                Name=user,
                Width=170,
                Height=45,
                Margin = new Thickness(0, 20, 0, 0)
        };
            grid1.Children.Add(border1);
            grid1.Children.Add(txtb);
            grid1.MouseUp+= new MouseButtonEventHandler(selecchat);
            chatlist.Children.Add(grid1);
           

    }

        private void selecchat(object sender, MouseButtonEventArgs e)
        {
            string usern= ((Grid)sender).Name;
            chosenuser = usern;
            messagelist.Children.Clear();

        }

        private void showm(string text, int friend)
        {
            TextBlock txtb1 = new TextBlock()
            {
                Padding = new Thickness(7),
                Background = Brushes.Transparent,
                Foreground = Brushes.WhiteSmoke,
                Text = text,
                MaxWidth = 330,
                FontSize = 16,
                TextWrapping = TextWrapping.Wrap
            };
            Color iviolet = (Color)ColorConverter.ConvertFromString("#d8302040");

            Border brd1 = new Border()
            {
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#d8302040"),
                MaxWidth = 350,
                CornerRadius = new CornerRadius(16),
                Margin = new Thickness(0, 20, 160, 0)

            };
            if (friend == 0)
            {
                brd1.Margin = new Thickness(150, 20, 0, 0);
            }
            brd1.Child = txtb1;
            messagelist.Children.Add(brd1);
        }
    }


    //___________________________________________________________________________________________


    public class tcp_client
    {
        public static MainWindow reff;
        public static SimpleTcpClient client;


        public static void initiateClient(String IP)
        {
            client = new SimpleTcpClient(IP + ":600");

            client.Events.Connected += Connected;
            client.Events.Disconnected += Disconnected;
            client.Events.DataReceived += DataReceivedClient;
            try
            {
                client.Connect();
            }
            catch (Exception e)
            {
            }

        }

        public static void Connected(object sender, EventArgs e)
        {
        }

        public static void Disconnected(object sender, EventArgs e)
        {
        }

        public static void DataReceivedClient(object sender, DataReceivedEventArgs e)
        {
            reff.receive(Encoding.UTF8.GetString(e.Data));


        }

    }


}
