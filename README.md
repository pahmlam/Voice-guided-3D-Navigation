# Voice-Guided 3D Navigation

A voice-controlled 3D simulation where a user can command an agent to navigate a complex environment using natural language. This project integrates **Unity** for the 3D frontend and **Python (Vosk)** for offline speech recognition, communicating via **TCP Sockets**.

## 🚀 Features

  * **Offline Speech Recognition:** Uses the Vosk API to process voice commands without an internet connection.
  * **Real-time TCP Communication:** Seamless low-latency data transmission between the Python server and Unity client.
  * **Smart Navigation:** Utilizes Unity's NavMesh system for intelligent pathfinding.
  * **Context-Aware Commands:** The system parses natural language (e.g., "Go to the table", "Approach the chair") and maps them to specific 3D objects.

## 🛠 Tech Stack

  * **Frontend:** Unity 6 (or 2022+), AI Navigation Package.
  * **Backend:** Python 3.x.
  * **Speech Engine:** Vosk (Kaldi-based).
  * **Communication:** TCP/IP Sockets (Localhost).

## 📂 Project Structure

```
├── UnityProject/
│   ├── Assets/Scripts/
│   │   ├── SpeechClient.cs          # TCP Client receiving text from Python
│   │   ├── VoiceCommandProcessor.cs # Parses text and triggers navigation
│   │   ├── NavigationController.cs  # Wrapper for NavMeshAgent logic
│   │   ├── ObjectRegistry.cs        # Manages interactable scene objects
│   │   └── ObjectAnchor.cs          # Defines object keywords (e.g., "table")
│   └── ...
├── PythonServer/
│   ├── voice_server.py              # Captures audio & runs Vosk inference
│   ├── vosk-model-small-en-us-0.15/ # The generic language model
│   └── ...
```

## ⚙️ Installation & Setup

### 1\. Python Backend (Speech Server)

**Prerequisites:**

  * Python 3.x installed.
  * A microphone connected.

**Steps:**

1.  Install required libraries:
    ```bash
    pip install vosk sounddevice
    ```
2.  Download a Vosk model (e.g., `vosk-model-small-en-us-0.15`) from [Vosk Models](https://alphacephei.com/vosk/models).
3.  Extract the model into the same folder as `voice_server.py`.
4.  Run the server:
    ```bash
    python voice_server.py
    ```
    *Wait until you see: `Đang chờ Unity kết nối...`*

### 2\. Unity Frontend

**Prerequisites:**

  * Unity Hub & Editor installed.
  * **AI Navigation** package installed (via Package Manager).

**Steps:**

1.  Open the project in Unity.
2.  **Important:** Setup the Navigation Mesh.
      * Select the **Plane** (Environment floor).
      * Add the `NavMeshSurface` component.
      * Click **Bake** to generate the blue navigation map.
3.  Ensure your `GameManager` object has the `SpeechClient`, `VoiceCommandProcessor`, and `ObjectRegistry` scripts attached and linked correctly in the Inspector.
4.  Press **Play**.

## 🎮 How to Use

1.  Start the Python server first.
2.  Start the Unity scene.
3.  Check the Console for the "Connected to Python" message.
4.  Speak a command clearly into your microphone.

### Supported Commands

The system recognizes the following patterns:

  * **Navigation:**
      * *"Go to [object]"* (e.g., "Go to table")
      * *"Move to [object]"*
      * *"Approach [object]"*
      * *Shortcut:* Just saying the object name (e.g., "Table") also works.
  * **Control:**
      * *"Stop"* (Immediately halts the character).

## 🔧 Troubleshooting

  * **Character doesn't move:**
      * Ensure the NavMesh is baked (The floor should look blue in the Scene view).
      * Check if the Player's Y position is slightly above the floor (not sinking).
      * Verify the target object has the `ObjectAnchor` script and the correct keyword in the Inspector.
  * **Unity not receiving text:**
      * Ensure `voice_server.py` is running **before** you press Play in Unity.
      * Check firewall settings for port `5000`.

## Team role
* Trần Trang Linh: Voice processing.
* Phạm Tùng Lâm: Navigation processing.
* Nguyễn Minh Huyền: Python-Unity connection, animation processing.
## 📜 License
Distributed under the MIT License. See `LICENSE.txt` for more information.

Copyright (c) 2025 Tran Trang Linh, Nguyen Minh Huyen, Pham Tung Lam


