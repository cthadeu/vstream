import datetime
from email.mime import base
import ffmpeg_streaming
from ffmpeg_streaming import Formats, Representation, Bitrate, Size
import pika
import base64
import sys
import logging

FORMAT = '%(asctime)s %(clientip)-15s %(user)-8s %(message)s'
logging.basicConfig(format=FORMAT)
QUEUE_NAME = 'video-stream-requested'

def monitor(ffmpeg, duration, time_, time_left, process):  
    per = round(time_ / duration * 100)
    sys.stdout.write(
        "\rTranscoding...(%s%%) %s left [%s%s]" %
        (per, datetime.timedelta(seconds=int(time_left)), '#' * per, '-' * (100 - per))
    )
    sys.stdout.flush()

def create_video_stream(filename):
    try:
        s = filename.decode()
        logging.info("Creating streamming for %s" % s)
        video = ffmpeg_streaming.input("/data/%s" % (s))
        dash = video.hls(Formats.h264())
        dash.auto_generate_representations()
        newName =  s.split(".")[0]
        logging.info("stream name %s" % newName)
        dash.output("/data/%s.m3u8" % (newName), monitor=monitor)
    except Exception as er:
        logging.info("ERROR %r" % er)

def message_callback(ch, method, properties, body):         
    logging.info("Message received %r" % body)     
    create_video_stream(body)

def main():    
    try:
        logging.info("Waiting for new messages")
        connection = pika.BlockingConnection(pika.ConnectionParameters(host="rabbitmq"))
        channel = connection.channel()    
        channel.queue_declare(queue=QUEUE_NAME)
        channel.basic_consume(queue=QUEUE_NAME, auto_ack=True, on_message_callback=message_callback)        
        channel.start_consuming()
    except Exception as er:
        print(er)


if __name__ == "__main__":
    main()