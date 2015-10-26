using UnityEngine;
using System.Collections;

public class Button_Exit : Button
{

	public override void OnClick()
	{
		Application.Quit();
	}
}
