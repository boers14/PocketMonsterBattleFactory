using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leftovers : PocketMonsterItem
{
    private PlayerBattle playerBattle;
    public override void SetStats()
    {
        onSwitch = true;
        everyTurn = true;
        name = "Leftovers";
        itemDescription = "Heals 6% of max health at every end of turn.";
        itemSort = ItemSort.Defense;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        playerBattle = player;
    }

    public override void GrantEveryTurnEffect(PocketMonster effectedPocketMonster, InBattleTextManager inBattleTextManager)
    {
        if (effectedPocketMonster.health < effectedPocketMonster.stats.maxHealth)
        {
            effectedPocketMonster.health += effectedPocketMonster.stats.maxHealth * 0.06f;
            effectedPocketMonster.RecalculateHealth();
            string message = effectedPocketMonster.stats.name + " healed a little bit from it's " + name + ".";
            if (playerBattle.pocketMonsters.Contains(effectedPocketMonster))
            {
                inBattleTextManager.QueMessage(message, false, true, false, false);
            } else
            {
                inBattleTextManager.QueMessage(message, true, false, false, false);
            }
        }
    }
}
