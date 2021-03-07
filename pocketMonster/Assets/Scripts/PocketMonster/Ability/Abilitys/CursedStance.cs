using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedStance : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Cursed Stance";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "Increase the defense and special defense by two stages, but become cursed. " +
            "One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        ownPocketMonster.stats.specialDefense.GetStatChanges(2);
        ownPocketMonster.stats.defense.GetStatChanges(2);
        ownPocketMonster.currentStatus = PocketMonster.StatusEffects.Cursed;
        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " increased its defense and special defense by two stages, but got cursed.", 
            false, false, false, false);
        base.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        return GetDecisionForTrainerAiInCaseOfSwapping(trainerAi, target);
    }
}
