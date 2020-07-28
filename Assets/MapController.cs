using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public GameObject cellPrefab;

    private GameObject[,] cells;
    private List<PlayerControls> players = new List<PlayerControls>();

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
        
    }
}
