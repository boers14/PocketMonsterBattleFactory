using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSlap : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 15;
        moveName = "Water Slap";
        moveType = PocketMonsterStats.Typing.Water;
        moveSort = MoveSort.Physical;
        baseDamage = 65;
        moveDescription = "Has no side effect. 15 power points.";
    }
}
