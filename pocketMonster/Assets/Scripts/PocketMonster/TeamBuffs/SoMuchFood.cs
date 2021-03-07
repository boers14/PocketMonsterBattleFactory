using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoMuchFood : PocketMonsterItem
{
    private PlayerBattle playerBattle;

    public override void SetStats()
    {
        everyTurn = true;
        name = "So much food";
        itemDescription = "Heals 5% of health after every turn.";
    }

    public override void GrantEveryTurnEffect(PocketMonster effectedPocketMonster, InBattleTextManager inBattleTextManager)
    {
        if (playerBattle == null)
        {
            GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
            playerBattle = player.GetComponent<PlayerBattle>();
        }

        if (effectedPocketMonster.health < effectedPocketMonster.stats.maxHealth)
        {
            effectedPocketMonster.health += effectedPocketMonster.stats.maxHealth * 0.06f;
            effectedPocketMonster.RecalculateHealth();
            string message = effectedPocketMonster.stats.name + " healed a little bit from " + name + ".";

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
