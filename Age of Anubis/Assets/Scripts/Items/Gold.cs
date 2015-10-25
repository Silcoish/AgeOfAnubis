using UnityEngine;
using System.Collections;

public class Gold : MonoBehaviour 
{
    public float coinValue = 10;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
			PlayerInventory.Inst.ChangeGold((int)coinValue);
            PlayerInventory.Inst.ChangeMultiplier(0.01f);
            AudioManager.Inst.PlaySFX(AudioManager.Inst.a_coin);
            Destroy(gameObject);
        }
    }
}
