# YoutubeCutter

A graphical wrapper of youtube-dl and ffmpeg using WPF. Normally, one can download clips of a youtube video by adding postprocess args to youtube-dl, however youtube-dl downloads the whole video first, then cut it. The workaround is to use the link given by youtube-dl as a source, and use ffmpeg to copy to the output file. This program takes care of the steps, and simplify the clipping process.
