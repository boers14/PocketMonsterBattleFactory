using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerOverwhelming : PocketMonsterAbility
{
    bool resetStats = false;

    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Power Overwhelming";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "Increases all stats by two stages for one turn, become paralyzed afterwards. " +
            "One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        if (player.opponentTrainer.firstTurn || player.lastPocketmonsterFainted)
        {
            if (player.pocketMonsters.Contains(ownPocketMonster))
            {
                IncreaseAllStats(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
                base.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
            } else
            {
                if (player.opponentTrainer.firstTurn || !player.opponentTrainer.wantsToSwitch)
                {
                    IncreaseAllStats(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
                } else
                {
                    ownPocketMonster.aftermathOfAbility = true;
                }
            }
        }
        else
        {
            ownPocketMonster.aftermathOfAbility = true;
        }
    }

    public override void UseAbilityAftermath(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        if (resetStats)
        {
            ownPocketMonster.ResetStats(false, true);
            ownPocketMonster.currentStatus = PocketMonster.StatusEffects.Paralyzed;
            ownPocketMonster.RecalculateStatsAfterStatus();
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " got paralyzed and reset its stats.", false, false, false, false);
        } else
        {
            IncreaseAllStats(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
        }
    }

    private void IncreaseAllStats(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        ownPocketMonster.stats.specialDefense.GetStatChanges(2);
        ownPocketMonster.stats.defense.GetStatChanges(2);
        ownPocketMonster.stats.attack.GetStatChanges(2);
        ownPocketMonster.stats.specialAttack.GetStatChanges(2);
        ownPocketMonster.stats.speed.GetStatChanges(2);
        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " increased its stats by two stages.", false, false, false, false);
        resetStats = true;
        ownPocketMonster.aftermathOfAbility = true;
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        if (pocketMonster.stats.speed.actualStat <= target.stats.speed.actualStat)
        {
            return true;
        }

        float damageDealt = trainerAi.CalculateComparativeDamage(pocketMonster, target, player);

        if (damageDealt < target.health && damageDealt * 2 >= target.health)
        {
            return true;
        }

        int faintedCounter = 0;

        for (int i = 0; i < player.pocketMonsters.Count; i++)
        {
            if (player.pocketMonsters[i].fainted)
            {
                faintedCounter++;
            }
        }

        if (player.pocketMonsters.Count - faintedCounter == 1)
        {
            return true;
        }

        return false;
    }
}
