/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class Ambience : MonoBehaviour
{
	public float minTime = 120.0f;
	public float maxTime = 180.0f;

	public float counter = 0.0f;
	public float timer = 0.0f;

	void Start()
	{
		SetTimer();
	}

	void Update()
	{
		counter += Time.deltaTime;
		if(counter >= timer)
		{
			counter = 0.0f;
			SetTimer();
			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ambience);
		}
	}

	void SetTimer()
	{
		timer = Random.Range(minTime, maxTime + 1.0f);
	}
}