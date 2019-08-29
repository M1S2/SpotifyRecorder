# SpotifyRecorder

Version: v3.1.0

## Purpose

This application can be used to record songs from Spotify.

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