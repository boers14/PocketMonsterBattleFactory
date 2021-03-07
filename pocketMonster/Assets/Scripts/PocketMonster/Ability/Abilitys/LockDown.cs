using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDown : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Lock Down";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "Raise the speed stat by one stage and set the status of the opponent to trapped. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        ownPocketMonster.stats.speed.GetStatChanges(1);
        opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.Trapped;

        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " raised it's speed by one stage and trapped " + 
            opponentPocketMonster.stats.name + ".", false, false, false, false);
        base.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        return GetDecisionForTrainerAiInCaseOfSwapping(trainerAi, target);
    }
}
