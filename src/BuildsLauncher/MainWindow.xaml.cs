using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using BuildsFlightCore;

namespace BuildsLauncher
{
    public partial class MainWindow : Window
    {
        private AppInfo App;

        public MainWindow()
        {
            InitializeComponent();

            BuildsFlight.Init("https://s3.ap-northeast-2.amazonaws.com/s3aad/buildsflight_index.json");

            App = BuildsFlight.GetApp("test");
            if (App == null)
                MessageBox.Show("Application not found", "error", MessageBoxButton.OK, MessageBoxImage.Error);
            
            foreach (var build in App.Builds)
            {
                var title = build.Version;

                if (App.TargetVersion == build.Version)
                {
                    title += " (target)";
                    targetVersion.SelectedIndex = targetVersion.Items.Count;
                }

                targetVersion.Items.Add(title);
            }

            if (Directory.Exists("builds") == false)
                Directory.CreateDirectory("builds");
        }

        private void targetVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var idx = targetVersion.SelectedIndex;

            releaseNote.Text = App.Builds[idx].Note.Replace("\\r\\n", "\r\n");
        }

        private static void DownloadFile(string url, string path)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile(url, path);
        }
        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedBuild = App.Builds[targetVersion.SelectedIndex];
            var gameDir = $".\\builds\\{selectedBuild.Version}\\";

            if (Directory.Exists(gameDir) == false)
            {
                Directory.CreateDirectory(gameDir);

                DownloadFile(selectedBuild.Url, gameDir + "package.zip");

                ZipFile.ExtractToDirectory(gameDir + "package.zip", gameDir);
            }

            Process.Start(new ProcessStartInfo()
            {
                FileName = "run.bat",
                WorkingDirectory = gameDir
            });
        }
    }
}
