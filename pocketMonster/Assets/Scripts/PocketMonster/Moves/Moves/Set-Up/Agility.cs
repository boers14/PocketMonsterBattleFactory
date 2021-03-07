using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agility : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Agility";
        moveType = PocketMonsterStats.Typing.Regular;
        moveSort = MoveSort.Status;
        typeOfBoost = TypeOfBoostMove.Speed;
        hasSideEffect = true;
        chanceOfSideEffect = 100;
        moveDescription = "Raise speed by 2 stages. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        GainStatBoost(ownPocketMonster, ownPocketMonster.stats.speed, inBattleTextManager, "speed", 2);
    }
}
