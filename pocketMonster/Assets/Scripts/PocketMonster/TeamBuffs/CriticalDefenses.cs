using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalDefenses : PocketMonsterItem
{
    public override void SetStats()
    {
        defenseTurn = true;
        name = "Critical defenses";
        itemDescription = "Take 20% less damage from super effective moves.";
    }

    public override void GrantAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move,
        PocketMonster opponentPocketmonster, InBattleTextManager inBattleTextManager)
    {
        if (effectedPocketMonster.amountOfDamageTaken > 0)
        {
            float damageMultiplier = effectedPocketMonster.CalculateInTyping(move.moveType);

            if (damageMultiplier >= 2)
            {
                effectedPocketMonster.amountOfDamageTaken *= 0.8f;
                inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " took less damage due to the " + name + ".", false, false, false, false);
            }
        }
    }

    public override float CalculateDamageForAi(float damageDone, PocketMonster pocketMonster, float damageMultiplier, PocketMonsterMoves move, int index)
    {
        if (damageMultiplier >= 2)
        {
            damageDone *= 0.8f;
        }

        return damageDone;
    }
}
