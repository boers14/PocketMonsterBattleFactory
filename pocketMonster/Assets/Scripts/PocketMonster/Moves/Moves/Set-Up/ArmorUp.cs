using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorUp : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Armor up";
        moveType = PocketMonsterStats.Typing.Regular;
        moveSort = MoveSort.Status;
        typeOfBoost = TypeOfBoostMove.PhysicalDefensive;
        hasSideEffect = true;
        chanceOfSideEffect = 100;
        moveDescription = "Raise defense by 2 stages. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        GainStatBoost(ownPocketMonster, ownPocketMonster.stats.defense, inBattleTextManager, "defense", 2);
    }
}
