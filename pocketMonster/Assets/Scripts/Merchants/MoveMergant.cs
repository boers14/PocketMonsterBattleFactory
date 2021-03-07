using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMergant : Mergant
{
    private PocketMonsterMoves move;

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (wantsToGive)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < meetDistance)
            {
                gameManager.CreatePocketMonsterMenuForMoveToGive(move, player);
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                player.GetComponent<PlayerMovement>().enabled = false;
                wantsToGive = false;
            }
        }
    }

    public void SetMove(PocketMonsterMoves move)
    {
        this.move = move;
    }
}
