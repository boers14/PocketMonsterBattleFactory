using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Struggle : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = Mathf.Infinity;
        moveName = "Struggle";
        moveType = PocketMonsterStats.Typing.None;
        moveSort = MoveSort.Physical;
        baseDamage = 30;
        hasSideEffect = true;
        chanceOfSideEffect = 100;
        moveDescription = "A struggle to do something dealing " + baseDamage + " base damage dealing 25% of max health to it's self. Physical damage with no typing. " + accuracy + "% accurate.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        ownPocketMonster.health -= ownPocketMonster.stats.maxHealth / 4;
        string message = ownPocketMonster.stats.name + " lost 25% health";
        if (player.pocketMonsters.Contains(ownPocketMonster))
        {
            inBattleTextManager.QueMessage(message, false, true, true, false);
        } else
        {
            inBattleTextManager.QueMessage(message, true, false, false, true);
        }
    }
}
