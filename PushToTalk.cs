using UnityEngine;
using Photon.Voice.Unity;

public class PushToTalk : MonoBehaviour
{
    private Recorder recorder;

    void Start()
    {
        recorder = GetComponent<Recorder>();

        if (recorder == null)
        {
            Debug.LogError("Recorder not found on this GameObject!");
        }
    }

    void Update()
    {
        if (recorder == null) return;

        if (Input.GetKeyDown(KeyCode.V))
        {
            recorder.TransmitEnabled = true;
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            recorder.TransmitEnabled = false;
        }
    }
}