using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerControls : MonoBehaviour, IPunObservable
{
    private PhotonView photonView;
    private SpriteRenderer spriteRenderer;

    private bool isRed;

    private Vector2Int direction;
    // Start is called before the first frame update

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(GameManager.SerializeVector2int(direction));
        }
        else
        {
            direction = (Vector2Int) GameManager.DeserializeVector2Int((byte[]) stream.ReceiveNext());
        }
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) direction = Vector2Int.left;
            if (Input.GetKey(KeyCode.RightArrow)) direction = Vector2Int.right;
            if (Input.GetKey(KeyCode.UpArrow)) direction = Vector2Int.up;
            if (Input.GetKey(KeyCode.DownArrow)) direction = Vector2Int.down;
        }

        if (direction == Vector2Int.left)
        {
            spriteRenderer.flipX = true;
        }
        if (direction == Vector2Int.right)
        {
            spriteRenderer.flipX = false;
        }
    }
}
