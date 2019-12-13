# SpotifyRecorder

## [%version%] - %date%

- Show playlist name and multiple artists if present
- Change active media player Issue  [#2](https://github.com/M1S2/SpotifyRecorder/issues/2) fixed
- Renew access token Issue [#3](https://github.com/M1S2/SpotifyRecorder/issues/3) fixed
- GUI: Settings moved to separate Flyout
- Recording device is now configurable
- Option to delete paused records added
- Several small bugfixes

## [v3.2.0] - 07.09.2019 20:19

- Fixed bug in v3.1.0
- Connecting to Spotify is now silent because it doesn't use the default browser anymore. An internal WebBrowser is used.

## [v3.1.0] - 29.08.2019 21:31

**This version contains a bug (Ad is always recogniced when the track is changed)**
- Splash screen added
- Saving window position
- Adblocker improved

## [v3.0.0] - 17.07.2019 21:20

- Spotify Web API used to get the users playback state (Local API is out of service since about July 2018).
- Metro style added using WPF.
- Generic recorder and player implemented to make the implementation of a new player application easy (if Spotify isn't working anymore like happened with the local API).
- Parallel recorders (no pause between tracks neccessary).

## [v2.3.0] - 21.09.2018 20:08

- Remove the fading that Spotify applies when pausing a song. This works almost perfect. Sometimes the transition between pause and play can be heared a little bit. But this is insignificant. To get perfect records, don't pause while recording!
- Added feature to save all log box entries.

## [v2.2.0] - 17.09.2018 18:46

- The audio router isn't used anymore because Spotify has a hidden feature to play the audio via a specific device.
- Spotify must be started with the \"--enable-audio-graph\" to be able to choose the output device. On startup of this application it is checked if Spotify runs with the correct option.
- The master volume can be changed without affecting the record (because recording from the virtual audio cable ?!).

## [v2.1.0] - 15.09.2018 19:39

- Record only the sound from Spotify. If another application plays sound, this sound isn't recorded.
- Use VB-Cable (Virtual audio cable) and audio router to record only the sound from Spotify.
- Use saved routings from the audio router, so the router must be only configured once to route the sounds from Spotify to the virtual audio cable.

## [v2.0.0] - 14.09.2018 22:00

- AssemblyInfoHelper added
- GUI improvements

## [v1.1.0] - 14.09.2018 20:35

- Initial version of project Spotify_Recorder with source code files

## [v1.0.0] - 14.09.2018 19:36:21

- Only binary files available (no source code)