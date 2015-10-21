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

	SpriteRenderer m_sr;

    public Door partnerDoor;
    public Transform parentRoom;

	BoxCollider2D doorCol;

	Vector2 m_startPos;
	Vector2 m_endPos;
	Transform m_t;

	public Direction dir;

	void Awake()
	{
		m_t = transform;
		m_startPos = m_t.position;
		m_sr = m_t.GetChild(1).GetComponent<SpriteRenderer>();
		doorCol = GetComponent<BoxCollider2D>();
		parentRoom = gameObject.transform.parent.parent;
		Lock();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag == "Player")
		{
			col.transform.position = partnerDoor.transform.GetChild(0).position;
			partnerDoor.parentRoom.GetComponent<RoomObject>().EnteredRoom();
		}
	}

	public void Lock()
	{
		doorCol.isTrigger = false;
		m_sr.enabled = true;
		//m_t.position = Vector2.Lerp(m_t.position, m_startPos, 0.1f);
		//AudioManager.Inst.PlaySFX(AudioManager.Inst.a_doorShut);
	}

	public void Unlock()
	{
		//If partner doesn't exist, don't actually unlock the door. 
		if (partnerDoor == null)
			return;
		doorCol.isTrigger = true;
		m_sr.enabled = false;
		//AudioManager.Inst.PlaySFX(AudioManager.Inst.a_doorOpen);
	}
}
