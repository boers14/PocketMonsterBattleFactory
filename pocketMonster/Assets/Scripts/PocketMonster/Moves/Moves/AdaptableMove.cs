using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptableMove : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = Mathf.Infinity;
        moveType = PocketMonsterStats.Typing.None;
        moveSort = MoveSort.Physical;
        baseDamage = 65;
        hasSideEffect = false;
    }
}

