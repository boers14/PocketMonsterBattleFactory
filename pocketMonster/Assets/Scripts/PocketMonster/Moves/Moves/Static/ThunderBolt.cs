using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBolt : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 80;
        powerPoints = 5;
        moveName = "Thunder bolt";
        moveType = PocketMonsterStats.Typing.Static;
        moveSort = MoveSort.Special;
        baseDamage = 90;
        hasSideEffect = false;
        moveDescription = "Has no side effect. 5 power points.";
    }
}
