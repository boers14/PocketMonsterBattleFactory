using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillingGift : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Killing Gift";
        onDeath = true;
        abilityDescription = "Deal 30% max health damage to the opponent and poison the opponent if they don't have any status yet when fainted.";
        base.SetAbilityStats(player);
    }

    public override void UseOnDeathAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager, bool isPlayer)
    {
        if (!opponentPocketMonster.fainted)
        {
            opponentPocketMonster.health -= Mathf.Ceil(opponentPocketMonster.stats.maxHealth * 0.3f);

            if (opponentPocketMonster.currentStatus == PocketMonster.StatusEffects.None)
            {
                opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.Poisened;
            }

            if (opponentPocketMonster.health <= 0)
            {
                opponentPocketMonster.fainted = true;
            }

            string message = opponentPocketMonster.stats.name + " got hurt by the " + abilityName + ".";

            if (player.pocketMonsters.Contains(ownPocketMonster))
            {
                inBattleTextManager.QueMessage(message, true, false, false, true);
            }
            else
            {
                inBattleTextManager.QueMessage(message, false, true, true, false);
            }
        }
    }
}
