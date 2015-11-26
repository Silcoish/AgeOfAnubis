/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
using UnityEngine.UI;
 
public class ShowDeathStats : MonoBehaviour
{

	public bool isActive = false;
	public UnityEngine.UI.Image img;
	public Text title;
	public Text startGold;
	public Text endGold;
	public Text enemiesKilled;
	public Text hpPickups;
	public Text roomsCleared;

	void Start()
	{
		if(LastRunStats.inst != null)
		{
			Init();
		}

	}

	void Init()
	{
		isActive = true;
		img.gameObject.SetActive(true);
		title.text =  LastRunStats.inst.died ? "You passed out!" : "You escaped!";
		startGold.text = LastRunStats.inst.startGold.ToString();
		endGold.text = LastRunStats.inst.endGold.ToString();
		enemiesKilled.text = LastRunStats.inst.enemiesKilled.ToString();
		hpPickups.text = LastRunStats.inst.hpPickups.ToString();
		roomsCleared.text = LastRunStats.inst.roomsCleared.ToString();
	}

	void Update()
	{
		if(isActive)
			img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.Lerp(img.color.a, 1.0f, 0.02f));
		else
			img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.Lerp(img.color.a, 0.0f, 0.1f));

		if(Input.GetButtonDown("Cancel"))
		{
			gameObject.SetActive(false);
		}
	}
}