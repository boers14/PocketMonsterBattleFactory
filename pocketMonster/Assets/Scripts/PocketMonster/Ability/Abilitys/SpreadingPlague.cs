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
        abilityDescription = "Inflicts a random status on opponent pocketmonster when used. Overwrites current status. Can be used multiple times.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        int randomStatus = Random.Range(1, System.Enum.GetNames(typeof(PocketMonster.StatusEffects)).Length);
        PocketMonster.StatusEffects status = PocketMonster.StatusEffects.None;
        status -= randomStatus;
        opponentPocketMonster.currentStatus = status;

        if (status == PocketMonster.StatusEffects.Paralyzed)
        {
            opponentPocketMonster.stats.speed.actualStat = opponentPocketMonster.stats.speed.GetStatChanges(0) / 4;
        } 

        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " used " + abilityName + ". " + ownPocketMonster.stats.name + " inflicted the " +
          status + " status on opponent pocketmonster.", false, false, false, false);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        if (target.currentStatus == PocketMonster.StatusEffects.None)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
