using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
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
using System.Windows.Threading;

namespace Crypto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client client = new();
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 5);
            dispatcherTimer.Start();
        }

        private void fileChooseButtob_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                fileNames.Items.Add(openFileDialog.FileName);
        }

        private void chooseFileToDelete_Click(object sender, RoutedEventArgs e)
        {
            var deletedItem = fileNames.SelectedItem;

            if (deletedItem != null)
            {
                fileNames.Items.Remove(deletedItem);   
            }
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            if (connectState.IsChecked == false)
            {
                connectState.IsChecked = true;
                connectState.Background = new SolidColorBrush(Colors.Green);
                connectState.Content = "Подключение...";
                connectButton.Content = "";

                client.connect("localhost", 8000);

                connectState.IsChecked = true;
                connectState.Background = new SolidColorBrush(Colors.Yellow);
                connectState.Content = "Подключено";
                connectButton.Content = "Отключиться";

                if (!client.isConnected())
                {
                    connectState.IsChecked = false;
                    connectState.Background = null;
                    connectState.Content = "Нет соединения";
                    connectButton.Content = "Подключиться";
                    client.disconnect();
                }
                // TODO connect method
            }
            else
            {
                connectState.IsChecked = false;
                connectState.Background = null;
                connectState.Content = "Нет соединения";
                connectButton.Content = "Подключиться";

                client.disconnect();
                // TODO disconnect method
            }
        }

        private async void sendFile_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => sendFiles());
        }

        private void sendFiles()
        {
            foreach (string filePath in fileNames.Items)
            {
                client.sendFile(filePath);
            }
        }

        private async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (client.isConnected())
            {
                progressBar.Value = client.encryptor.process;
            }
        }

        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (client.getFile(downloadFileName.Text))
                downloadStatus.Content = "Загружено";
            else
                downloadStatus.Content = "Не удалость загрузить файл";
        }
    }
}
