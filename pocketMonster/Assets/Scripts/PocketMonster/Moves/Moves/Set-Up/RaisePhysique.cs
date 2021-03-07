using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaisePhysique : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Raise physique";
        moveType = PocketMonsterStats.Typing.Regular;
        moveSort = MoveSort.Status;
        typeOfBoost = TypeOfBoostMove.PhysicalOffensive;
        hasSideEffect = true;
        chanceOfSideEffect = 100;
        moveDescription = "Raise attack by 2 stages. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        GainStatBoost(ownPocketMonster, ownPocketMonster.stats.attack, inBattleTextManager, "attack", 2);
    }
}
