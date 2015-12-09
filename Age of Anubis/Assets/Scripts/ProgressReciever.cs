using UnityEngine;
using System.Collections;

public class ProgressReciever : MonoBehaviour {

	public Shop m_shop;

	public void ProgressIcons()
	{
		m_shop.ProgressIcons();
	}

	public void ReturnToIdle()
	{
		m_shop.ReturnToIdle();
	}
}
