using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatNeutralizer : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Stat Neutralizer";
        oneTime = false;
        instantEffect = true;
        abilityDescription = "Resets the stats of the opponent pocketmonster. Can be used multiple times.";
        base.SetAbilityStats(player);

    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        opponentPocketMonster.ResetStats(false, false);
        player.SetSwitchButtonDescription();
        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " resetted the stats of " + opponentPocketMonster.stats.name + ".", 
            false, false, false, false);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        BoostableStat.BoostAmount baseBoostAmount = BoostableStat.BoostAmount.X0;

        if (target.stats.speed.boostAmount > baseBoostAmount || target.stats.attack.boostAmount > baseBoostAmount ||
            target.stats.specialDefense.boostAmount > baseBoostAmount || target.stats.specialAttack.boostAmount > baseBoostAmount ||
            target.stats.defense.boostAmount > baseBoostAmount)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
