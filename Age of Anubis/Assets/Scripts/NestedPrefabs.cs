using UnityEngine;
using System.Collections;

public enum EnemyType
{
	Scarab,
	Spider,
	Snake,
	Eye,
    Door
}

public class NestedPrefabs : MonoBehaviour 
{
	public EnemyType m_enemyType;
	public int m_level;

	GameObject m_prefab;

	public void ResetPrefab()
	{
		//m_prefab = Resources.Load(".." + m_prefabLocation) as GameObject;

		EnemyManager em = EnemyManager.Inst;

		if (em == null)
			Debug.LogError("Enemy Manager Inst Not Found");

		m_prefab = em.GetPrefab(m_enemyType, m_level);

		if (m_prefab != null)
		{
			GameObject createdObject = Instantiate(m_prefab, transform.position, transform.rotation) as GameObject;

			createdObject.name = gameObject.name;
			createdObject.transform.parent = transform.parent;
			createdObject.transform.position = transform.position;
			createdObject.transform.rotation = transform.rotation;
			createdObject.transform.localScale = transform.localScale;

			DestroyImmediate(gameObject);
		}
		else
		{
			Debug.LogError("Could not find prefab, Please check", gameObject);
		}
	}

}
