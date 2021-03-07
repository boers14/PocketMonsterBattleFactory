using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodKiller : PocketMonsterAbility
{
    private BoostableStat.BoostAmount attackAmount = 0;
    private BoostableStat.BoostAmount spAttackAmount = 0;
    private BoostableStat.BoostAmount speedAmount = 0;
    private PocketMonster effectedPocketMonster;

    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Mood Killer";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "Lower the attacking and speed stats of the opponent to the minimum for one turn. " +
            "One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        effectedPocketMonster = opponentPocketMonster;
        attackAmount = opponentPocketMonster.stats.attack.boostAmount;
        spAttackAmount = opponentPocketMonster.stats.specialAttack.boostAmount;
        speedAmount = opponentPocketMonster.stats.speed.boostAmount;

        opponentPocketMonster.stats.attack.GetStatChanges(-12);
        opponentPocketMonster.stats.specialAttack.GetStatChanges(-12);
        opponentPocketMonster.stats.speed.GetStatChanges(-12);
        ownPocketMonster.aftermathOfAbility = true;
        inBattleTextManager.QueMessage("The attacking and speed stats of " + opponentPocketMonster.stats.name + " got lowered to the minimum.",
            false, false, false, false);
    }

    public override void UseAbilityAftermath(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        if (opponentPocketMonster == effectedPocketMonster)
        {
            if (opponentPocketMonster.stats.attack.boostAmount < BoostableStat.BoostAmount.X0 ||
                opponentPocketMonster.stats.specialAttack.boostAmount < BoostableStat.BoostAmount.X0 ||
                opponentPocketMonster.stats.speed.boostAmount < BoostableStat.BoostAmount.X0)
            {
                opponentPocketMonster.stats.attack.boostAmount = attackAmount;
                opponentPocketMonster.stats.specialAttack.boostAmount = spAttackAmount;
                opponentPocketMonster.stats.speed.boostAmount = speedAmount;

                opponentPocketMonster.stats.attack.GetStatChanges(0);
                opponentPocketMonster.stats.specialAttack.GetStatChanges(0);
                opponentPocketMonster.stats.speed.GetStatChanges(0);
                inBattleTextManager.QueMessage("The attacking and speed stats of " + opponentPocketMonster.stats.name + " got restored.",
                    false, false, false, false);
            }
        }
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        return true;
    }
}
