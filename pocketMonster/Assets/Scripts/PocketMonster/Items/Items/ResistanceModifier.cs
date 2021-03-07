using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResistanceModifier : PocketMonsterItem
{
    public override void SetStats()
    {
        defenseTurn = true;
        name = "Resistance modifier";
        itemDescription = "Decreases damage done from not very effective moves by 30%, but increases damage done from super effective moves by 20%.";
        itemSort = ItemSort.GeneralOffense;
    }

    public override void GrantAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move,
        PocketMonster opponentPocketmonster, InBattleTextManager inBattleTextManager)
    {
        if (move.moveSort != PocketMonsterMoves.MoveSort.Status && effectedPocketMonster.amountOfDamageTaken > 0)
        {
            float damageMultiplier = effectedPocketMonster.CalculateInTyping(move.moveType);

            if (damageMultiplier <= 0.5f)
            {
                effectedPocketMonster.amountOfDamageTaken *= 0.7f;
                inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " took less damage from the attack due to the " + name + "."
                    , false, false, false, false);
            }
            else if (damageMultiplier >= 2)
            {
                effectedPocketMonster.amountOfDamageTaken *= 1.2f;
                inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " took more damage from the attack due to the " + name + "."
                    , false, false, false, false);
            }
        }
    }

    public override float CalculateDamageForAi(float damageDone, PocketMonster pocketMonster, float damageMultiplier, PocketMonsterMoves move, int index)
    {
        if (damageMultiplier <= 0.5f)
        {
            damageDone *= 0.7f;
        }
        else if (damageMultiplier >= 2)
        {
            damageDone *= 1.2f;
        }

        return damageDone;
    }
}
