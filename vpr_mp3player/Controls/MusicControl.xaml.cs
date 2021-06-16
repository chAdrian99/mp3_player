﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;

namespace vpr_mp3player.Controls
{
    /// <summary>
    /// Interaktionslogik für MusicControl.xaml
    /// </summary>
    public partial class MusicControl : UserControl
    {


        private bool userIsDraggingSlider = false;

        private bool isPlaying = false;
        //
        public double sliderCurrentTime;
        
        private MediaPlayer _player = new MediaPlayer();

        public List<Song> Songs { get; set; }

        public MediaPlayer Player
        {
            get { return _player; }
            set { _player = value; }
        }

        public MusicControl()
        {
            InitializeComponent();
            Player.Volume = (double)sliVolume.Value;
            Songs = new List<Song>();
            UpdatePlayButton();
        }

        #region Events

        /// <summary>
        /// Button click event for play
        /// </summary>
        /// <param name="sender">Sender element</param>
        /// <param name="e">Routed event args</param>
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.Items.Count <= 0)
            {
                return;
            }

            if (!isPlaying)
            {
                //TODO crash wenn nichts in listbox gewählt ist
                if (playlist.SelectedItem == null)
                {
                    return;
                }

                Song selectedSong = playlist.SelectedItem as Song;

                lblTitle.Content = selectedSong.Title.ToString();
                Player.Open(new Uri(selectedSong.Path));
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
            float volUpdate = (float)(Math.Sqrt(sliVolume.Value) / 10);
            Player.Volume = volUpdate;
        }

        /// <summary>
        /// Öffnet ein Windows Fenster wo man ein Song hinzufügen kann.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenAudioFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            openFileDialog.ShowDialog();

            var path = openFileDialog.FileNames[0];
            var title = path.Split('\\').Last().Split('.').First();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            Songs.Add(new Song()
            {
                Title = title,
                Path = path,
            });
            
            playlist.ItemsSource = Songs;
        }

        void timer_Tick(object sender, EventArgs e)
        {

            if ((Player.Source != null) && (Player.NaturalDuration.HasTimeSpan))
            {
                sliDuration.Minimum = 0;
                sliDuration.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
                sliDuration.Value = Player.Position.TotalSeconds;

                lblCurrentTime.Content = String.Format("{0} / {1}", Player.Position.ToString(@"mm\:ss"), Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));

                lblEndTime.Content = String.Format(Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
            {
                lblCurrentTime.Content = "0:00";
            }
        }

        #endregion
        /// <summary>
        /// Aktualisiert das Play Button Icon
        /// </summary>
        private void UpdatePlayButton()
        {
            if (!isPlaying)
            {
                imgPlay.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/PlayImageMp3.png"));
            }
            else
            {
                imgPlay.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/PauseImageMp3.png"));
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void btnNextSong_Click(object sender, RoutedEventArgs e)
        {

        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliDuration_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            Player.Position = TimeSpan.FromSeconds(sliDuration.Value);
        }

        private void sliDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblCurrentTime.Content = TimeSpan.FromSeconds(sliDuration.Value).ToString(@"hh\:mm\:ss");
        }




    }
}
