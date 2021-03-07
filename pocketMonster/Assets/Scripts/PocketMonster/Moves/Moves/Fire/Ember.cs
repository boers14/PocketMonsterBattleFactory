using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ember : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 15;
        moveName = "Ember";
        moveType = PocketMonsterStats.Typing.Fire;
        moveSort = MoveSort.Special;
        baseDamage = 65;
        hasSideEffect = false;
        moveDescription = "Has no side effect. 15 power points.";
    }
}
