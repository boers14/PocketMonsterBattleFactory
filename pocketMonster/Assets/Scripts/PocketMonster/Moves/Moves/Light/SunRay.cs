using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRay : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 80;
        powerPoints = 5;
        moveName = "Sun ray";
        moveType = PocketMonsterStats.Typing.Light;
        moveSort = MoveSort.Special;
        baseDamage = 90;
        hasSideEffect = false;
        moveDescription = "Has no side effect. 5 power points.";
    }
}
