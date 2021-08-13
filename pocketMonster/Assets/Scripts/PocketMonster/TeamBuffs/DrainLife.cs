using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrainLife : PocketMonsterItem
{
    private PlayerBattle playerBattle;

    public override void SetStats()
    {
        endOfDamageCalc = true;
        name = "Drain life";
        itemDescription = "Heals for 33% of the damage dealt.";
    }

    public override void GrantEndOfDamageCalcEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move, PocketMonster opponentPocketMonster,
        InBattleTextManager inBattleTextManager, bool defenseTurn)
    {
        if (!defenseTurn)
        {
            return;
        }

        if (playerBattle == null)
        {
            GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
            playerBattle = player.GetComponent<PlayerBattle>();
        }

        if (effectedPocketMonster.health < effectedPocketMonster.stats.maxHealth && !effectedPocketMonster.fainted)
        {
            float damageDone = opponentPocketMonster.amountOfDamageTaken;
            if (damageDone != 0)
            {
                if (damageDone > opponentPocketMonster.health)
                {
                    damageDone = opponentPocketMonster.health;
                }

                float regainedHealth = damageDone * 0.33f;
                effectedPocketMonster.health += regainedHealth;

                if (effectedPocketMonster.health > effectedPocketMonster.stats.maxHealth)
                {
                    regainedHealth -= effectedPocketMonster.health - effectedPocketMonster.stats.maxHealth;
                    effectedPocketMonster.health = effectedPocketMonster.stats.maxHealth;
                }
                string message = effectedPocketMonster.stats.name + " got " + (int)regainedHealth + " health back from the attack from the " +
                    name + ".";

                if (playerBattle.pocketMonsters.Contains(effectedPocketMonster))
                {
                    inBattleTextManager.QueMessage(message, false, true, false, false);
                }
                else
                {
                    inBattleTextManager.QueMessage(message, true, false, false, false);
                }
            }

        }
    }
}
