using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampericOrb : PocketMonsterItem
{
    private PlayerBattle playerBattle;

    public override void SetStats()
    {
        endOfDamageCalc = true;
        onSwitch = true;
        name = "Vamperic orb";
        itemDescription = "Restore 50% of damage dealt to the opponent, but decrease speed by one stage on switch in.";
        itemSort = ItemSort.Defense;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        effectedPocketMonster.stats.speed.GetStatChanges(-1);
        inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " lowered it's speed by one stage due to the " + name + "."
            , false, false, false, false);
        playerBattle = player;
    }

    public override void GrantEndOfDamageCalcEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move, PocketMonster opponentPocketMonster,
        InBattleTextManager inBattleTextManager, bool defenseTurn)
    {
        if (!defenseTurn)
        {
            return;
        }

        if (opponentPocketMonster.amountOfDamageTaken > 0 && effectedPocketMonster.health < effectedPocketMonster.stats.maxHealth 
            && !effectedPocketMonster.fainted)
        {
            float damageDone = opponentPocketMonster.amountOfDamageTaken;

            if (damageDone > opponentPocketMonster.health)
            {
                damageDone = opponentPocketMonster.health;
            } 

            float restoredHealth = damageDone / 2;
            if (restoredHealth > 0)
            {
                effectedPocketMonster.health += restoredHealth;
                if (effectedPocketMonster.health > effectedPocketMonster.stats.maxHealth)
                {
                    restoredHealth -= effectedPocketMonster.health - effectedPocketMonster.stats.maxHealth;
                    effectedPocketMonster.health = effectedPocketMonster.stats.maxHealth;
                }
                string message = effectedPocketMonster.stats.name + " restored " + (int)restoredHealth + " health due to the " + name + ".";

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

    public override float CalculateSpeedStatChanges(float speedStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        speedStat /= 1.5f;
        return speedStat;
    }
}
