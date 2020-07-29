using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayersTop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in GetComponentsInChildren<Text>())
        {
            item.text = "";
        }
    }

    // Update is called once per frame
    public void SetTexts(List<PlayerControls> players)
    {
        PlayerControls[] top = players.Where(p => !p.isDead).OrderByDescending(p => p.score).Take(5).ToArray();

        for (int i = 0; i < top.Length; i++)
        {
            transform.GetChild(i).GetComponent<Text>().text = (i + 1) + ". " + top[i].photonView.Owner.NickName + "   " + top[i].score;
        }
    }
}
