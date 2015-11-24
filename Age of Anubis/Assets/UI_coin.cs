using UnityEngine;
using System.Collections;

public class UI_coin : MonoBehaviour 
{
    public Vector3 m_endPos;
    Vector3 m_startPos;

    public float m_moveTime;


    RectTransform m_rt;


    float m_lerp = 0;

    void Awake()
    {
        m_rt = gameObject.GetComponent<RectTransform>();
        m_startPos = m_rt.position;
    }



	void Update () 
    {
        m_lerp += Time.deltaTime / m_moveTime;

        Vector3 tt = m_endPos;

        tt.y = Screen.height - tt.y;

        m_rt.position = Vector3.Lerp(m_startPos, tt, m_lerp);


	}
}
