using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrainPunch : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Drain punch";
        moveType = PocketMonsterStats.Typing.Regular;
        moveSort = MoveSort.Physical;
        baseDamage = 55;
        hasSideEffect = true;
        chanceOfSideEffect = 100;
        moveDescription = "Heals you're pocketmonster for 50% of the damage dealt. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, 
        InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        if (ownPocketMonster.health < ownPocketMonster.stats.maxHealth && !ownPocketMonster.fainted)
        {
            float restoredHealth = damageDone * 0.5f;
            if (restoredHealth > 0)
            {
                ownPocketMonster.health += restoredHealth;
                if (ownPocketMonster.health > ownPocketMonster.stats.maxHealth)
                {
                    restoredHealth -= ownPocketMonster.health - ownPocketMonster.stats.maxHealth;
                    ownPocketMonster.health = ownPocketMonster.stats.maxHealth;
                }
                string message = ownPocketMonster.stats.name + " restored " + (int)restoredHealth + " health.";
                if (player.pocketMonsters.Contains(ownPocketMonster))
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
