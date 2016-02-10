using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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
using TestClient;

namespace WpfApp_StoryMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TestClient.TestClient cl = new TestClient.TestClient("http://localhost:63260/api/File");
            
        public MainWindow()
        {
            InitializeComponent();
            Download.IsEnabled = false;
            upload.IsEnabled = false;
        }

        private void getCollage_Click(object sender, RoutedEventArgs e)
        {
            collageList.Items.Clear();
            
            string[] files = cl.getAvailableFiles("collage");
            //if (!Dispatcher.CheckAccess())
            //{
            //    Dispatcher.Invoke(() =>
            //    {
                    foreach (string file in files)
                    {
                        collageList.Items.Add(file);
                    }
            //    });
            //}
            
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            string collage = collageList.SelectedItem.ToString();
            string collageId = collage.Split('_').Last();
            cl.downLoadFile(collageId);
            collageList.Items.Clear();
        }

        private void upload_Click(object sender, RoutedEventArgs e)
        {
            string title = caption.Text;
            string cs = content.Text;
            string position = order.Text;
            string storyId = styList.SelectedItem.ToString().Split('_').Last();
            string img = ImgPath.Text;
            string blk = storyId + "?" + title + "?" + cs + "?" + position + "?" + img;

            cl.upLoadFile(blk);

            caption.Clear();
            content.Clear();
            order.Clear();
            styList.Items.Clear();
            ImgPath.Clear();
        }

        private void getsty_Click(object sender, RoutedEventArgs e)
        {
            styList.Items.Clear();
            string[] files = cl.getAvailableFiles("story");
            foreach (string file in files)
            {
                styList.Items.Add(file);
            }
        }

        private void Img_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*";
            if(dlg.ShowDialog()==true)
            ImgPath.Text = dlg.FileName;
        }

        private void collageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (collageList.SelectedIndex >= 0)
            {
                Download.IsEnabled = true;
            }
            else
            {
                Download.IsEnabled = false;
            }
        }

        private void styList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (styList.SelectedIndex >= 0 && !String.IsNullOrEmpty(caption.Text) && !String.IsNullOrEmpty(content.Text) && !String.IsNullOrEmpty(order.Text) && !String.IsNullOrEmpty(ImgPath.Text))
            {
                upload.IsEnabled = true;
            }
            else
            {
                upload.IsEnabled = false;
            }
        }

       

    }
}
