using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleansedMind : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Cleansed Mind";
        oneTime = false;
        instantEffect = true;
        abilityDescription = "Sets the current status of both pocketmonsters to none. If a status was changed raise both defenses by one stage." +
            " Can be used multiple times.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        string basetext = ownPocketMonster.stats.name + " used " + abilityName + ". ";
        string extraText = "";

        if (ownPocketMonster.currentStatus != PocketMonster.StatusEffects.None || opponentPocketMonster.currentStatus != PocketMonster.StatusEffects.None)
        {
            ownPocketMonster.currentStatus = PocketMonster.StatusEffects.None;
            opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.None;
            extraText += ownPocketMonster.stats.name + " set both statusses to none. " + ownPocketMonster.stats.name + " raised both defenses by on stage.";
            ownPocketMonster.stats.specialDefense.GetStatChanges(1);
            ownPocketMonster.stats.defense.GetStatChanges(1);
            ownPocketMonster.CheckForParalazys();
            opponentPocketMonster.CheckForParalazys();
            base.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
        } else
        {
            extraText = " It failed.";
        }

        inBattleTextManager.QueMessage(basetext + extraText, false, false, false, false);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        if (pocketMonster.currentStatus != PocketMonster.StatusEffects.None || target.currentStatus != PocketMonster.StatusEffects.None)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
