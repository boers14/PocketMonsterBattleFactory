using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMergant : Mergant
{
    private PocketMonsterItem item;

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (wantsToGive)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < meetDistance)
            {
                gameManager.CreatePocketMonsterMenuForItemToGive(item, player);
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                player.GetComponent<PlayerMovement>().enabled = false;
                wantsToGive = false;
            }
        }
    }

    public void SetItem(PocketMonsterItem item)
    {
        this.item = item;
    }
}
