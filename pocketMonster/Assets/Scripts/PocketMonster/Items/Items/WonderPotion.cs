using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WonderPotion : PocketMonsterItem
{
    private int turnCounter = 0;

    public override void SetStats()
    {
        onSwitch = true;
        defenseTurn = true;
        everyTurn = true;
        name = "Wonder Potion";
        itemDescription = "Set the max health of the pocketmonster to 1, but become immune to non super effective moves. Become cursed on the 3rd turn, " +
            "overwrites other statusses. The counter resets on switch. Secondary effects of moves still occur.";
        itemSort = ItemSort.GeneralOffense;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        turnCounter = 0;
        if (effectedPocketMonster.stats.maxHealth != 1)
        {
            effectedPocketMonster.stats.maxHealth = 1;
            effectedPocketMonster.RecalculateHealth();
            inBattleTextManager.QueMessage("The max health of " + effectedPocketMonster.stats.name + " got set to 1."
                , false, false, false, false);
            base.GrantOnSwitchInEffect(effectedPocketMonster, opponentPocketMonster, inBattleTextManager, player);
        }
    }

    public override void GrantAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        float damageMultiplier = effectedPocketMonster.CalculateInTyping(move.moveType);

        if (damageMultiplier <= 1 && move.moveSort != PocketMonsterMoves.MoveSort.Status && effectedPocketMonster.amountOfDamageTaken > 0)
        {
            effectedPocketMonster.amountOfDamageTaken = 0;
            inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " blocked all incoming damage from the move " + move.moveName + "."
                , false, false, false, false);
        }
    }

    public override void GrantEveryTurnEffect(PocketMonster effectedPocketMonster, InBattleTextManager inBattleTextManager)
    {
        turnCounter++;

        if (turnCounter >= 3)
        {
            effectedPocketMonster.currentStatus = PocketMonster.StatusEffects.Cursed;
            inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " became cursed."
                , false, false, false, false);
        } else
        {
            string message = "";
            int turnsLeft = 3 - turnCounter;

            if (turnsLeft > 1)
            {
                message = turnsLeft + " turns";
            } else
            {
                message = turnsLeft + " turn";
            }

            inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " has " + message + " left till the curse from the " +
                name + ".", false, false, false, false);
        }
    }

    public override float CalculateDamageForAi(float damageDone, PocketMonster pocketMonster, float damageMultiplier, PocketMonsterMoves move, int index)
    {
        if (damageMultiplier <= 1)
        {
            if (move.hasSideEffect)
            {
                damageDone = 0.1f;
            }
            else
            {
                damageDone = 0;
            }
        }

        return damageDone;
    }
}
