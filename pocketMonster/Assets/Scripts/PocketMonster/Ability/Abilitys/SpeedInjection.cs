using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedInjection : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Speed Injection";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "Increases the speed stat by two stages, but become poisened. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    { 
        ownPocketMonster.currentStatus = PocketMonster.StatusEffects.Poisened;
        ownPocketMonster.stats.speed.GetStatChanges(2);
        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " raised it's speed by 2 stages, but became poisened.", false, false, false, false);
        base.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
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
            if (pocketMonster.stats.speed.actualStat > target.stats.speed.actualStat)
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
    }
}
