/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class Event_AnubisTrigger : MonoBehaviour
{
	public GameObject anubis;

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag == "Player")
		{
			anubis.SetActive(true);
			Destroy(this);
		}

        Camera.main.gameObject.GetComponent<CameraController>().m_isBossCam = true;
        Camera.main.gameObject.GetComponent<CameraController>().m_bossTrans = anubis.transform;
	}
}