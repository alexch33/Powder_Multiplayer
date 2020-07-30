using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Text logText;
    public TMP_InputField NickName;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.NickName = PlayerPrefs.GetString("nick", "Player" + Random.Range(1000, 9000));
        NickName.text = PhotonNetwork.NickName;
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
        if (NickName.text != null && NickName.text.Length > 0)
        {
            PlayerPrefs.SetString("nick", NickName.text);
            PhotonNetwork.NickName = NickName.text;
        }
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 20, CleanupCacheOnLeave = false });
    }

    public void JoinRoom()
    {
        Log("Joining room...");
        if (NickName.text != null && NickName.text.Length > 0)
        {
            PlayerPrefs.SetString("nick", NickName.text);
            PhotonNetwork.NickName = NickName.text;
        }
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
