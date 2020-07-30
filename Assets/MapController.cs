using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

public class MapController : MonoBehaviour, IOnEventCallback
{
    public GameObject cellPrefab;

    public PlayersTop playersTop;

    private GameObject[,] cells;
    public List<PlayerControls> players = new List<PlayerControls>();
    public List<PlayerControls> deadPlayers = new List<PlayerControls>();

    private double lastTick;

    public void AddPlayer(PlayerControls player)
    {
        players.Add(player);
        cells[player.gamePosition.x, player.gamePosition.y].SetActive(false);
    }


    // Start is called before the first frame update
    void Start()
    {
        cells = new GameObject[20, 10];

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                cells[x, y] = Instantiate(cellPrefab, new Vector3(x, y), Quaternion.identity, transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.Time > lastTick + 1 &&
            PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            Vector2Int[] directions = players.Where(players => !players.isDead).OrderBy(p => p.photonView.Owner.ActorNumber).Select(p => p.direction).ToArray();

            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOptions = new SendOptions { Reliability = true };

            PhotonNetwork.RaiseEvent(42, directions, options, sendOptions);

            PerformTick(directions);
        }
    }

    public void SendSyncData(Player player)
    {
        SyncData data = new SyncData();

        data.positions = new Vector2Int[players.Count];
        data.scores = new int[players.Count];

        PlayerControls[] sortedPlayers = players.Where(p => !p.isDead).OrderBy(p => p.photonView.Owner.ActorNumber).ToArray();

        for (int i = 0; i < sortedPlayers.Length; i++)
        {
            data.positions[i] = sortedPlayers[i].gamePosition;
            data.scores[i] = sortedPlayers[i].score;
        }

        data.mapData = new BitArray(20 * 10);
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                data.mapData.Set(x + y * cells.GetLength(0), cells[x, y].activeSelf);
            }
        }

        RaiseEventOptions options = new RaiseEventOptions { TargetActors = new[] { player.ActorNumber } };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(43, data, options, sendOptions);

    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 42:
                Vector2Int[] directions = (Vector2Int[])photonEvent.CustomData;

                PerformTick(directions);
                break;
            case 43:
                SyncData data = (SyncData)photonEvent.CustomData;

                StartCoroutine(OnSyncDataRecived(data));
                break;
        }
    }

    private IEnumerator OnSyncDataRecived(SyncData data)
    {
        PlayerControls[] sortedPlayers;
        do
        {
            yield return null;
            sortedPlayers = players.Where(p => !p.isDead).Where(p => !p.photonView.IsMine).OrderBy(p => p.photonView.Owner.ActorNumber).ToArray();
        } while (sortedPlayers.Length != data.positions.Length);


        for (int i = 0; i < sortedPlayers.Length; i++)
        {
            sortedPlayers[i].gamePosition = data.positions[i];
            sortedPlayers[i].score = data.scores[i];

            sortedPlayers[i].transform.position = (Vector2)sortedPlayers[i].gamePosition;
        }

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                bool cellActive = data.mapData.Get(x + y * cells.GetLength(0));
                if (!cellActive) cells[x, y].SetActive(cellActive);
            }
        }
    }

    private void PerformTick(Vector2Int[] directions)
    {
        if (players.Count != directions.Length) return;
        PlayerControls[] sortedPlayers = players.Where(players => !players.isDead).OrderBy(p => p.photonView.Owner.ActorNumber).ToArray();

        int i = 0;
        foreach (PlayerControls player in sortedPlayers)
        {
            player.direction = directions[i++];
            MinePlayerBlock(player);
        }

        foreach (PlayerControls player in sortedPlayers)
        {
            MovePlayer(player);
        }

        foreach (PlayerControls player in players.Where(players => players.isDead))
        {
            Vector2Int pos = player.gamePosition;

            while (pos.y > 0 && !cells[pos.x, pos.y].activeSelf)
            {
                pos.y--;
            }
            player.gamePosition = pos;
        }

        playersTop.SetTexts(players);

        lastTick = PhotonNetwork.Time;
    }

    private void MinePlayerBlock(PlayerControls player)
    {
        if (player.direction == Vector2Int.zero)
        {
            return;
        }

        Vector2Int targetPosition = player.gamePosition + player.direction;

        if (targetPosition.x < 0) return;
        if (targetPosition.y < 0) return;
        if (targetPosition.x >= cells.GetLength(0)) return;
        if (targetPosition.y >= cells.GetLength(1)) return;

        if (cells[targetPosition.x, targetPosition.y].activeSelf)
        {
            cells[targetPosition.x, targetPosition.y].SetActive(false);
            player.score++;
        }

        Vector2Int pos = targetPosition;
        PlayerControls minePlayer = players.First(p => p.photonView.IsMine);

        if (minePlayer != player)
        {
            while (pos.y < cells.GetLength(1) && !cells[pos.x, pos.y].activeSelf)
            {
                if (pos == minePlayer.gamePosition)
                {
                    minePlayer.Kill();
                    PhotonNetwork.LeaveRoom();
                    break;
                }
                pos.y++;
            }
        }


    }

    private void MovePlayer(PlayerControls player)
    {
        player.gamePosition += player.direction;

        if (player.gamePosition.x < 0) player.gamePosition.x = 0;
        if (player.gamePosition.y < 0) player.gamePosition.y = 0;
        if (player.gamePosition.x >= cells.GetLength(0)) player.gamePosition.x = cells.GetLength(0) - 1;
        if (player.gamePosition.y >= cells.GetLength(1)) player.gamePosition.y = cells.GetLength(1) - 1;

        int ladderLength = 0;
        Vector2Int pos = player.gamePosition;

        while (pos.y > 0 && !cells[pos.x, pos.y].activeSelf)
        {
            ladderLength++;
            pos.y--;
        }
        player.SetLadderLength(ladderLength);
    }

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
