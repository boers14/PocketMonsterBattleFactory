using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeechBlast : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Leech blast";
        moveType = PocketMonsterStats.Typing.Grass;
        moveSort = MoveSort.Special;
        baseDamage = 55;
        hasSideEffect = true;
        chanceOfSideEffect = 30;
        moveDescription = "Has a 30% chance to leech the opposing pocketmonster. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        if (opponentPocketMonster.currentStatus == PocketMonster.StatusEffects.None && opponentPocketMonster.health > 0)
        {
            opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.Leeched;
            inBattleTextManager.QueMessage(opponentPocketMonster.stats.name + " got leeched from the attack.", false, false, false, false);
        }
    }
}
