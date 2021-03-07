using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveTraining : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Offensive training";
        moveType = PocketMonsterStats.Typing.Regular;
        moveSort = MoveSort.Status;
        typeOfBoost = TypeOfBoostMove.GeneralOffense;
        hasSideEffect = true;
        chanceOfSideEffect = 100;
        moveDescription = "Raise attack and special attack by one stage. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
            {
                GainStatBoost(ownPocketMonster, ownPocketMonster.stats.attack, inBattleTextManager, "attack", 1);
            } else
            {
                GainStatBoost(ownPocketMonster, ownPocketMonster.stats.specialAttack, inBattleTextManager, "special attack", 1);
            }
        }
    }
}
