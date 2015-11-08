using UnityEngine;
using System.Collections;

public class OpenShop : ActivateArea 
{
    public GameObject m_shop;
	public override void OnActivate()
	{
		base.OnActivate();

        m_shop.SetActive(true);
        m_player.GetComponent<Player>().m_isShopOpen = true;
        gameObject.SetActive(false);
	}


}
