using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensivePreparations : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Defensive Preparations";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "Boosts the defense and special defense stat of the next pocketmonster in the party by one stage. Will fail if the next pocketmonster is" +
            " fainted. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        PocketMonster trainedPocketmonster = null;

        List<PocketMonster> listToSearchThrough = GetTeamToAffect(ownPocketMonster, player);

        trainedPocketmonster = GetBuffedPocketMonster(trainedPocketmonster, ownPocketMonster, listToSearchThrough);

        if (trainedPocketmonster != null)
        {
            trainedPocketmonster.stats.defense.GetStatChanges(1);
            trainedPocketmonster.stats.specialDefense.GetStatChanges(1);
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " buffed one of it's team members.", false, false, false, false);
        }
        else if (listToSearchThrough.Count == 1)
        {
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " has no team member.", false, false, false, false);
        } else
        {
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " failed to buff its team member.", false, false, false, false);
        }

        player.SetSwitchButtonDescription();
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        return true;
    }
}
