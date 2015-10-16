using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    public float m_moveSpeed;
    public float m_jumpHeight;
    public int m_jumpNumber;

    private Rigidbody2D m_rb2D;
    private bool m_isJumping;
    private int m_jumpCounter;

	void Start () 
    {
        m_rb2D = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate () 
    {
        float move = Input.GetAxisRaw("Horizontal");
        m_rb2D.velocity = new Vector2(move * m_moveSpeed, m_rb2D.velocity.y);
    
    }
}
