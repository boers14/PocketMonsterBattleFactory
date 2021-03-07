using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckyCharm : PocketMonsterItem
{
    public override void SetStats()
    {
        everyTurn = true;
        name = "Lucky charm";
        itemDescription = "Boosts the chance of a getting a critical hit by 10% every turn. The amplified crit chance" +
            " resets on switch.";
        itemSort = ItemSort.Defense;
    }

    public override void GrantEveryTurnEffect(PocketMonster effectedPocketMonster, InBattleTextManager inBattleTextManager)
    {
        if (effectedPocketMonster.stats.critChance < 100)
        {
            effectedPocketMonster.stats.critChance += 10;

            if (effectedPocketMonster.stats.critChance > 100)
            {
                effectedPocketMonster.stats.critChance = 100;
            }

            inBattleTextManager.QueMessage("The " + name + " of " + effectedPocketMonster.stats.name + " boosted it's critchance."
                , false, false, false, false);
        }
    }
}
