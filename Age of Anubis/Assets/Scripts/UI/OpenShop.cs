using UnityEngine;
using System.Collections;

public class OpenShop : ActivateArea 
{
    public GameObject m_shop;
	public override void OnActivate()
	{
		base.OnActivate();

		Shop sh = m_shop.gameObject.GetComponent<Shop>();

		if (GameManager.inst != null)
		{
			if (GameManager.inst.isPaused == false)
			{
				if (sh != null)
					sh.OnOpenSHop();
				gameObject.SetActive(false);
			}
		}
		else
		{
			if (sh != null)
				sh.OnOpenSHop();
			gameObject.SetActive(false);

		}
	}


}
