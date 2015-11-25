using UnityEngine;
using System.Collections;

public class JumpCloud : MonoBehaviour 
{
    private Animator m_anim;

    void Start()
    {
        m_anim = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        if(!m_anim.GetCurrentAnimatorStateInfo(0).IsName("JumpCloud"))
        {
            Destroy(gameObject);
        }
    }
}
