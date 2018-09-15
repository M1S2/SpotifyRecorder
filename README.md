# README 

|
| ---------| -----------------------------------------|
| Project: | Spotify_Recorder                   |
| Version: | %version%                                |

## Purpose
This application can be used to record songs from Spotify.

On startup the application checks if Spotify is open and if it was started with the \"--enable-audio-graph\" option. This is neccessary to choose the output device on which Spotify plays.
Configure all neccessary settings (output path, output format, ...) and then arm the recorder.

A new record is started if one of the following conditions is met:
- The recorder is armed, Spotify is playing and the track time is set to 0.
- The recorder is armed, Spotify is paused, the track time is 0 and Spotify is started.
- The recorder is armed, Spotify is playing and the track is changed.

If the track changes and the recorded length is shorter than the expected length (because the track is skipped before it ended or you fast-forwarded it) the record is deleted.

If the record length is correct, the record is normalized (the volume is amplified to the maximum value without clipping), converted to MP3 (if needed) and tagged (infos are added to the file).

The record is taken from a virtual audio device (virtual audio cable) to avoid other applications from interrupting the record. Therefore the sound from Spotify must be played via the audio cable output device.

## Known issues
- If the volume is changing while a record is running, you can hear the volume changes in the recorded song.
- If Spotify is paused while a record is running, you can hear the pause in the recorded song (because of the fading, spotify applies when pausing).