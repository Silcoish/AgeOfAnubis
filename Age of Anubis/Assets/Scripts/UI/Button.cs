using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour 
{
	public Sprite m_spriteIdle;
	public Sprite m_spriteHighlighted;
	public Sprite m_spritePressed;

	public Button m_inputUp;
	public Button m_inputDown;
	public Button m_inputLeft;
	public Button m_inputRight;

	public bool m_defaultSelection = false;
	public bool m_isBackButton = false;
	private bool m_isSelected;

	Vector2 m_inputAxis;

	private SpriteRenderer m_sp;

	private bool m_canMove;

	private float m_timer = 0;

	private float m_scrollTime = 0.5f;

	void Awake ()
	{
		m_sp = gameObject.GetComponent<SpriteRenderer>();
	}

	void OnEnable()
	{
		///if (m_sp != null)
		//SetupDefaultSelection();
	}

	void Start()
	{
		SetupDefaultSelection();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_isBackButton)
		{
			if (Input.GetButton("Fire2"))
			{
				SelectButton();
				m_sp.sprite = m_spritePressed;
			}
			else if (Input.GetButtonUp("Fire2"))
			{
				OnClick();
				m_sp.sprite = m_spriteHighlighted;
			}
		}
		else
		{
			if (Input.GetButton("Fire2"))
			{
				DeSelectButtton();
			}
		}

		if (m_isSelected)
		{
			m_inputAxis.x = Input.GetAxisRaw("Horizontal");
			m_inputAxis.y = Input.GetAxisRaw("Vertical");

			if (Input.GetButton("Jump"))
			{
				m_sp.sprite = m_spritePressed;
			}
			else if (Input.GetButtonUp("Jump"))
			{
				m_sp.sprite = m_spriteHighlighted;
				OnClick();
			}


			if (m_inputAxis.sqrMagnitude > (0.6 * 0.6))
			{
				if (m_timer > 0)
				{
					m_timer -= Time.deltaTime;
				}
				else
				{
					
					//Allow Input
					if (Mathf.Abs(m_inputAxis.x) > Mathf.Abs(m_inputAxis.y))
					{
						//Move X
						if (m_inputAxis.x > 0)
						{
							//Move Right
							if (m_inputRight != null)
							{
								m_inputRight.SelectButton();
								DeSelectButtton();
							}
						}
						else
						{
							//Move Left
							if (m_inputLeft != null)
							{
								m_inputLeft.SelectButton();
								DeSelectButtton();
							}
						}
					}
					else
					{
						//Move Y
						if (m_inputAxis.y > 0)
						{
							//Move Up
							if (m_inputUp != null)
							{
								m_inputUp.SelectButton();
								DeSelectButtton();
							}
						}
						else
						{
							//Move Down
							if (m_inputDown != null)
							{
								m_inputDown.SelectButton();
								DeSelectButtton();
							}
						}
					}
				}
			}
			else
			{
				m_timer = 0;
			}
		}
	}

	void SetupDefaultSelection()
	{
		//Make Sure only one button selected in the parent Object.
		if (m_defaultSelection)
		{
			SelectButton();
			Button[] alllButtons = transform.parent.GetComponentsInChildren<Button>();

			foreach (var b in alllButtons)
			{
				if (b != this)
				{
					b.DeSelectButtton();
					b.m_defaultSelection = false;
				}

			}
		}

		//Make Sure only one back button.
		if (m_isBackButton)
		{
			Button[] alllButtons = transform.parent.GetComponentsInChildren<Button>();

			foreach (var b in alllButtons)
			{
				if (b != this)
				{
					//print(b.gameObject.name);
					b.m_isBackButton = false;
				}

			}
		}
	}

	public void SelectButton()
	{
		m_isSelected = true;
		m_sp.sprite = m_spriteHighlighted;
		m_timer = m_scrollTime;
	}

	public void DeSelectButtton()
	{
		//print(gameObject.name);
		m_isSelected = false;
		m_sp.sprite = m_spriteIdle;
	}

	public virtual void OnClick()
	{

	}

}
