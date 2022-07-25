FROM python:3.9

RUN pip3 install pipenv

RUN apt-get update && apt-get install -y \
    ffmpeg \
    && rm -rf /var/lib/apt/lists/*



WORKDIR /
COPY . .

RUN pip3 install -r requirements.txt
# python-ffmpeg-video-streaming

CMD ["python", "main.py"]