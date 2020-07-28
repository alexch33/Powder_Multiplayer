using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerControls : MonoBehaviour, IPunObservable
{
    public PhotonView photonView;
    private SpriteRenderer spriteRenderer;
    public Sprite otherSprite;

    private bool isRed;

    public Vector2Int direction;
    public Vector2Int gamePosition;

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

        gamePosition = new Vector2Int((int) transform.position.x, (int) transform.position.y);
        FindObjectOfType<MapController>().AddPlayer(this);

        if (!photonView.IsMine)
        {
            spriteRenderer.sprite = otherSprite;
            spriteRenderer.color = Color.red;
        }
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

        transform.position = Vector3.Lerp(transform.position, (Vector2) gamePosition, Time.deltaTime * 3);
    }
}
