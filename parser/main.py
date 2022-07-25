import ffmpeg_streaming
from ffmpeg_streaming import Formats, Representation, Bitrate, Size


import sys
import logging

logging.basicConfig(filename='streaming.log', level=logging.NOTSET, format='[%(asctime)s] %(levelname)s: %(message)s')

def monitor(ffmpeg, duration, time_, time_left, process):
    per = round(time_ / duration * 100)
    sys.stdout.write(
        "\rTranscoding...(%s%%) %s left [%s%s]" %
        (per, datetime.timedelta(seconds=int(time_left)), '#' * per, '-' * (100 - per))
    )
    sys.stdout.flush()



video = ffmpeg_streaming.input("./sample.mp4")
dash = video.hls(Formats.h264())
dash.auto_generate_representations()

try:
    dash.output("/data/out.m3u8")
except Exception as er:
    print(er)