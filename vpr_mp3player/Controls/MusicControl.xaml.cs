using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace vpr_mp3player.Controls
{
    /// <summary>
    /// Interaktionslogik für MusicControl.xaml
    /// </summary>
    public partial class MusicControl : UserControl
    {
        private bool isPlaying = false;

        private MediaPlayer _player = new MediaPlayer();

        public MediaPlayer Player
        {
            get { return _player; }
            set { _player = value; }
        }

        public MusicControl()
        {
            InitializeComponent();
            Player.Volume = (double)sliVolume.Value;
        }

        #region Events

        /// <summary>
        /// Button click event for play
        /// </summary>
        /// <param name="sender">Sender element</param>
        /// <param name="e">Routed event args</param>
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlaying)
            {
                //TODO crash wenn nichts in listbox gewählt ist
                lblTitle.Content = (playlist.SelectedValue).ToString();
                Player.Open(new Uri((playlist.SelectedValue).ToString()));
                Player.Play();
            }
            else
            {
                Player.Pause();
            }

            isPlaying = !isPlaying;
            UpdatePlayButton();
        }

        /// <summary>
        /// Stellt die Lautstärke des Liedes ein.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            Player.Volume = (double)sliVolume.Value;
        }


        /// <summary>
        /// Öffnet ein Windows Fenster wo man ein Song hinzufügen kann.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        string[] files;
        private void btnOpenAudioFile_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            openFileDialog.ShowDialog();

            files = openFileDialog.FileNames;

            foreach (string song in files)
            {
                if (!playlist.Items.Contains(song))
                {
                    playlist.Items.Add(song);
                }
            }

        }

        #endregion
        /// <summary>
        /// Aktualisiert das Play Button Icon
        /// </summary>
        private void UpdatePlayButton()
        {
            //if (isPlaying)
            //{
            //    btnPlay.Content = new BitmapImage(new Uri("//play.png"));
            //}
            //else
            //{
            //    btnPlay.Content = new BitmapImage(new Uri("//paused.png"));
            //}
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnNextSong_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
