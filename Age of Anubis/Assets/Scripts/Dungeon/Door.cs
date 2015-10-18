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

	public Sprite m_closedDoorSprite;
	Sprite m_openedDoorSprite;

    public Transform m_partnerDoor;
    public Transform m_parentRoom;

	BoxCollider2D m_doorCol;

	public Direction m_dir;

	void Start()
	{
		m_openedDoorSprite = GetComponent<SpriteRenderer>().sprite;
		m_doorCol = GetComponent<BoxCollider2D>();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag == "Player")
		{
            col.transform.position = m_partnerDoor.GetChild(0).position;
            //col.GetComponent<Player>().currentRoom = partnerDoor.gameObject.GetComponent<Door>().parentRoom;
			m_partnerDoor.GetComponent<Door>().m_parentRoom.GetComponent<RoomObject>().EnteredRoom();
		}
	}

	public void Lock()
	{
		GetComponent<SpriteRenderer>().sprite = m_closedDoorSprite;
		m_doorCol.isTrigger = false;
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_doorShut);
	}

	public void Unlock()
	{
		GetComponent<SpriteRenderer>().sprite = m_openedDoorSprite;
		m_doorCol.isTrigger = true;
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_doorOpen);
	}
}
