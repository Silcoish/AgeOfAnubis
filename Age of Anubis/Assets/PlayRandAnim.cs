using UnityEngine;
using System.Collections;

public class PlayRandAnim : MonoBehaviour 
{
    public AnimationClip[] m_clips;
    Animation m_anim;
 
    void Awake()
    {
        m_anim = gameObject.GetComponent<Animation>();


        int rand = Random.Range(0, m_clips.Length);


        m_anim.clip = m_clips[rand];

        m_anim.Play();

    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
