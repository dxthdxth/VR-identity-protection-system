using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;

public class VoiceManager : MonoBehaviourPunCallbacks
{
    public VoiceConnection voiceConnection;
    public Recorder recorder;

    private bool voiceStarted = false;

    void Awake()
    {
        // 🔥 PREVENT DUPLICATE VOICE MANAGERS
        if (FindObjectsOfType<VoiceManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (recorder != null)
        {
            recorder.TransmitEnabled = false;
        }
    }

    // 🎮 ROOM JOIN (Hide & Seek)
    public void JoinHideAndSeekVoice()
    {
        JoinRoom("HideAndSeekRoom");
    }

    // 🎮 ROOM JOIN (Trick or Treat)
    public void JoinTrickOrTreatVoice()
    {
        JoinRoom("TrickOrTreatRoom");
    }

    void JoinRoom(string roomName)
    {
        Debug.Log("Joining Room: " + roomName);

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 20
        };

        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);

        StartVoice();
    }

    void StartVoice()
    {
        if (voiceStarted) return;
        voiceStarted = true;

        if (voiceConnection == null || recorder == null)
        {
            Debug.LogError("VoiceConnection or Recorder missing!");
            return;
        }

        // 🔥 SAFE START
        if (!voiceConnection.Client.IsConnected)
        {
            voiceConnection.ConnectUsingSettings();
        }

        recorder.TransmitEnabled = true;

        Debug.Log("🎤 Voice Started Safely");
    }

    public void StopVoice()
    {
        voiceStarted = false;

        if (recorder != null)
            recorder.TransmitEnabled = false;

        if (voiceConnection != null && voiceConnection.Client != null)
        {
            voiceConnection.Client.Disconnect();
        }

        Debug.Log("🛑 Voice Stopped");
    }
}