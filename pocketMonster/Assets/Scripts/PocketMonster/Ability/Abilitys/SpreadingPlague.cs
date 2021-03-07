using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadingPlague : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Spreading Plague";
        oneTime = false;
        instantEffect = true;
        abilityDescription = "Inflicts a random status on both pocketmonsters when used. Overwrites current statusses. Can be used multiple times.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        int randomStatus = Random.Range(1, System.Enum.GetNames(typeof(PocketMonster.StatusEffects)).Length);
        PocketMonster.StatusEffects status = PocketMonster.StatusEffects.None;
        status -= randomStatus;
        opponentPocketMonster.currentStatus = status;
        ownPocketMonster.currentStatus = status;

        ownPocketMonster.RecalculateStatsAfterStatus();
        opponentPocketMonster.RecalculateStatsAfterStatus();

        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " used " + abilityName + ". " + ownPocketMonster.stats.name + " inflicted the " +
          status + " status on both pocketmonsters.", false, false, false, false);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        if (pocketMonster.currentStatus == PocketMonster.StatusEffects.None && target.currentStatus != PocketMonster.StatusEffects.None)
        {
            return false;
        } else
        {
            return true;
        }
    }
}
