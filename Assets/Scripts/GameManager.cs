using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    public MapController mapController;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = new Vector3(UnityEngine.Random.Range(1, 15), UnityEngine.Random.Range(1, 5));
        PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
        PhotonPeer.RegisterType(typeof(Vector2Int), 242, SerializeVector2int, DeserializeVector2Int);
    }

    public static object DeserializeVector2Int(byte[] data)
    {
        Vector2Int result = new Vector2Int();
        result.x = BitConverter.ToInt32(data, 0);
        result.y = BitConverter.ToInt32(data, 4);

        return result;
    }

    public static byte[] SerializeVector2int(object data)
    {
        byte[] result = new byte[8];
        Vector2Int vector2Int = (Vector2Int)data;

        BitConverter.GetBytes(vector2Int.x).CopyTo(result, 0);
        BitConverter.GetBytes(vector2Int.y).CopyTo(result, 4);

        return result;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        mapController.players = mapController.deadPlayers;

        PlayerControls player = mapController.players.FirstOrDefault(p=>p.photonView.Owner.ActorNumber == otherPlayer.ActorNumber);

        if (player != null) player.Kill();
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }
}
