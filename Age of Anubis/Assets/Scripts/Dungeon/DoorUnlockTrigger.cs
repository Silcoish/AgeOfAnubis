using UnityEngine;
using System.Collections;

public class DoorUnlockTrigger : MonoBehaviour 
{
    public Door m_targetDoor;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (m_targetDoor == null)
                Debug.Log("DoorUnlockTrigger does not have a target door");
            else
            {
                if(m_targetDoor.m_isLocked)
				{
                    m_targetDoor.Unlock();
					AudioManager.Inst.PlaySFX(AudioManager.Inst.a_doorOpen);
				}
            }
        }
    }
}
