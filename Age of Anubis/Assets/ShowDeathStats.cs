/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
using UnityEngine.UI;
 
public class ShowDeathStats : MonoBehaviour
{

	public UnityEngine.UI.Image img;

	void Start()
	{
		if(LastRunStats.inst != null)
		{
			Init();
		}
		else
		{
			Destroy(this);
		}

	}

	void Init()
	{

	}

	void Update()
	{
		img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.Lerp(img.color.a, 1.0f, 0.02f));
	}
}