#!/bin/bash
./yt-dlp --default-search ytsearch -o - "'$*'" | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1
