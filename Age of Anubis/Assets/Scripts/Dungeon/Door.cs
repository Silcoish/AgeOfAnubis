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


    public Sprite m_hideSprite;
    public GameObject m_door;
    public bool m_isLocked = true;
    Animation m_anim;

	public Direction dir;

	void Awake()
	{
        m_sr = m_door.GetComponent<SpriteRenderer>();
        m_anim = gameObject.GetComponentInChildren<Animation>();
		parentRoom = gameObject.transform.parent.parent;
		Lock();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag == "Player")
		{
			col.transform.position = partnerDoor.transform.GetChild(0).position;
			partnerDoor.parentRoom.GetComponent<RoomObject>().EnteredRoom();
			parentRoom.GetComponent<RoomObject>().LeaveRoom();
		}
	}

    public void InitDoor()
    {
       // print("InitDoor");
        //Debug.Log("Room Init", gameObject);
        if (partnerDoor == null)
        {
            m_sr.sprite = m_hideSprite;
            return;
        }

        RoomObject r = parentRoom.GetComponent<RoomObject>();

        if (r != null)
        {
            if (r.m_allEnemies.Count <= 0)
            {
                m_anim.Play();
                m_isLocked = false;
            }
        }

    }

	public void Lock()
	{
        m_isLocked = true;
		//AudioManager.Inst.PlaySFX(AudioManager.Inst.a_doorShut);
	}

	public void Unlock()
	{
		//If partner doesn't exist, don't actually unlock the door. 
		if (partnerDoor == null)
			return;
        if (m_isLocked)
        {
            m_anim.Play();
            m_isLocked = false;
        }
		//AudioManager.Inst.PlaySFX(AudioManager.Inst.a_doorOpen);
	}
}
