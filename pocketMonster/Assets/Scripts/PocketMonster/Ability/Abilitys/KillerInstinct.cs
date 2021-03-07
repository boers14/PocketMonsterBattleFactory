using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerInstinct : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Killer Instinct";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "If the opponent is at 30% or lower execute the opponent. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        string baseText = ownPocketMonster.stats.name + " used it's ability " + abilityName + ". ";

        if (opponentPocketMonster.health > opponentPocketMonster.stats.maxHealth * 0.3f + 1)
        {
            hasBeenUsed = false;
            inBattleTextManager.QueMessage(baseText + opponentPocketMonster.stats.name + " can't get executed yet.", false, false, false, false);
            return;
        }

        bool isPlayer = false;
        opponentPocketMonster.health = 0;
        opponentPocketMonster.fainted = true;
        string message = baseText + ownPocketMonster.stats.name + " has executed " + opponentPocketMonster.stats.name + ".";

        if (player.pocketMonsters.Contains(ownPocketMonster))
        {
            isPlayer = true;
            inBattleTextManager.QueMessage(message, true, false, false, true);
        } else
        {
            inBattleTextManager.QueMessage(message, false, true, true, false);
        }

        if (opponentPocketMonster.ability.onDeath)
        {
            opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.None;
            opponentPocketMonster.ability.UseOnDeathAbility(opponentPocketMonster, ownPocketMonster, null, inBattleTextManager, !isPlayer);
        }

        player.CheckIfBattleOver();
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        if (target.health > target.stats.maxHealth * 0.3f + 1)
        {
            return false;
        } else
        {
            return true;
        }
    }
}
