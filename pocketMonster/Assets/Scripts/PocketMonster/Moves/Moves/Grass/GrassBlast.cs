using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBlast : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 15;
        moveName = "Grass blast";
        moveType = PocketMonsterStats.Typing.Grass;
        moveSort = MoveSort.Special;
        baseDamage = 65;
        hasSideEffect = false;
        moveDescription = "Has no side effect. 15 power points.";
    }
}
