using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaknessIndentifier : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Weakness Indentifier";
        instantEffect = true;
        oneTime = true;
        abilityDescription = "Drops the defensive stats of the opponent to the minimum. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        opponentPocketMonster.stats.defense.GetStatChanges(-12);
        opponentPocketMonster.stats.specialDefense.GetStatChanges(-12);
        inBattleTextManager.QueMessage("The defensive stats of " + opponentPocketMonster.stats.name + " got dropped to the minimum.",
            false, false, false, false);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        return true;
    }
}
