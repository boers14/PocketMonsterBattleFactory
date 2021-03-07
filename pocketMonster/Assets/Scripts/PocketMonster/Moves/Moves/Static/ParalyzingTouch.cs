using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalyzingTouch : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Paralyzing touch";
        moveType = PocketMonsterStats.Typing.Static;
        moveSort = MoveSort.Physical;
        baseDamage = 55;
        hasSideEffect = true;
        chanceOfSideEffect = 30;
        moveDescription = "Has a 30% chance to paralyze the opposing pocketmonster. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        if (opponentPocketMonster.currentStatus == PocketMonster.StatusEffects.None && opponentPocketMonster.health > 0)
        {
            opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.Paralyzed;
            opponentPocketMonster.RecalculateStatsAfterStatus();
            inBattleTextManager.QueMessage(opponentPocketMonster.stats.name + " got paralyzed from the attack.", false, false, false, false);
        }
    }
}
