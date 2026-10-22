using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ConnectToPhoton : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom(
            "GameRoom",
            new RoomOptions(),
            TypedLobby.Default
        );
    }
}