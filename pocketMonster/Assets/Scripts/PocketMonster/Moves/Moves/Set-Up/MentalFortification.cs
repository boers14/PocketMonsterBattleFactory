using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MentalFortification : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Mental fortification";
        moveType = PocketMonsterStats.Typing.Regular;
        moveSort = MoveSort.Status;
        typeOfBoost = TypeOfBoostMove.SpecialDefensive;
        hasSideEffect = true;
        chanceOfSideEffect = 100;
        moveDescription = "Raise special defense by 2 stages. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        GainStatBoost(ownPocketMonster, ownPocketMonster.stats.specialDefense, inBattleTextManager, "special defense", 2);
    }
}
