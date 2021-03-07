using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDestroyer : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Item Destroyer";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "Destroys all items of the opponent. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        if (opponentPocketMonster.items.Count > 0)
        {
            opponentPocketMonster.items.Clear();
            inBattleTextManager.QueMessage("The items of " + opponentPocketMonster.stats.name + " have been destroyed.", false, false, false, false);
        }
        else
        {
            hasBeenUsed = false;
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " didn't find any items to destroy.", false, false, false, false);
        }
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        if (target.items.Count > 0)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
