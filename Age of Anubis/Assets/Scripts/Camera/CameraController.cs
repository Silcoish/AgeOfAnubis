using UnityEngine;
 
public class CameraController : MonoBehaviour
{
	[Header("Shop Scene Variables")]
	[Tooltip("Only set this in the shop scene")]
	public GameObject room;
	[Tooltip("Only set this in the shop scene")]
	public GameObject shopPlayer;
	Bounds roomBounds;

    public bool m_isBossCam;

    public Transform m_playerTrans;
    public Vector2 m_playerOffsetPos;
    public Transform m_bossTrans;

    public float m_transOffset;

    float m_playerForwardOffset = 2;

    Vector2 m_min = Vector2.zero;
    Vector2 m_max = Vector2.zero;
    Vector2 m_center = Vector2.zero;
    Vector2 m_size = Vector2.zero;

    float m_lerpSpeed = 2f;

    public float scalarX;
    public float scalarY;



	Transform t;

	float vertical;
	float horizontal;

	public float lerpTime = 0.1f;

	void Start()
	{
		t = transform;
		vertical = gameObject.GetComponent<Camera>().orthographicSize * 2;
		horizontal = vertical * Screen.width / Screen.height;

		if (room != null)
			roomBounds = room.GetComponent<BoxCollider2D>().bounds;

		if(shopPlayer == null)
			shopPlayer = GameManager.inst.player;
	}

	void Update()
	{
        if (m_isBossCam)
        {
            if (room != null)
            {
                if (shopPlayer == null)
                   m_playerTrans = GameManager.inst.player.transform;

                if (m_playerTrans != null && m_bossTrans != null)
                {
                    m_playerOffsetPos = m_playerTrans.position + (Vector3.right * m_playerForwardOffset * m_playerTrans.localScale.x);
                    //Get bounds of player and anubis
                    if (m_playerOffsetPos.x < m_bossTrans.position.x)
                    {
                        m_min.x = m_playerOffsetPos.x - m_transOffset;
                        m_max.x = m_bossTrans.position.x + m_transOffset;
                    }
                    else
                    {
                        m_min.x = m_bossTrans.position.x - m_transOffset;
                        m_max.x = m_playerOffsetPos.x + m_transOffset;
                    }

                    if (m_playerOffsetPos.y < m_bossTrans.position.y)
                    {
                        m_min.y = m_playerOffsetPos.y - m_transOffset;
                        m_max.y = m_bossTrans.position.y + m_transOffset;
                    }
                    else
                    {
                        m_min.y = m_bossTrans.position.y - m_transOffset;
                        m_max.y = m_playerOffsetPos.y + m_transOffset;
                    }

                    //Check Bounds of room
                    m_max.x = Mathf.Min(m_max.x, roomBounds.max.x);
                    m_max.y = Mathf.Min(m_max.y, roomBounds.max.y);

                    m_min.x = Mathf.Max(m_min.x, roomBounds.min.x);
                    m_min.y = Mathf.Max(m_min.y, roomBounds.min.y);

                    //Find Center
                    m_center.x = (m_min.x + m_max.x) / 2f;
                    m_center.y = (m_min.y + m_max.y) / 2f;

                    scalarX = (m_max.x - m_min.x) / 16f;
                    scalarY = (m_max.y - m_min.y) / 9f;

                    //Make sure size is never less than min size
                    scalarX = Mathf.Max(scalarX, (10f / 9f));
                    scalarY = Mathf.Max(scalarY, (10f / 9f));

                    //find What Axis is the smaller
                    if (scalarY > scalarX)
                    {
                        //Y is the scaler axis.
                        m_size.y = scalarY * 9f;
                        m_size.x = scalarY * 16f;
                    }
                    else
                    {
                        //X is the scaler axis.
                        m_size.y = scalarX * 9f;
                        m_size.x = scalarX * 16f;
                    }

                    //Ensure Camera never leaves the room bounds
                    m_center.x = Mathf.Min(m_center.x, roomBounds.max.x - (m_size.x / 2));
                    m_center.x = Mathf.Max(m_center.x, roomBounds.min.x + (m_size.x / 2));

                    m_center.y = Mathf.Min(m_center.y, roomBounds.max.y - (m_size.y / 2));
                    m_center.y = Mathf.Max(m_center.y, roomBounds.min.y + (m_size.y / 2));

                    Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, m_size.y / 2, Time.deltaTime * m_lerpSpeed);

                    transform.position =  Vector3.Lerp(transform.position,  new Vector3(m_center.x, m_center.y, transform.position.z) , Time.deltaTime * 5);





                }


            }
        }
        else
        {
            if (room != null)
            {
                if (shopPlayer == null)
                    shopPlayer = GameManager.inst.player;
                if (shopPlayer == null)
                    m_playerTrans = GameManager.inst.player.transform;
                //set the position to the player position, then move it later to adjust for walls
                //t.position = new Vector3(shopPlayer.transform.position.x, shopPlayer.transform.position.y, t.position.z);
                t.position = Vector2.Lerp(t.position, shopPlayer.transform.position + (Vector3.right * 1.5f * m_playerForwardOffset * m_playerTrans.localScale.x), Time.deltaTime * 2);
                t.position = new Vector3(t.position.x, t.position.y, -10f);

                Vector2 pos = new Vector2(t.position.x, t.position.y);

                #region Horizontal
                if (t.position.x + horizontal / 2 > room.transform.position.x + (roomBounds.size.x / 2))
                {
                    pos.x = room.transform.position.x + (roomBounds.size.x / 2) - (horizontal / 2);
                }
                else if (transform.position.x - horizontal / 2 < room.transform.position.x - (roomBounds.size.x / 2))
                {
                    pos.x = room.transform.position.x - (roomBounds.size.x / 2) + (horizontal / 2);
                }
                #endregion
                #region Vertical
                if (t.position.y - vertical / 2 < room.transform.position.y - (roomBounds.size.y / 2))
                {
                    pos.y = room.transform.position.y - (roomBounds.size.y / 2) + (vertical / 2);
                }
                else if (t.position.y + vertical / 2 > room.transform.position.y + (roomBounds.size.y / 2))
                {
                    pos.y = room.transform.position.y + (roomBounds.size.y / 2) - (vertical / 2);
                }
                #endregion

                t.position = new Vector3(pos.x, pos.y, t.position.z);
            }
        }
	}

	public void SetRoom(GameObject r)
	{
		room = r;
		roomBounds = r.GetComponent<BoxCollider2D>().bounds;
	}
}