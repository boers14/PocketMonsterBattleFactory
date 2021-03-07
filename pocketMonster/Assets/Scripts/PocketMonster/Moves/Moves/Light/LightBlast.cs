using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlast : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 15;
        moveName = "Light blast";
        moveType = PocketMonsterStats.Typing.Light;
        moveSort = MoveSort.Special;
        baseDamage = 65;
        hasSideEffect = false;
        moveDescription = "Has no side effect. 15 power points.";
    }
}
