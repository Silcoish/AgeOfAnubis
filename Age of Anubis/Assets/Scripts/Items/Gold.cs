using UnityEngine;
using System.Collections;

public class Gold : MonoBehaviour 
{
    public float m_coinValue = 10;
    public float m_animationFrequency = 5F;
    private float m_curTimer = 0;
    private Animator m_anim;

    void Awake()
    {
        m_anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (m_curTimer <= 0)
        {
            m_anim.SetTrigger("Animate");
            m_curTimer = m_animationFrequency;
        }
        else
            m_curTimer -= Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            PlayerInventory.Inst.ChangeGold((int)m_coinValue);
            //PlayerInventory.Inst.ChangeMultiplier(0.01f);
            AudioManager.Inst.PlaySFX(AudioManager.Inst.a_coin);
            Destroy(gameObject);
        }
    }
}
