using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public VoiceManager voiceManager;

    // Called after login success (web or unity login)
    public void OnLoginSuccess()
    {
        Debug.Log("Login Success → Loading Game Scene...");
        // SceneManager.LoadScene("GameScene");
    }

    // 🎤 UI BUTTON: Hide & Seek Voice
    public void JoinHideAndSeek()
    {
        voiceManager.JoinHideAndSeekVoice();
    }

    // 🎤 UI BUTTON: Trick or Treat Voice
    public void JoinTrickOrTreat()
    {
        voiceManager.JoinTrickOrTreatVoice();
    }

    // 🛑 Stop Voice
    public void StopVoice()
    {
        voiceManager.StopVoice();
    }
}