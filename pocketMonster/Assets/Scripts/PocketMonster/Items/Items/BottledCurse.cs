using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottledCurse : PocketMonsterItem
{
    private PlayerBattle playerBattle;

    public override void SetStats()
    {
        onSwitch = true;
        attackTurn = true;
        name = "Bottled curse";
        itemDescription = "On switch in give the cursed status to the pocketmonster, but non attacking moves are used twice if the pocketmonster is cursed.";
        itemSort = ItemSort.None;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        if (effectedPocketMonster.currentStatus == PocketMonster.StatusEffects.None)
        {
            effectedPocketMonster.currentStatus = PocketMonster.StatusEffects.Cursed;
            inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " became cursed due to it's " + name + ".", false, false, false, false);
        }
        playerBattle = player;
    }

    public override void GrantAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        if (move.moveSort == PocketMonsterMoves.MoveSort.Status && effectedPocketMonster.currentStatus == PocketMonster.StatusEffects.Cursed &&
            move.currentPowerPoints > 0)
        {
            move.currentPowerPoints -= 3;
            move.GrantSideEffect(effectedPocketMonster, opponentPocketMonster, effectedPocketMonster.amountOfDamageTaken, inBattleTextManager, playerBattle);
            inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " used " + move.moveName + " again due to the " + name + "."
                , false, false, false, false);
        }
    }
}
