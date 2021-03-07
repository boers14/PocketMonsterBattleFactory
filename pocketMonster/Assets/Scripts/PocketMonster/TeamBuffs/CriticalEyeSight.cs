using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalEyeSight : PocketMonsterItem
{
    public override void SetStats()
    {
        attackTurn = true;
        name = "Critical eye sight";
        itemDescription = "Super effective moves deal 10% more damage.";
    }

    public override void GrantAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move,
        PocketMonster opponentPocketmonster, InBattleTextManager inBattleTextManager)
    {
        float damageMultiplier = opponentPocketmonster.CalculateInTyping(move.moveType);

        if (damageMultiplier >= 2)
        {
            opponentPocketmonster.amountOfDamageTaken *= 1.1f;
            inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " dealt extra damage due to the " + name + ".", false, false, false, false);
        }
    }

    public override float CalculateDamageForAi(float damageDone, PocketMonster pocketMonster, float damageMultiplier, PocketMonsterMoves move, int index)
    {
        if (damageMultiplier >= 2)
        {
            damageDone *= 1.1f;
        }

        return damageDone;
    }
}
