using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedTouch : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Cursed Touch";
        moveType = PocketMonsterStats.Typing.Spooky;
        moveSort = MoveSort.Physical;
        baseDamage = 55;
        hasSideEffect = true;
        chanceOfSideEffect = 50;
        moveDescription = "Has a 50% chance to curse the opposing pocketmonster. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager
        , PlayerBattle player)
    {
        if (opponentPocketMonster.currentStatus == PocketMonster.StatusEffects.None && opponentPocketMonster.health > 0)
        {
            opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.Cursed;
            inBattleTextManager.QueMessage(opponentPocketMonster.stats.name + " got cursed from the attack.", false, false, false, false);
        }
    }
}
