using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificialPact : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Sacrificial Pact";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "If the opponent is at 80% or lower execute the opponent, but also executes itself. Must be at the same amount " +
            "or a lower health amount then the opponent to use. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        string baseText = ownPocketMonster.stats.name + " used it's ability " + abilityName + ". ";

        if (ownPocketMonster.health / ownPocketMonster.stats.maxHealth < opponentPocketMonster.health / opponentPocketMonster.stats.maxHealth || 
            opponentPocketMonster.health > opponentPocketMonster.stats.maxHealth * 0.8f)
        {
            if (opponentPocketMonster.stats.maxHealth > 1)
            {
                hasBeenUsed = false;
                inBattleTextManager.QueMessage(baseText + opponentPocketMonster.stats.name + " can't get executed yet.", false, false, false, false);
                return;
            }
        }

        bool isPlayer = false;

        ownPocketMonster.health = 0;
        ownPocketMonster.fainted = true;
        opponentPocketMonster.health = 0;
        opponentPocketMonster.fainted = true;
        inBattleTextManager.QueMessage(baseText + ownPocketMonster.stats.name + " has executed " + opponentPocketMonster.stats.name + " in a " +
            "sacrificial pact.", true, true, true, true);

        if (player.pocketMonsters.Contains(ownPocketMonster))
        {
            isPlayer = true;
        }

        if (ownPocketMonster.ability.onDeath)
        {
            ownPocketMonster.currentStatus = PocketMonster.StatusEffects.None;
            ownPocketMonster.ability.UseOnDeathAbility(ownPocketMonster, opponentPocketMonster, null, inBattleTextManager, isPlayer);
        }

        if (opponentPocketMonster.ability.onDeath)
        {
            opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.None;
            opponentPocketMonster.ability.UseOnDeathAbility(opponentPocketMonster, ownPocketMonster, null, inBattleTextManager, !isPlayer);
        }
        player.CheckIfBattleOver();
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        if (pocketMonster.health / pocketMonster.stats.maxHealth < target.health / target.stats.maxHealth ||
                        target.health > target.stats.maxHealth * 0.8f)
        {
            return false;
        }

        int aliveCounter = 0;
        for (int i = 0; i < trainerAi.pocketMonsters.Count; i++)
        {
            if (!trainerAi.pocketMonsters[i].fainted)
            {
                aliveCounter++;
            }
        }

        if (aliveCounter > 1)
        {
            bool winsMatchup = false;
            if (pocketMonster.stats.speed.actualStat > target.stats.speed.actualStat)
            {
                float damageDealt = trainerAi.CalculateComparativeDamage(pocketMonster, target, player);
                if (damageDealt >= target.health)
                {
                    winsMatchup = true;
                }
            }

            if (!winsMatchup)
            {
                if (pocketMonster.health / pocketMonster.stats.maxHealth < target.health / target.stats.maxHealth ||
                        target.health > target.stats.maxHealth * 0.8f)
                {
                    if (target.stats.maxHealth > 1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            } else
            {
                return false;
            }
        } else
        {
            return false;
        }
    }
}
