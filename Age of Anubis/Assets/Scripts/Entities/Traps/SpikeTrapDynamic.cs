/* Copyright (c) Dungeon Crawlers
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
using System.Collections.Generic; 

public class SpikeTrapDynamic : Enemy
{
	public List<GameObject> spikes;
	private List<float> spikeStartPosY;
	private float spikeMovedDistance = 0.0f;
	public float spikeMovementDistance = 0.2f;
	public float activateSpeed = 2.0f;
	bool activated = false;
	bool completed = false;
	public float activateDistance = 2.0f;
	public float waitReactTime = 0.3f;
	public BoxCollider2D col;
	private float waitReactCounter;
	private float distanceFromPlayer = Mathf.Infinity;

	Transform t;

	void Start()
	{
		t = transform;
		spikeStartPosY = new List<float>();
		for(int i = 0; i < spikes.Count; i++)
		{
			spikeStartPosY.Add(spikes[i].transform.position.y);
		}
	}

	public override void EnemyBehaviour()
	{
		distanceFromPlayer = Vector2.Distance(t.position, GameManager.inst.player.transform.position);

		if(distanceFromPlayer <= activateDistance)
		{
			waitReactCounter += Time.deltaTime;
			if(waitReactCounter >= waitReactTime)
			{
				//Shoot up
				activated = true;
				
				if(!completed)
				{
					spikeMovedDistance += spikeMovementDistance * activateSpeed * Time.deltaTime;
					for (int i = 0; i < spikes.Count; i++)
					{
						if (spikeMovedDistance <= spikeMovementDistance)
						{
							spikes[i].transform.position = new Vector2(spikes[i].transform.position.x, spikeStartPosY[i] + spikeMovedDistance);
						}
						else
						{
							completed = true;
							col.isTrigger = false;
						}
					}
				}
			}
		}
		else
		{
			waitReactCounter = 0;
			activated = false;
			completed = false;
			spikeMovedDistance = 0;

			col.isTrigger = true;

			for(int i = 0; i < spikes.Count; i++)
			{

				if (spikes[i].transform.position.y <= spikeStartPosY[i])
				{
					spikes[i].transform.position = new Vector2(spikes[i].transform.position.x, spikeStartPosY[i]);
				}
				else
				{
					spikes[i].transform.position = new Vector2(spikes[i].transform.position.x, spikeStartPosY[i] - (spikeMovementDistance * activateSpeed * Time.deltaTime));
				}
			}
		}
	}
}