using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindTrick : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 15;
        moveName = "Mind trick";
        moveType = PocketMonsterStats.Typing.Regular;
        moveSort = MoveSort.Special;
        baseDamage = 65;
        hasSideEffect = false;
        moveDescription = "Has no side effect. 15 power points.";
    }
}
