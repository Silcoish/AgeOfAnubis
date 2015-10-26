using UnityEngine;
using System.Collections;

public class Button_SwitchMenus : Button 
{
	public GameObject m_targetMenu;

	public override void OnClick()
	{
		m_targetMenu.SetActive(true);
		transform.parent.gameObject.SetActive(false);
	}
}
