using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeLens : PocketMonsterItem
{
    public override void SetStats()
    {
        attackTurn = true;
        name = "Scope lens";
        itemDescription = "Increases damage done with super effective moves by 20%, but lowers not very effective move damage by 20%.";
        itemSort = ItemSort.GeneralOffense;
    }

    public override void GrantAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move,
        PocketMonster opponentPocketmonster, InBattleTextManager inBattleTextManager)
    {
        float damageMultiplier = opponentPocketmonster.CalculateInTyping(move.moveType);

        if (move.moveSort != PocketMonsterMoves.MoveSort.Status && opponentPocketmonster.amountOfDamageTaken > 0)
        {
            if (damageMultiplier >= 2)
            {
                opponentPocketmonster.amountOfDamageTaken *= 1.2f;
                inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " did increased damage due to the " + name + "."
                    , false, false, false, false);
            } else if (damageMultiplier <= 0.5)
            {
                opponentPocketmonster.amountOfDamageTaken *= 0.8f;
                inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " did decreased damage due to the " + name + "."
                    , false, false, false, false);
            }
        }
    }

    public override float CalculateDamageForAi(float damageDone, PocketMonster pocketMonster, float damageMultiplier, PocketMonsterMoves move, int index)
    {
        if (damageMultiplier >= 2)
        {
            damageDone *= 1.2f;
        }
        else if (damageMultiplier <= 0.5)
        {
            damageDone *= 0.8f;
        }

        return damageDone;
    }
}
