using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTouch : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 15;
        moveName = "Fire touch";
        moveType = PocketMonsterStats.Typing.Fire;
        moveSort = MoveSort.Physical;
        baseDamage = 65;
        hasSideEffect = false;
        moveDescription = "Has no side effect. 15 power points.";
    }
}
