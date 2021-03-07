using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperTantrum : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Temper Tantrum";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "If the pocketmonster is under 40% health gain maximum attack and speed. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        if (ownPocketMonster.health <= ownPocketMonster.stats.maxHealth * 0.4f)
        {
            ownPocketMonster.stats.attack.GetStatChanges(12);
            ownPocketMonster.stats.speed.GetStatChanges(12);
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " maximized it's attack and speed.", false, false, false, false);
            base.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
        } else
        {
            hasBeenUsed = false;
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " failed to maximize it's attack and speed.", false, false, false, false);
        }
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        if (pocketMonster.health <= pocketMonster.stats.maxHealth * 0.4f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
