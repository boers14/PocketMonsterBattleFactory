using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCleanse : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Status Cleanse";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "Sets the statusses of all pocketmonsters in the team to none. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        List<PocketMonster> teamToCleanse;

        if (player.pocketMonsters.Contains(ownPocketMonster))
        {
            teamToCleanse = player.pocketMonsters;
        } else
        {
            teamToCleanse = player.opponentTrainer.pocketMonsters;
        }

        int pocketmonstersNotCleansed = 0;

        for (int i = 0; i < teamToCleanse.Count; i++)
        {
            if (teamToCleanse[i].currentStatus == PocketMonster.StatusEffects.None)
            {
                pocketmonstersNotCleansed++;
            } else
            {
                teamToCleanse[i].currentStatus = PocketMonster.StatusEffects.None;
                teamToCleanse[i].CheckForParalazys();
            }
        }

        if (pocketmonstersNotCleansed == teamToCleanse.Count)
        {
            hasBeenUsed = false;
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " failed to cure any status conditions.", false, false, false, false);
        } else
        {
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " set all status conditions to none.", false, false, false, false);
        }

        player.SetSwitchButtonDescription();
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        int pocketmonstersNotCleansed = 0;

        for (int i = 0; i < player.opponentTrainer.pocketMonsters.Count; i++)
        {
            if (player.opponentTrainer.pocketMonsters[i].currentStatus == PocketMonster.StatusEffects.None)
            {
                pocketmonstersNotCleansed++;
            }
        }

        if (pocketmonstersNotCleansed == player.opponentTrainer.pocketMonsters.Count)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
