using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Text logText;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.NickName = "Player" + Random.Range(1000, 9000);
        Log("Player's name set to " + PhotonNetwork.NickName);

        PhotonNetwork.GameVersion = "1";

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Log("Connected to master");
    }

    public void CreateRoom()
    {
        Log("Creaing room...");

        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2, CleanupCacheOnLeave = false });
    }

    public void JoinRoom()
    {
        Log("Joining room...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Log("Joined to room");
        PhotonNetwork.LoadLevel("Game");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Log(string message)
    {
        Debug.Log(message);
        logText.text += "\n";
        logText.text += message;
    }
}
