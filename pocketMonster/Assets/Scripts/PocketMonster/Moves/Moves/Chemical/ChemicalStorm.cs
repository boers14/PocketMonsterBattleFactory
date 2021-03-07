using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChemicalStorm : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 80;
        powerPoints = 5;
        moveName = "Chemical storm";
        moveType = PocketMonsterStats.Typing.Chemical;
        moveSort = MoveSort.Special;
        baseDamage = 90;
        hasSideEffect = false;
        moveDescription = "Has no side effect. 5 power points.";
    }
}
