using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketMonsterMergant : Mergant
{ 
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (wantsToGive)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < meetDistance)
            {
                gameManager.CreatePickAPocketMonsterMenu(player);
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                player.GetComponent<PlayerMovement>().enabled = false;
                wantsToGive = false;
            }
        }
    }
}
