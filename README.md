# SpotifyRecorder

[![GitHub Release Version](https://img.shields.io/github/v/release/M1S2/SpotifyRecorder)](https://github.com/M1S2/SpotifyRecorder/releases/latest)
[![GitHub License](https://img.shields.io/github/license/M1S2/SpotifyRecorder)](LICENSE.md)

This application can be used to record songs from Spotify.

## Installation

1. Download the latest release from [here](https://github.com/M1S2/SpotifyRecorder/releases/latest).
2. Extract the downloaded zip file.
3. Do one of the following:
a) Call setup.exe in the bin_Setup folder to install the SpotifyRecorder. This installs the required VB-Audio cable.
b) Alternatively to 3a. you can copy the bin folder to any location on your computer. Then you have to install and setup the VB-Audio cable by yourself.

## Usage

Configure all neccessary settings (output path, output format, ...) and then arm the recorder.

A new record is started if one of the following conditions is met:
- The recorder is armed, Spotify is playing and the track time is set to 0.
- The recorder is armed, Spotify is paused, the track time is 0 and Spotify is started.
- The recorder is armed, Spotify is playing and the track is changed.

If the track changes and the recorded length is shorter than the expected length (because the track is skipped before it ended or you fast-forwarded it) the record is deleted.

If the record length is correct, the record is normalized (the volume is amplified to the maximum value without clipping), converted to MP3 (if needed) and tagged (infos are added to the file).

The output device of Spotify needs to be set to "CABLE Input (VB-Audio Virtual Cable)" because the record is taken from a virtual audio device (virtual audio cable) to avoid other applications from interrupting the record. 
You can even set the master volume to 0 and the song is still recorded.