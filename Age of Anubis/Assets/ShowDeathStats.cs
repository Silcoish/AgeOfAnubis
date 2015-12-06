/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
public class ShowDeathStats : MonoBehaviour
{
	bool m_isFinished;

	Animation m_anim;

	//public GameObject m_butonstupidhack;

	public bool isActive = false;
	public UnityEngine.UI.Image img;

	public EventSystem m_es;

	public float m_increaseTime = 2;
	public float m_pauseTime = 1;
	
	public Text m_textTotalEnemies;
	public Text m_textTotalRooms;
	public Text m_textTotalHealth;
	public Text m_textTotalGold;

	public Text m_textCurEnemies;
	public Text m_textCurRooms;
	public Text m_textCurHealth;
	public Text m_textCurGold;

	int m_totalEnemies;
	int m_totalRooms;
	int m_totalHealth;
	int m_totalGold;
  
	int m_curEnemies;
	int m_curRooms;
	int m_curHealth;
	int m_curGold;

	int m_tempGoldHolder = 0;

	enum Row { Enemies, Rooms, Health, Gold, Weapons}
	enum Column { Dungeon, Bank}

	Row m_curRow;
	Column m_curColumn;

	float m_timer = 0;
	float m_lerpNumber = 0;
	float m_pauseTimer = 0;
	void Awake()
	{
		//m_es = gameObject.GetComponentInChildren<EventSystem>();
		m_anim = gameObject.GetComponent<Animation>();
	}


	void Start()
	{
		if(LastRunStats.inst != null)
		{
			Init();
		}
		else
		{
			gameObject.SetActive(false);
		}
		Pause(5);
	}

	void OnEnable()
	{
		GameManager.inst.player.GetComponent<Player>().m_isShopOpen = true;
		//m_es.SetSelectedGameObject(m_es.firstSelectedGameObject);
	}

	void Init()
	{
		//isActive = true;
		//img.gameObject.SetActive(true);
		//title.text =  LastRunStats.inst.died ? "You passed out!" : "You escaped!";
		//startGold.text = LastRunStats.inst.startGold.ToString();
		//endGold.text = LastRunStats.inst.endGold.ToString();
		//enemiesKilled.text = LastRunStats.inst.enemiesKilled.ToString();
		//hpPickups.text = LastRunStats.inst.hpPickups.ToString();
		//roomsCleared.text = LastRunStats.inst.roomsCleared.ToString();

		m_totalEnemies = PlayerPrefs.GetInt("TotalEnemies");
		m_totalRooms = PlayerPrefs.GetInt("TotalRooms");
		m_totalHealth = PlayerPrefs.GetInt("TotalHealth");
		m_totalGold = PlayerPrefs.GetInt("BankGold");

		m_textTotalEnemies.text = m_totalEnemies.ToString();
		m_textTotalRooms.text = m_totalRooms.ToString();
		m_textTotalHealth.text = m_totalHealth.ToString();
		m_textTotalGold.text = m_totalGold.ToString();

		m_textCurEnemies.text = "";
		m_textCurRooms.text = "";
		m_textCurHealth.text = "";
		m_textCurGold.text = "";

		m_isFinished = false;

		//ChangeState(Row.Enemies);
	}

	void Update()
	{
		GameManager.inst.player.GetComponent<Player>().m_isShopOpen = true;
		//m_es.SetSelectedGameObject(m_butonstupidhack);
		if (m_pauseTimer > 0)
		{
			m_pauseTimer -= Time.deltaTime;
		}
		else
		{
			if (LastRunStats.inst != null)
			{
				switch (m_curColumn)
				{
					case Column.Dungeon:
						switch (m_curRow)
						{
							case Row.Enemies:
								if (LerpValue(m_textCurEnemies, m_curEnemies, LastRunStats.inst.enemiesKilled, m_increaseTime))
								{
									m_curEnemies = LastRunStats.inst.enemiesKilled;
									m_curRow = Row.Rooms;
									m_lerpNumber = 0;
									Pause(m_pauseTime);
								}
								break;
							case Row.Rooms:
								if (LerpValue(m_textCurRooms, m_curRooms, LastRunStats.inst.roomsCleared, m_increaseTime))
								{
									m_curRooms = LastRunStats.inst.roomsCleared;
									m_curRow = Row.Health;
									m_lerpNumber = 0;
									Pause(m_pauseTime);
								}
								break;
							case Row.Health:
								if (LerpValue(m_textCurHealth, m_curHealth, LastRunStats.inst.hpPickups, m_increaseTime))
								{
									m_curHealth = LastRunStats.inst.hpPickups;
									m_curRow = Row.Gold;
									m_lerpNumber = 0;
									Pause(m_pauseTime);
									m_tempGoldHolder = LastRunStats.inst.endGold;
								}
								break;
							case Row.Gold:
								if (LerpValue(m_textCurGold, m_curGold, m_tempGoldHolder, m_increaseTime))
								{
									m_curGold = m_tempGoldHolder;
									m_curRow = Row.Weapons;
									m_lerpNumber = 0;
									Pause(m_pauseTime);
									if (LastRunStats.inst.died)
										m_anim.Play();
								}
								break;
							case Row.Weapons:
								if (LastRunStats.inst.died)
								{
									//Do Final Death stuff
									if (LerpValue(m_textCurGold, m_curGold, 0, m_increaseTime))
									{
										m_curGold = m_tempGoldHolder;
										m_curRow = Row.Enemies;
										m_curColumn = Column.Bank;
										Pause(m_pauseTime);
									}
								}
								else
								{
									m_curRow = Row.Enemies;
									m_curColumn = Column.Bank;
									Pause(m_pauseTime);
								}

								break;
						}
						break;
					case Column.Bank:
						switch (m_curRow)
						{
							case Row.Enemies:
								if (LerpValue(m_textTotalEnemies, m_totalEnemies, m_totalEnemies + m_curEnemies, m_increaseTime))
								{
									m_totalEnemies = m_totalEnemies + m_curEnemies;
									m_curRow = Row.Rooms;
									m_lerpNumber = 0;
									Pause(m_pauseTime);
								}
								break;
							case Row.Rooms:
								if (LerpValue(m_textTotalRooms, m_totalRooms, m_totalRooms + m_curRooms, m_increaseTime))
								{
									m_totalRooms = m_totalRooms + m_curRooms;
									m_curRow = Row.Health;
									m_lerpNumber = 0;
									Pause(m_pauseTime);
								}
								break;
							case Row.Health:
								if (LerpValue(m_textTotalHealth, m_totalHealth, m_totalHealth + m_curHealth, m_increaseTime))
								{
									m_totalHealth = m_totalHealth + m_curHealth;
									m_curRow = Row.Gold;
									m_lerpNumber = 0;
									Pause(m_pauseTime);
								}
								break;
							case Row.Gold:
								if (LastRunStats.inst.died == false)
								{
									if (LerpValue(m_textTotalGold, m_totalGold, m_totalGold + m_curGold, m_increaseTime))
									{
										m_totalGold = m_totalGold + m_curGold;
										m_curRow = Row.Weapons;
										m_lerpNumber = 0;
										Pause(m_pauseTime);
									}
								}
								else
								{
									m_curRow = Row.Weapons;
									m_lerpNumber = 0;
									Pause(m_pauseTime);
								}
								break;
							case Row.Weapons:

								m_isFinished = true;
								break;
						}
						break;
				}
			}
		}

		if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Submit"))
		{
			if (AudioManager.Inst != null)
				AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_confirm);

			Finished();
		}
	}

	public void Finished()
	{
		if (m_isFinished || LastRunStats.inst == null)
		{

				PlayerPrefs.SetInt("TotalEnemies", m_totalEnemies);
				PlayerPrefs.SetInt("TotalRooms", m_totalRooms);
				PlayerPrefs.SetInt("TotalHealth", m_totalHealth);
				PlayerPrefs.SetInt("BankGold", m_totalGold);

			UIManager.Inst.UpdateCoinTotal(0);

				PlayerInventory.Inst.m_currentGold = 0;
				if (LastRunStats.inst != null)
				{
					LastRunStats.inst.m_isEnded = false;
					LastRunStats.inst.m_isEnded = false;
				}

				gameObject.SetActive(false);

				GameManager.inst.m_saveManager.Save();
				GameManager.inst.player.GetComponent<Player>().m_isShopOpen = false;
			
		}
		else
		{
				m_increaseTime *= 0.5f;
				m_pauseTime *= 0.5f;

		}
	}

	bool LerpValue(Text tx, int original, int next, float transTime)
	{
		m_lerpNumber += Time.deltaTime / transTime;
		int num = (int)Mathf.Lerp(original, next, m_lerpNumber);

		if (num == next)
		{
			m_lerpNumber = 0;
			tx.text = next.ToString();
			return true;
		}
		else
		{
			tx.text = num.ToString();
			return false;
		}
	}

	void Pause(float seconds)
	{
		m_pauseTimer = seconds;
	}


	//void ChangeState(State st)
	//{
	//    switch (st)
	//    {
	//        case State.Enemies: break;
	//        case State.Rooms: break;
	//        case State.Health: break;
	//        case State.Gold: break;
	//        case State.Weapons: break;
	//    }

	//    m_curRow = st;
	//}
}