using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthGiver : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Health Giver";
        onDeath = true;
        abilityDescription = "After fainting restores the next pocketmonster that is send out to full health.";
        base.SetAbilityStats(player);
    }

    public override void UseOnDeathAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move,
        InBattleTextManager inBattleTextManager, bool isPlayer)
    {
        if (player.pocketMonsters.Contains(ownPocketMonster))
        {
            player.healNextPocketMonster = true;
        } else
        {
            player.opponentTrainer.healNextPocketMonster = true;
        }
    }
}
