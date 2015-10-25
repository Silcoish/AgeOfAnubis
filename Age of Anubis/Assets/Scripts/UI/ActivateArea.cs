using UnityEngine;
using System.Collections;

public class ActivateArea : MonoBehaviour 
{
	public Vector2 m_iconOffset = Vector2.zero;

	private Vector2 m_iconOffsetPos = new Vector2(0, 0.03f);
	//private Vector2 m_iconOffsetScale = new Vector2(1.2f, 1.2f);

	private float m_iconFlashTime = 0.1f;
	private float m_timer = 0;
	private float m_lerp;

	private bool m_isActive;

	private Transform m_activateBackground;
	private Transform m_activateIcon;

	protected Player m_player;

	void Awake()
	{
		m_activateBackground = transform.Find("Background");
		m_activateIcon = transform.Find("Icon");

	}
	// Use this for initialization
	void Start () 
	{
		//TODO use static player if possible.
		m_player = FindObjectOfType<Player>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_isActive)
		{
			m_timer += Time.deltaTime / m_iconFlashTime;
			m_lerp = (Mathf.Sin(m_timer) + 1) / 2;

			m_activateIcon.transform.localPosition = Vector2.Lerp(m_iconOffset, m_iconOffset + m_iconOffsetPos, m_lerp);
			//m_activateIcon.transform.localScale = Vector2.Lerp(Vector2.one, m_iconOffsetScale, m_lerp);

			Vector2 vert;
			vert.y = Input.GetAxis("Vertical");
			vert.x = Input.GetAxis("Horizontal");

			if (vert.y > 0.7 && Mathf.Abs(vert.y) > Mathf.Abs(vert.x))
			{
				OnActivate();
				m_isActive = false;
			}

		}	
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			Active();
		}

	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			InActive();
		}

	}



	void Active()
	{
		m_activateBackground.gameObject.SetActive(true);
		m_activateIcon.gameObject.SetActive(true);
		m_isActive = true;
	}

	void InActive()
	{
		m_activateBackground.gameObject.SetActive(false);
		m_activateIcon.gameObject.SetActive(false);
		m_isActive = false;
	}


	public virtual void OnActivate()
	{
		//TODO play sound on activate.
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_pickupWeapon);

	}
}
