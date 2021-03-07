using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoBackingOut : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "No Backing Out";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "Increase all stats by one stage but become trapped. " +
            "One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        ownPocketMonster.stats.specialDefense.GetStatChanges(1);
        ownPocketMonster.stats.defense.GetStatChanges(1);
        ownPocketMonster.stats.attack.GetStatChanges(1);
        ownPocketMonster.stats.specialAttack.GetStatChanges(1);
        ownPocketMonster.stats.speed.GetStatChanges(1);
        ownPocketMonster.currentStatus = PocketMonster.StatusEffects.Trapped;
        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " increased all it's stats by one stage, but it trapped it's self.",
            false, false, false, false);
        base.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        return GetDecisionForTrainerAiInCaseOfSwapping(trainerAi, target);
    }
}
