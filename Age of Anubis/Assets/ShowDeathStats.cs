/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
using UnityEngine.UI;
 
public class ShowDeathStats : MonoBehaviour
{

	public bool isActive = false;
	public UnityEngine.UI.Image img;

    public int m_increaseSpeed = 3;
    
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

    enum State { Enemies, Rooms, Health, Gold, Weapons}

    State m_curState;

    float m_timer;

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
        m_totalGold =   LastRunStats.inst.startGold;

        m_textTotalEnemies.text = m_totalEnemies.ToString();
        m_textTotalRooms.text = m_totalRooms.ToString();
        m_textTotalHealth.text = m_totalHealth.ToString();
        m_textTotalGold.text = m_totalGold.ToString();

        m_textCurEnemies.text = "";
        m_textCurRooms.text = "";
        m_textCurHealth.text = "";
        m_textCurGold.text = "";

        ChangeState(State.Enemies);
	}

	void Update()
	{


		if(Input.GetButtonDown("Cancel"))
		{
			gameObject.SetActive(false);
		}

        switch (m_curState)
        {
            case State.Enemies: 
                if (m_timer >= 42)
                {
                    ChangeState(State.Rooms);
                }
                else if (m_timer >= 40)
                {
                    //Pause
                }
                else if (m_timer >= 22)
                {
                    m_totalEnemies += m_increaseSpeed;

                    if (m_totalEnemies >= PlayerPrefs.GetInt("TotalEnemies") + m_curEnemies)
                    {
                        m_totalEnemies = PlayerPrefs.GetInt("TotalEnemies") + m_curEnemies;
                        m_timer = 40;
                        m_textTotalEnemies.text = m_totalEnemies.ToString();
                    }
                }
                else if (m_timer >= 20)
                {
                    //Pause
                }
                else
                {
                    m_curEnemies += m_increaseSpeed;

                    if (m_curEnemies >= LastRunStats.inst.enemiesKilled)
                    {
                        m_curEnemies = LastRunStats.inst.enemiesKilled;
                        m_timer = 20;
                        m_textCurEnemies.text = m_curEnemies.ToString();
                    }
                }
                m_timer += Time.deltaTime;
                
                break;
            case State.Rooms: break;
            case State.Health: break;
            case State.Gold: break;
            case State.Weapons: break;
        }
	}

    void ChangeState(State st)
    {
        switch (st)
        {
            case State.Enemies: break;
            case State.Rooms: break;
            case State.Health: break;
            case State.Gold: break;
            case State.Weapons: break;
        }

        m_curState = st;
    }
}