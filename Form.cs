using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WMPLib;

namespace MusicPlayer
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        // Initialize a list to store url of music file
        List<String> musicUrls = new List<String>();

        // Initialize number to record how long a song will be, and now time
        double maxTime, nowTime;

        // Add a new music to play list
        private void btnAddMusic_Click(object sender, EventArgs e)
        {
            // Initialize a new file dialog
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Please choose your mp3 music file";
            // Define the file type can be choosed
            fileDialog.Filter = "(*.mp3)|*.mp3";
            fileDialog.Multiselect = true;

            // When user comfirm choose, add the music
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                // Receive the selected file names
                string[] musicList = fileDialog.FileNames;

                // Iterate to save music name
                foreach (string url in musicList) {
                    // Remove the prefix of mp3 file
                    string musicName = Path.GetFileNameWithoutExtension(url);
                    // Cannot add same music
                    if (listBoxMusics.Items.Contains(musicName)) continue;
                    listBoxMusics.Items.Add(musicName);
                    // Add url to music url list
                    musicUrls.Add(url);
                }
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            // If index is larger or equals 0, means now music is pause, we need to continue play it
            if (listBoxMusics.SelectedIndex >= 0)
            {
                mediaPlayer.Ctlcontrols.play();
                // Enable the timer
                timer.Enabled = true;
                // Update track bar immediately
                updateTrackBar();
            } else
            {
                // If there is not selected music, play first one
                playNewMusic(0);
            };
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            mediaPlayer.Ctlcontrols.pause();
            // Pause the timer
            timer.Enabled = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            mediaPlayer.Ctlcontrols.stop();
            // Pause the timer
            timer.Enabled = false;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            // Get the index of selected music and minus 1 (previous song)
            int previousMusicIndex = listBoxMusics.SelectedIndex - 1;

            // Consider edge cases
            if (previousMusicIndex < 0) return;

            // Play the music
            playNewMusic(previousMusicIndex);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // Get the index of selected music and plus 1 (next song)
            int nextMusicIndex = listBoxMusics.SelectedIndex + 1;

            // Consider edge cases
            if (nextMusicIndex < 0) return;

            // Play the music
            playNewMusic(nextMusicIndex);
        }

        // Select a mumsic name and play
        private void listBoxMusics_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the index of selected music
            int selectedMusicIndex = listBoxMusics.SelectedIndex;

            // Play the music
            playNewMusic(selectedMusicIndex);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // Every second, update the track bar once
            updateTrackBar();
        }

        // Click to drag track bar
        private void trackBar_MouseDown(object sender, MouseEventArgs e)
        {
            // If max time is 0, cannot drag
            if (maxTime <= 0) return;

            // Pause the timer
            timer.Enabled = false;
            // Pause the music
            mediaPlayer.Ctlcontrols.pause();
        }

        // Mouse up to set the time of music
        private void trackBar_MouseUp(object sender, MouseEventArgs e)
        {
            // Get the time after drag
            double nowValue = trackBar.Value;
            // Set the new time
            mediaPlayer.Ctlcontrols.currentPosition = nowValue;
            // Start timer
            timer.Enabled = true;
            // Update track bar immediately
            updateTrackBar();
            // Play music
            mediaPlayer.Ctlcontrols.play();
        }

        // Method to calculate and update track bar
        private void updateTrackBar()
        {
            // Get the time length of the music
            maxTime = mediaPlayer.currentMedia.duration;
            // Get current time of music
            nowTime = mediaPlayer.Ctlcontrols.currentPosition;

            // Update track bar
            trackBar.Maximum = (int)maxTime;
            trackBar.Value = (int)nowTime;

            // Calculate remaining time
            int remainingTimeInt = (int)(maxTime - nowTime);
            String minutes = (remainingTimeInt / 60).ToString();
            String seconds = (remainingTimeInt % 60).ToString();

            remainingTime.Text = minutes + " : " + seconds;
        }

        // Method to play a new music, reuse code
        private void playNewMusic(int selectedMusicIndex)
        {
            // Consider edge cases, not music in list
            if (musicUrls.Count == 0 || selectedMusicIndex > musicUrls.Count - 1) return;

            // Update select index
            listBoxMusics.SelectedIndex = selectedMusicIndex;

            // Update music name text
            lblMusicName.Text = listBoxMusics.SelectedItem.ToString();

            // Get the music url
            string url = musicUrls[selectedMusicIndex];
            // Set url of media player and play
            mediaPlayer.URL = url;

            // Enable the timer
            timer.Enabled = true;
            // Update track bar immediately
            updateTrackBar();
        }
    }
}
