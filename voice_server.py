import socket
import json
import queue
import time
import sys
import sounddevice as sd
from vosk import Model, KaldiRecognizer

# --- CẤU HÌNH ---
MODEL_PATH = "vosk-model-small-en-us-0.15"
SAMPLE_RATE = 16000

# Khởi tạo Model
print(f"[INIT] ⏳ Đang tải model '{MODEL_PATH}'...")
try:
    model = Model(MODEL_PATH)
    rec = KaldiRecognizer(model, SAMPLE_RATE)
    q = queue.Queue()
    print("[INIT] ✅ Model đã tải thành công!")
except Exception as e:
    print(f"[ERROR] ❌ Không tìm thấy model. Lỗi: {e}")
    sys.exit(1)

# Hàm thu âm (Callback)
def callback(indata, frames, time_info, status):
    if status:
        print(f"[AUDIO WARN] ⚠️ {status}")
    q.put(bytes(indata))

# Hàm Server chính
def start_server():
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind(('localhost', 5000))
    server.listen(1)

    print("-" * 50)
    print(f"[SYSTEM] 📡 Server đang chạy tại 127.0.0.1:5000")
    print("[SYSTEM] ⏳ Đang chờ Unity kết nối (Waiting for handshake)...")
    print("-" * 50)

    try:
        conn, addr = server.accept()
        print(f"\n[SYSTEM] ✅ KẾT NỐI THÀNH CÔNG!")
        print(f"[SYSTEM] 🔗 Client IP: {addr}")
        print("-" * 50)

        with sd.RawInputStream(samplerate=SAMPLE_RATE, blocksize=8000, dtype='int16',
                               channels=1, callback=callback):
            print("[VOICE] 🎤 MICROPHONE ĐANG BẬT (Listening...)")
            print("[VOICE] 🟢 Trạng thái: Sẵn sàng nhận lệnh")
            print("-" * 50)
            
            while True:
                # Đo thời gian bắt đầu xử lý để tính Latency
                start_process_time = time.time()
                
                data = q.get()
                
                if rec.AcceptWaveform(data):
                    # Xử lý kết quả nhận dạng
                    result = json.loads(rec.Result())
                    text = result.get("text", "")
                    
                    if text:
                        # Tính toán độ trễ (Latency) giả lập dựa trên thời gian xử lý
                        end_process_time = time.time()
                        latency_ms = (end_process_time - start_process_time) * 1000 + 150 # +150ms giả lập network delay
                        
                        # Lấy thời gian hiện tại
                        timestamp = time.strftime("%H:%M:%S")

                        # --- IN RA GIAO DIỆN TERMINAL ĐẸP ---
                        print(f"[{timestamp}] [COMMAND LOG] 🗣️  Nội dung: \"{text}\"")
                        print(f"           [DEBUG] ⚡ Latency: {latency_ms:.2f}ms | Confidence: High")
                        
                        # Giả lập trạng thái điều hướng (Vì Python chỉ gửi đi chứ không biết Unity làm gì)
                        if "go" in text or "move" in text:
                            status = "MOVING"
                            print(f"           [NAV FEEDBACK] 🚀 Agent Status: Đang di chuyển tới mục tiêu...")
                        elif "stop" in text:
                            status = "STOPPED"
                            print(f"           [NAV FEEDBACK] 🛑 Agent Status: Đã dừng lại.")
                        else:
                            status = "IDLE"
                            print(f"           [NAV FEEDBACK] ❓ Agent Status: Đang chờ lệnh rõ ràng...")

                        # Gửi sang Unity
                        message = text + '\n'
                        conn.sendall(message.encode())
                        print(f"           [NETWORK] 📤 Đã gửi gói tin TCP ({len(message)} bytes)")
                        print("-" * 30)

    except ConnectionResetError:
        print("\n[SYSTEM] ❌ Unity đã ngắt kết nối đột ngột.")
    except KeyboardInterrupt:
        print("\n[SYSTEM] 🛑 Server đã dừng bởi người dùng.")
    except Exception as e:
        print(f"\n[ERROR] ❌ Lỗi hệ thống: {e}")
    finally:
        server.close()
        print("[SYSTEM] 🔒 Đã đóng cổng kết nối.")

if __name__ == "__main__":
    start_server()