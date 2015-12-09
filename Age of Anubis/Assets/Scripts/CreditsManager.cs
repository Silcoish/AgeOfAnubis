using UnityEngine;
using System.Collections;
 
public class CreditsManager : MonoBehaviour
{
	public GameObject[] layers;
	public SpriteRenderer background;
	int curLayer = 0;
	public float viewTime = 10.0f;
	public float counter = 0.0f;
	bool fadeIn = true;

	void Start()
	{
		layers[0].SetActive(true);
	}

	void Update()
	{
		counter += Time.deltaTime;
		if(counter >= viewTime)
		{
			if(fadeIn)
			{
				background.color = new Color(background.color.r, background.color.g, background.color.b, Mathf.Lerp(background.color.a, 1.0f, 0.01f));
				if(background.color.a >= 0.98f)
				{
					fadeIn = false;
					background.color = new Color(background.color.r, background.color.g, background.color.b, 1.0f);
					if(curLayer < layers.Length - 1)
					{
						layers[curLayer].SetActive(false);
						curLayer++;
						layers[curLayer].SetActive(true);
					}
					else
					{
						Application.LoadLevel("StartUpScene");
					}
				}
			}
			else
			{
				background.color = new Color(background.color.r, background.color.g, background.color.b, Mathf.Lerp(background.color.a, 0.0f, 0.01f));
				if(background.color.a <= 0.02f)
				{
					fadeIn = true;
					counter = 0.0f;
					background.color = new Color(background.color.r, background.color.g, background.color.b, 0.0f);
				}
			}
		}
	}

	IEnumerator WaitForLayer(float time)
	{
		yield return new WaitForSeconds(time);
	}
	
}