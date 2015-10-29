using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour 
{
	[Header("DamageableStats")]
	public string m_unitName;
	public int m_hitPoints = 10;
	protected int m_maxHitpoints;

	protected float m_effectFlashRate = 0.2f;

	protected float m_globalMoveSpeed = 1;

	protected float m_poisonTime = 1f;//Time inbetween poison hits.
	protected float m_timerPoisonHits = 0;

	protected float m_timerPoison = 0;
	protected float m_timerBurn = 0;
	protected float m_timerFreeze = 0;
	protected float m_timerMud = 0;
	protected float m_timerBleed = 0;
	protected float m_timerPause = 0;
	protected float m_damageTimer = 0;


	protected float m_strengthPoison = 0;
	protected float m_strengthBurn = 0;
	protected float m_strengthBleed = 1;//damage multiplyer
	protected float m_strengthMud = 1;//SpeedMultiplyer
	protected float m_strengthFreeze = 1;//does not effect entity.

	protected float m_leftoverBurnDamage = 0;
	protected float m_leftoverPoisonDamage = 0;



	protected SpriteRenderer m_sp;
	protected Color m_startColor;

	protected Rigidbody2D m_rb;

	
	

	void Awake () 
	{
		m_rb = gameObject.GetComponent<Rigidbody2D>();
		m_sp = gameObject.GetComponent<SpriteRenderer>();

		if (m_rb == null)
			m_rb = gameObject.GetComponentInChildren<Rigidbody2D>();
		if (m_sp == null)
			m_sp = gameObject.GetComponentInChildren<SpriteRenderer>();

		m_startColor = m_sp.color;

		m_maxHitpoints = m_hitPoints;

		AwakeOverride();
	}

	void Start()
	{
		StartOverride();
	}
	
	void Update () 
	{
		m_sp.color = Color.white;

		//Colour Change for taking damage.
		if (m_damageTimer > 0)
		{
			m_damageTimer -= Time.deltaTime;
			m_sp.color = Color.Lerp(m_startColor, Color.red, m_damageTimer);
		}

		//POISON
		if (m_timerPoison > 0)
		{
			m_timerPoison -= Time.deltaTime;
			m_sp.color = Color.Lerp(m_sp.color, Color.green, SinLerp(m_timerPoison));

			m_timerPoisonHits += Time.deltaTime;

			if (m_timerPoisonHits > m_poisonTime)
			{
				DamagePoison();
				m_timerPoisonHits = 0;
			}
		}

		//BURN
		if (m_timerBurn > 0)
		{
			m_timerBurn -= Time.deltaTime;
			m_sp.color = Color.Lerp(m_sp.color, Color.red, SinLerp(m_timerPoison));

			DamageBurn();

		}

		//FREEZE
		if (m_timerFreeze > 0)
		{
			m_timerFreeze -= Time.deltaTime;
			m_sp.color = Color.Lerp(m_sp.color, Color.blue, SinLerp(m_timerPoison));

			m_globalMoveSpeed = 0;

			if (m_timerFreeze <= 0)
			{
				m_globalMoveSpeed = 1;
			}
		}

		//MUD
		if (m_timerMud > 0)
		{
			m_globalMoveSpeed = 0;

			if (m_timerFreeze <= 0)
			{
				m_globalMoveSpeed = 1;
			}
		}

        if (m_hitPoints <= 0)
        {
            OnDeath();
        }


		// Run the Update Override
		if (m_timerPause > 0)
		{
			m_timerPause -= Time.deltaTime;
		}
		else
		{
			UpdateOverride();
		}
	}

	public virtual void AwakeOverride()
	{

	}

	public virtual void StartOverride()
	{

	}

	public virtual void UpdateOverride()
	{

	}


	float SinLerp(float tmr)
	{
		return (1 + Mathf.Sin(tmr / m_effectFlashRate)) / 2;
	}

	/// <summary>
	/// Called When the Enemy Takes any damage/hit
	/// Damge could be = to 0
	/// </summary>
	/// <param name="dam"></param>
	public virtual void OnTakeDamage(Damage dam)
	{
        if(m_timerPause <= 0)
        {
            dam = UpdateDamage(dam);
            m_hitPoints -= dam.amount;

            //AudioManager.Inst.PlaySFX(AudioManager.Inst.a_takeDamage);

            // Old Knockback calculations
            //Vector2 kbForce = (transform.position - dam.fromGO.position).normalized * dam.knockback;
            //kbForce = new Vector2(kbForce.x, 0); // Remove Y axis calculations from knockback. NOTE: Consider halving y force to create a smaller "bounce" effect
            //m_rb.AddForce(kbForce, ForceMode2D.Impulse);

            // New Knockback calculations
            float kbDirCheck = transform.position.x - dam.fromGO.position.x;
            Vector2 kbDir = new Vector2(1F, 0.5F); // Default knockback to right.
            if (kbDirCheck < 0) // If my X minus source X is negative, then they are to my right.
                kbDir = new Vector2(-1F, 0.5F); // Damage source is to my right, so knockback to left.
            m_rb.velocity = kbDir * dam.knockback; // set velocity to direction times knockback strength.
            Pause(0.3F); // Pause input briefly.

            switch (dam.type)
            {
                case DamageType.NONE:
                    break;
                case DamageType.POISON:
                    AudioManager.Inst.PlaySFX(AudioManager.Inst.a_poison);
                    m_timerPoison = dam.effectTime;
                    m_strengthPoison = dam.effectStrength;
                    break;
                case DamageType.BURN:
                    AudioManager.Inst.PlaySFX(AudioManager.Inst.a_burnt);
                    m_timerBurn = dam.effectTime;
                    m_strengthBurn = dam.effectStrength;
                    break;
                case DamageType.FREEZE:
                    AudioManager.Inst.PlaySFX(AudioManager.Inst.a_frozen);
                    m_timerFreeze = dam.effectTime;
                    m_strengthFreeze = dam.effectStrength;
                    break;
                case DamageType.BLEED:
                    AudioManager.Inst.PlaySFX(AudioManager.Inst.a_bleed);
                    m_timerBleed = dam.effectTime;
                    m_strengthBleed = dam.effectStrength;
                    break;
                default:
                    AudioManager.Inst.PlaySFX(AudioManager.Inst.a_stab);
                    break;
            }

            if (m_timerBleed > 0)
            {
                DamageBleed(dam.amount);
            }

            m_damageTimer = 1;

            if (m_hitPoints <= 0)
            {
                OnDeath();
            }
        }
	}

	//Use this to modify the damage if you need
	//Main use to set kb to 0 on anubis
	public virtual Damage UpdateDamage(Damage dam)
	{
		return dam;
	}

	public virtual void OnDeath()
	{
		Destroy(gameObject);
	}

	public string GetName()
	{
		return m_unitName;
	}

	public void Pause(float seconds)
	{
		m_timerPause = seconds;
	}

    public virtual void RecoverHealth(int amount)
    {
        m_hitPoints += amount;
        if (m_hitPoints > m_maxHitpoints)
            m_hitPoints = m_maxHitpoints;
    }

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// ######## ##       ######## ##     ## ######## ##    ## ########    ###    ##          ########     ###    ##     ##    ###     ######   ########	//
	// ##       ##       ##       ###   ### ##       ###   ##    ##      ## ##   ##          ##     ##   ## ##   ###   ###   ## ##   ##    ##  ##		//
	// ##       ##       ##       #### #### ##       ####  ##    ##     ##   ##  ##          ##     ##  ##   ##  #### ####  ##   ##  ##        ##		//
	// ######   ##       ######   ## ### ## ######   ## ## ##    ##    ##     ## ##          ##     ## ##     ## ## ### ## ##     ## ##   #### ######	//
	// ##       ##       ##       ##     ## ##       ##  ####    ##    ######### ##          ##     ## ######### ##     ## ######### ##    ##  ##		//
	// ##       ##       ##       ##     ## ##       ##   ###    ##    ##     ## ##          ##     ## ##     ## ##     ## ##     ## ##    ##  ##		//
	// ######## ######## ######## ##     ## ######## ##    ##    ##    ##     ## ########    ########  ##     ## ##     ## ##     ##  ######   ########	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	void DamagePoison()
	{
        m_leftoverPoisonDamage += m_strengthPoison * m_poisonTime;
		m_hitPoints -= (int)m_leftoverPoisonDamage;      
        if ((int)m_leftoverPoisonDamage > 0)
			SpawnText(Color.green, ((int)m_leftoverPoisonDamage).ToString());
		m_leftoverPoisonDamage -= (int)m_leftoverPoisonDamage;

		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_poison);
	}

	void DamageBurn()
	{
		m_leftoverBurnDamage += m_strengthBurn * Time.deltaTime;
		m_hitPoints -= (int)m_leftoverBurnDamage;
		if ((int)m_leftoverBurnDamage > 0)
			SpawnText(Color.red, ((int)m_leftoverBurnDamage).ToString());
		m_leftoverBurnDamage -= (int)m_leftoverBurnDamage;

		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_burnt);
	}

	void DamageBleed(int damIn)
	{
		m_hitPoints -= (int)(damIn * m_strengthBleed);

		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_bleed);
	}

	void SpawnText(Color col, string amt)
	{
		//GameObject temp = Instantiate(GameDrops.Inst.textObject, transform.position, GameDrops.Inst.textObject.transform.rotation) as GameObject;

		//TextObject tempText = temp.GetComponent<TextObject>();

		//tempText.SetParams(col, amt);
	}

}
