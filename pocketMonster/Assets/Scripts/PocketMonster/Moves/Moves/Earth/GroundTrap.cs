using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrap : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Ground Trap";
        moveType = PocketMonsterStats.Typing.Earth;
        moveSort = MoveSort.Physical;
        baseDamage = 55;
        hasSideEffect = true;
        chanceOfSideEffect = 50;
        moveDescription = "Has a 50% chance to trap the opposing pocketmonster. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        if (opponentPocketMonster.currentStatus == PocketMonster.StatusEffects.None && opponentPocketMonster.health > 0)
        {
            opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.Trapped;
            inBattleTextManager.QueMessage(opponentPocketMonster.stats.name + " got trapped from the attack.", false, false, false, false);
        }
    }
}
