using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpookPunch : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 15;
        moveName = "Spook punch";
        moveType = PocketMonsterStats.Typing.Spooky;
        moveSort = MoveSort.Physical;
        baseDamage = 65;
        hasSideEffect = false;
        moveDescription = "Has no side effect. 15 power points.";
    }
}
