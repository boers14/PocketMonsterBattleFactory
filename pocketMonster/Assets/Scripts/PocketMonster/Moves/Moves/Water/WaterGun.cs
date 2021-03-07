using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGun : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 15;
        moveName = "Water gun";
        moveType = PocketMonsterStats.Typing.Water;
        moveSort = MoveSort.Special;
        baseDamage = 65;
        moveDescription = "Has no side effect. 15 power points.";
    }
}
