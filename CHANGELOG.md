# CHANGELOG 

Project: Spotify_Recorder

## [%version%] - %date%


## [v2.2] - 17.09.2018 18:46

- The audio router isn't used anymore because Spotify has a hidden feature to play the audio via a specific device.
- Spotify must be started with the \"--enable-audio-graph\" to be able to choose the output device. On startup of this application it is checked if Spotify runs with the correct option.
- The master volume can be changed without affecting the record (because recording from the virtual audio cable ?!).

## [v2.1] - 15.09.2018 19:39

- Record only the sound from Spotify. If another application plays sound, this sound isn't recorded.
- Use VB-Cable (Virtual audio cable) and audio router to record only the sound from Spotify.
- Use saved routings from the audio router, so the router must be only configured once to route the sounds from Spotify to the virtual audio cable.

## [v2.0] - 14.09.2018 22:00

- AssemblyInfoHelper added
- GUI improvements

## [v1.1] - 14.09.2018 20:35

- Initial version of project Spotify_Recorder with source code files

## [v1.0] - 14.09.2018 19:36:21

- Only binary files available (no source code)