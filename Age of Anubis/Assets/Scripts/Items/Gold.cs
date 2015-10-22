using UnityEngine;
using System.Collections;

public class Gold : MonoBehaviour 
{
    public int coinValue = 10;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            PlayerInventory.Inst.m_gold += coinValue;
            //AudioManager.Inst.PlaySFX(AudioManager.Inst.a_coin);
            Destroy(gameObject);
        }
    }
}
