using Microsoft.Win32;
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

        private bool loop = false;
        
        private bool shuffle = false;

        public double sliderCurrentTime;
        
        private MediaPlayer _player = new MediaPlayer();

        Random rndShuffle = new Random();

        private int currentSongIndex = 0;

        public int CurrentSongIndex
        {
            get { return currentSongIndex; }
            set { currentSongIndex = value; }
        }

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

        private void songNextMethod()
        {
            Uri pathToSong = new Uri(Songs[CurrentSongIndex].Path);
            lblTitle.Content = Songs[CurrentSongIndex].Title.ToString();
            Player.Open(pathToSong);
            Player.Play();
        }

        #region Events

        /// <summary>
        /// Button click event for play
        /// </summary>
        /// <param name="sender">Sender element</param>
        /// <param name="e">Routed event args</param>

        int active = 0;
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

                CurrentSongIndex = Array.IndexOf(Songs.ToArray(), selectedSong);

                if(active == 0)
                {
                    Player.Open(new Uri(selectedSong.Path));
                    Player.Play();
                }
                lblTitle.Content = selectedSong.Title.ToString();
                Player.Play();
            }
            else
            {
                Player.Pause();
            }

            active++;
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
            
            playlist.Items.Clear();
            foreach(var song in Songs)
            {
                playlist.Items.Add(song);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {

            if ((Player.Source != null) && (Player.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
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
                lblEndTime.Content = "0:00";
            }

            //next song when ended
            if (sliDuration.Value == sliDuration.Maximum)
            {
                //loop active
                if (loop && !shuffle)
                {
                    Player.Position = TimeSpan.Zero;
                    Player.Play();
                }
                //shuffle active
                else if (shuffle && !loop)
                {
                    int shuffleSong = rndShuffle.Next(0, Songs.Count);
                    shuffleSong = CurrentSongIndex;
                    songNextMethod();
                }
                //go next in q
                else
                {
                    if(CurrentSongIndex < Songs.Count())
                    {
                        CurrentSongIndex++;
                        songNextMethod();
                    }
                    else
                    {
                        return;
                    }
                }
                //TODO CurrentSongIndex in eigene Funktion, dann jedesmal aufrufen mit Index++/--/shuffle
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

        private void btnLastSong_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSongIndex > 0)
            {

                CurrentSongIndex--;
                songNextMethod();
            }

            else
            {
                return;
            }
        }

        private void btnNextSong_Click(object sender, RoutedEventArgs e)
        {

            if(CurrentSongIndex < Songs.Count() -1)
            {
                CurrentSongIndex++;
                songNextMethod();
            }

            else
            {
                return;
            }
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

        int loopClick;
        private void btnLoop_Click(object sender, RoutedEventArgs e)
        {
            loopClick++;

            if (loopClick % 2 == 1)
            {
                loop = true;
            }
            else
            {
                loop = false;

            }
        }

        int shuffleClick;
        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            shuffleClick++;

            if(shuffleClick % 2 == 1)
            {
                shuffle = true;
            }
            else
            {
                shuffle = false;
            }
        }
    }
}
