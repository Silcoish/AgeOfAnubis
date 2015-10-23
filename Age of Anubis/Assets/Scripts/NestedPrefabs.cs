using UnityEngine;
using System.Collections;

public class NestedPrefabs : MonoBehaviour 
{
	public GameObject m_prefab;

	public void ResetPrefab()
	{
		GameObject createdObject = Instantiate(m_prefab, transform.position, transform.rotation) as GameObject;

		createdObject.name = gameObject.name;
		createdObject.transform.parent = transform.parent;
		createdObject.transform.position = transform.position;
		createdObject.transform.rotation = transform.rotation;
		createdObject.transform.localScale = transform.localScale;

		DestroyImmediate(gameObject);
	}

}
