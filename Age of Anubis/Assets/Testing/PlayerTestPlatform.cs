using UnityEngine;
using System.Collections;

public class PlayerTestPlatform : MonoBehaviour 
{
	public float m_jumpVel = 5;

	private Rigidbody2D m_rb;
	private CircleCollider2D m_col;

	//private Collider2D m_lastCol;

	// Use this for initialization
	void Start () 
	{
		m_rb = gameObject.GetComponent<Rigidbody2D>();
		m_col = gameObject.GetComponent<CircleCollider2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Check2WayPlatforms();

		Vector2 vel = m_rb.velocity;
		if (Input.GetKeyDown(KeyCode.Space))
		{
			vel.y = m_jumpVel;
		}


		if (Input.GetKeyUp(KeyCode.Space))
		{
			if (m_rb.velocity.y > 0)
			{
				vel.y = 0;
			}

		}

		vel.x = Input.GetAxis("Horizontal") * 5;

		m_rb.velocity = vel;

		if (Input.GetKey(KeyCode.DownArrow))
		{
			RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.down, 2);

			foreach (var h in hit)
			{
				if (h.collider.gameObject.tag == "NotSolid")
				{
					Physics2D.IgnoreCollision(h.collider, m_col,true);
					//m_lastCol = h.collider;
				}
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{

	}

	void Check2WayPlatforms()
	{
		Vector2 offsetDistance = new Vector2(2,2);

		Collider2D[] allCols = Physics2D.OverlapAreaAll((Vector2)m_col.bounds.min - offsetDistance, (Vector2)m_col.bounds.max + offsetDistance);

		foreach (var col in allCols)
		{
			if (col.CompareTag("NotSolid"))
			{
				if (m_col.bounds.min.y < col.bounds.max.y)
				{
					Physics2D.IgnoreCollision(m_col, col, true);
				}
				else
				{
					Physics2D.IgnoreCollision(m_col, col, false);
				}

			}
		}
	}




	

}
