/* Copyright (c) Dungeon Crawlers
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class Lava : Enemy
{
	Transform t;
	float startY = 0;
	public float movementAmount = 1;
	public float movementSpeed = 5;
	public bool isActive = false;

	void Start()
	{
		t = transform;
		startY = t.position.y;
	}

	void Update()
	{
		if(isActive)
		{
			t.position = new Vector2(t.position.x, t.position.y + movementSpeed * Time.deltaTime);

			if (t.position.y > (startY + movementAmount))
			{
				t.position = new Vector2(t.position.x, startY + movementAmount);
				isActive = false;
			}
		}
	}
}