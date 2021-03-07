using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydroSlap : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 80;
        powerPoints = 5;
        moveName = "Hydro Slap";
        moveType = PocketMonsterStats.Typing.Water;
        moveSort = MoveSort.Physical;
        baseDamage = 90;
        moveDescription = "Has no side effect. 5 power points.";
    }
}
