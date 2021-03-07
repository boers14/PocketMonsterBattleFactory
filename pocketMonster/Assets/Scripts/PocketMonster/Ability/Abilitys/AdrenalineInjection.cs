using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrenalineInjection : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Adrenaline Injection";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "Increase critical hit ratio to 100%, but become nearsighted. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        ownPocketMonster.currentStatus = PocketMonster.StatusEffects.Nearsighted;
        ownPocketMonster.stats.critChance = 100;
        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " increased its critchance to 100%, but got nearsighted.", false, false, false, false);
        base.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        int faintedCounter = 0;

        for(int i = 0; i< player.pocketMonsters.Count; i++)
        {
            if (player.pocketMonsters[i].fainted)
            {
                faintedCounter++;
            }
        }

        if (player.pocketMonsters.Count - faintedCounter == 1)
        {
            float damageDealt = trainerAi.CalculateComparativeDamage(pocketMonster, target, player);
            if (damageDealt >= target.health)
            {
                return false;
            } else
            {
                return true;
            }
        } else
        {
            return true;
        }
    }
}
