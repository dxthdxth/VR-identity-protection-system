using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Unity.Netcode;

public class MeetingManager : MonoBehaviour
{
    public string serverURL = "http://192.168.1.78:5000";
    public string username;
    public string token;
    public string meetingID;

    // CREATE MEETING (HOST)
    public void CreateMeeting()
    {
        StartCoroutine(CreateMeetingRequest());
    }

    IEnumerator CreateMeetingRequest()
    {
        string json = "{\"username\":\"" + username + "\",\"token\":\"" + token + "\"}";

        UnityWebRequest request = new UnityWebRequest(serverURL + "/create_meeting", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Meeting Created: " + request.downloadHandler.text);

            NetworkManager.Singleton.StartHost(); // YOU are host
        }
    }

    // JOIN MEETING (CLIENT)
    public void JoinMeeting()
    {
        StartCoroutine(JoinMeetingRequest());
    }

    IEnumerator JoinMeetingRequest()
    {
        string json = "{\"username\":\"" + username + "\",\"token\":\"" + token + "\",\"meeting_id\":\"" + meetingID + "\"}";

        UnityWebRequest request = new UnityWebRequest(serverURL + "/join_meeting", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Joined Meeting");

            NetworkManager.Singleton.StartClient(); // FRIEND joins
        }
    }
}