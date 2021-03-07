using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyStrike : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 80;
        powerPoints = 5;
        moveName = "Sky Strike";
        moveType = PocketMonsterStats.Typing.Wind;
        moveSort = MoveSort.Physical;
        baseDamage = 90;
        hasSideEffect = false;
        moveDescription = "Has no side effect. 5 power points.";
    }
}
