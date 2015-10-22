using UnityEngine;
 
public class CameraController : MonoBehaviour
{
	GameObject room;
	Bounds roomBounds;

	Transform t;

	float vertical;
	float horizontal;

	void Start()
	{
		t = transform;
		vertical = Camera.main.orthographicSize * 2;
		horizontal = vertical * Screen.width / Screen.height;
	}

	void Update()
	{
		if(room != null)
		{
			t.position = new Vector3(GameManager.inst.player.transform.position.x, GameManager.inst.player.transform.position.y, t.position.z);

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