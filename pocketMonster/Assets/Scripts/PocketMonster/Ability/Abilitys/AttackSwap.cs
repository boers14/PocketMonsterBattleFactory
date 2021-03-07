using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSwap : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Attack Swap";
        oneTime = false;
        instantEffect = true;
        abilityDescription = "Swaps the base attacking stats of the current two pocketmonsters for the rest of the battle. Can be used multiple times.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        float ownAttack = ownPocketMonster.stats.attack.baseStat;
        float ownSpAttack = ownPocketMonster.stats.specialAttack.baseStat;

        ownPocketMonster.stats.attack.baseStat = opponentPocketMonster.stats.attack.baseStat;
        ownPocketMonster.stats.specialAttack.baseStat = opponentPocketMonster.stats.specialAttack.baseStat;
        opponentPocketMonster.stats.attack.baseStat = ownAttack;
        opponentPocketMonster.stats.specialAttack.baseStat = ownSpAttack;

        ownPocketMonster.stats.attack.actualStat = ownPocketMonster.stats.attack.GetStatChanges(0);
        ownPocketMonster.stats.specialAttack.actualStat = ownPocketMonster.stats.specialAttack.GetStatChanges(0);
        opponentPocketMonster.stats.attack.actualStat = opponentPocketMonster.stats.attack.GetStatChanges(0);
        opponentPocketMonster.stats.specialAttack.actualStat = opponentPocketMonster.stats.specialAttack.GetStatChanges(0);

        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " swapped its special attack and attacking base stats with its opponent.", 
            false, false, false, false);
        base.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        if (target.stats.attack.baseStat >= 100 || target.stats.specialAttack.baseStat >= 100)
        {
            return true;
        }
        else
        {
            if (pocketMonster.stats.attack.baseStat > pocketMonster.stats.specialAttack.baseStat)
            {
                if (pocketMonster.stats.attack.baseStat <= target.stats.attack.baseStat)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (pocketMonster.stats.specialAttack.baseStat <= target.stats.specialAttack.baseStat)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
