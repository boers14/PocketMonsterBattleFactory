using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTheSkies : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "To the skies";
        moveType = PocketMonsterStats.Typing.Wind;
        moveSort = MoveSort.Special;
        baseDamage = 55;
        hasSideEffect = true;
        chanceOfSideEffect = 30;
        moveDescription = "Has a 30% chance to set the opposing pocketmonster airborne. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        if (opponentPocketMonster.currentStatus == PocketMonster.StatusEffects.None && opponentPocketMonster.health > 0)
        {
            opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.Airborne;
            inBattleTextManager.QueMessage(opponentPocketMonster.stats.name + " got airborne from the attack.", false, false, false, false);
        }
    }
}
