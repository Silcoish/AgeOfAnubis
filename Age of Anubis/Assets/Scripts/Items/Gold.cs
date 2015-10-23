using UnityEngine;
using System.Collections;

public class Gold : MonoBehaviour 
{
    public float coinValue = 10;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            float value = coinValue * PlayerInventory.Inst.m_multiplier;
            PlayerInventory.Inst.m_gold += (int)value;
            PlayerInventory.Inst.m_multiplier += 0.01F;
            Debug.Log("Multiplier = " + PlayerInventory.Inst.m_multiplier + " Gold = " + PlayerInventory.Inst.m_gold);
            //AudioManager.Inst.PlaySFX(AudioManager.Inst.a_coin);
            Destroy(gameObject);
        }
    }
}
