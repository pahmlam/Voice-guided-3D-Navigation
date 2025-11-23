import socket
import json
import queue
import sounddevice as sd
from vosk import Model, KaldiRecognizer

model = Model("vosk-model-small-en-us-0.15")  
rec = KaldiRecognizer(model, 16000)
q = queue.Queue()

# Hàm thu âm
def callback(indata, frames, time, status):
    if status:
        print(status)
    q.put(bytes(indata))

# Mở socket server để gửi kết quả sang Unity
def start_server():
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind(('localhost', 5000))
    server.listen(1)
    print("Đang chờ Unity kết nối...")
    conn, addr = server.accept()
    print("Unity đã kết nối từ", addr)

    with sd.RawInputStream(samplerate=16000, blocksize=8000, dtype='int16',
                           channels=1, callback=callback):
        print("🎤 Bắt đầu nhận giọng nói...")
        while True:
            data = q.get()
            if rec.AcceptWaveform(data):
                result = json.loads(rec.Result())
                text = result.get("text", "")
                if text:
                    print("Nhận được:", text)
                    conn.sendall((text + '\n').encode())

start_server()
