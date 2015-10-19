using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	public enum Direction
	{
		SOUTH,
		EAST,
        NORTH,
		WEST
	}

	public Sprite closedDoorSprite;
	Sprite openedDoorSprite;

    public Door partnerDoor;
    public Transform parentRoom;

	BoxCollider2D doorCol;

	public Direction dir;

	void Start()
	{
		//openedDoorSprite = GetComponent<SpriteRenderer>().sprite;
		doorCol = GetComponent<BoxCollider2D>();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag == "Player")
		{
			col.transform.position = partnerDoor.transform.GetChild(0).position;
            //col.GetComponent<Player>().currentRoom = partnerDoor.gameObject.GetComponent<Door>().parentRoom;
			//partnerDoor.GetComponent<Door>().parentRoom.GetComponent<RoomObject>().EnteredRoom();
		}
	}

	public void Lock()
	{
		//GetComponent<SpriteRenderer>().sprite = closedDoorSprite;
		doorCol.isTrigger = false;
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_doorShut);
	}

	public void Unlock()
	{
		//GetComponent<SpriteRenderer>().sprite = openedDoorSprite;
		doorCol.isTrigger = true;
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_doorOpen);
	}
}
