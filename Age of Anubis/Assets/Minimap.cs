using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour {

	void Awake()
	{
		GameManager.inst.visibleMap = gameObject;
	}

}
