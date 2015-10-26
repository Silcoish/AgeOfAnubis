using UnityEngine;
using System.Collections;

public class Button_NewGame : Button
{

	public override void OnClick()
	{
		LoadingManager.Inst.LoadLevel("ShopScene", true);
	}
}
