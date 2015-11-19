using UnityEngine;
 
public class CameraController : MonoBehaviour
{
	[Header("Shop Scene Variables")]
	[Tooltip("Only set this in the shop scene")]
	public GameObject room;
	[Tooltip("Only set this in the shop scene")]
	public GameObject shopPlayer;
	Bounds roomBounds;

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

		shopPlayer = GameManager.inst.player;
	}

	void Update()
	{
		if(room != null)
		{
			//set the position to the player position, then move it later to adjust for walls
			//t.position = new Vector3(shopPlayer.transform.position.x, shopPlayer.transform.position.y, t.position.z);
			t.position = Vector2.Lerp(t.position, shopPlayer.transform.position, lerpTime);
			t.position = new Vector3(t.position.x, t.position.y, -10f);

			Vector2 pos = new Vector2(t.position.x, t.position.y);

			#region Horizontal
			if (t.position.x + horizontal / 2 > room.transform.position.x + (roomBounds.size.x / 2))
			{
				pos.x = room.transform.position.x + (roomBounds.size.x / 2) - (horizontal / 2);
			}
			else if(transform.position.x - horizontal / 2 < room.transform.position.x - (roomBounds.size.x / 2))
			{
				pos.x = room.transform.position.x - (roomBounds.size.x / 2) + (horizontal / 2);
			}
			#endregion
			#region Vertical
			if(t.position.y - vertical / 2 < room.transform.position.y - (roomBounds.size.y / 2))
			{
				pos.y = room.transform.position.y - (roomBounds.size.y / 2) + (vertical / 2);
			}
			else if(t.position.y + vertical / 2 > room.transform.position.y + (roomBounds.size.y / 2))
			{
				pos.y = room.transform.position.y + (roomBounds.size.y / 2) - (vertical / 2);
			}
			#endregion

			t.position = new Vector3(pos.x, pos.y, t.position.z);
		}
	}

	public void SetRoom(GameObject r)
	{
		room = r;
		roomBounds = r.GetComponent<BoxCollider2D>().bounds;
	}
}