using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseMental : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Raise mental";
        moveType = PocketMonsterStats.Typing.Regular;
        moveSort = MoveSort.Status;
        typeOfBoost = TypeOfBoostMove.SpecialOffensive;
        hasSideEffect = true;
        chanceOfSideEffect = 100;
        moveDescription = "Raise special attack by 2 stages. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        GainStatBoost(ownPocketMonster, ownPocketMonster.stats.specialAttack, inBattleTextManager, "special attack", 2);
    }
}
