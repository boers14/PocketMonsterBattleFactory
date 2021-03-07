using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveTraining : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Defensive training";
        moveType = PocketMonsterStats.Typing.Regular;
        moveSort = MoveSort.Status;
        typeOfBoost = TypeOfBoostMove.GeneralDefense;
        hasSideEffect = true;
        chanceOfSideEffect = 100;
        moveDescription = "Raise defense and special defense by one stage. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
            {
                GainStatBoost(ownPocketMonster, ownPocketMonster.stats.defense, inBattleTextManager, "defense", 1);
            } else
            {
                GainStatBoost(ownPocketMonster, ownPocketMonster.stats.specialDefense, inBattleTextManager, "special defense", 1);
            }
        }
    }
}
