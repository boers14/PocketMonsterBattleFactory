using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrownShot : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Drown shot";
        moveType = PocketMonsterStats.Typing.Water;
        moveSort = MoveSort.Special;
        baseDamage = 55;
        hasSideEffect = true;
        chanceOfSideEffect = 50;
        moveDescription = "Has a 50% chance to bloat the opposing pocketmonster. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        if (opponentPocketMonster.currentStatus == PocketMonster.StatusEffects.None && opponentPocketMonster.health > 0)
        {
            opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.Bloated;
            inBattleTextManager.QueMessage(opponentPocketMonster.stats.name + " got bloated from the attack.", false, false, false, false);
        }
    }
}
