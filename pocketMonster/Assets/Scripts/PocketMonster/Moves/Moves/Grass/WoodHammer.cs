using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodHammer : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 80;
        powerPoints = 5;
        moveName = "Wood hammer";
        moveType = PocketMonsterStats.Typing.Grass;
        moveSort = MoveSort.Physical;
        baseDamage = 90;
        hasSideEffect = false;
        moveDescription = "Has no side effect. 5 power points.";
    }
}
