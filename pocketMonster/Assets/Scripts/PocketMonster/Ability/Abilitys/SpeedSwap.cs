using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSwap : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Speed Swap";
        oneTime = false;
        instantEffect = true;
        abilityDescription = "Swaps the base speed stat of the current two pocketmonsters for the rest of the battle. Can be used multiple times.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        float ownSpeed = ownPocketMonster.stats.speed.baseStat;

        ownPocketMonster.stats.speed.baseStat = opponentPocketMonster.stats.speed.baseStat;
        opponentPocketMonster.stats.speed.baseStat = ownSpeed;

        ownPocketMonster.stats.speed.actualStat = ownPocketMonster.stats.speed.GetStatChanges(0);
        opponentPocketMonster.stats.speed.actualStat = opponentPocketMonster.stats.speed.GetStatChanges(0);

        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " swapped its speed base stat with its opponent.", false, false, false, false);
        base.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        if (target.stats.speed.baseStat > pocketMonster.stats.speed.baseStat)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
