using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceOfNature : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 80;
        powerPoints = 5;
        moveName = "Force of nature";
        moveType = PocketMonsterStats.Typing.Earth;
        moveSort = MoveSort.Special;
        baseDamage = 90;
        hasSideEffect = false;
        moveDescription = "Has no side effect. 5 power points.";
    }
}
