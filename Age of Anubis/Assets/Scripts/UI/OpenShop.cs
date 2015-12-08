using UnityEngine;
using System.Collections;

public class OpenShop : ActivateArea 
{
    public GameObject m_shop;
	public override void OnActivate()
	{
		base.OnActivate();

		Shop sh = m_shop.gameObject.GetComponent<Shop>();

		if (sh != null)
			sh.OnOpenSHop();
        gameObject.SetActive(false);
	}


}
