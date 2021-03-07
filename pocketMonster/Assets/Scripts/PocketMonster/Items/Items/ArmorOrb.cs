using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorOrb : PocketMonsterItem
{
    private float amplifier = 1;
    private int amplifiedCounter = 0;

    public override void SetStats()
    {
        onSwitch = true;
        defenseTurn = true;
        everyTurn = true;
        name = "Armor orb";
        itemDescription = "Reduces the damage taken every turn. This effects stacks till a max of a 33% damage reduction. The amplified reduction" +
            " resets on switch.";
        itemSort = ItemSort.Defense;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        amplifier = 1;
        amplifiedCounter = 0;
    }

    public override void GrantAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        if (move.moveSort != PocketMonsterMoves.MoveSort.Status)
        {
            if (amplifier > 1 && effectedPocketMonster.amountOfDamageTaken > 0)
            {
                effectedPocketMonster.amountOfDamageTaken /= amplifier;
                inBattleTextManager.QueMessage("The damage taken of " + move.moveName + " got reduced by the " + name + " of " +
                    effectedPocketMonster.stats.name + ".", false, false, false, false);
            }
        }
    }

    public override void GrantEveryTurnEffect(PocketMonster effectedPocketMonster, InBattleTextManager inBattleTextManager)
    {
        if (amplifiedCounter < 10)
        {
            amplifiedCounter++;
            amplifier += 0.05f;
            inBattleTextManager.QueMessage("The " + name + " of " + effectedPocketMonster.stats.name + " gained more power."
                , false, false, false, false);
        }
    }

    public override float CalculateDamageForAi(float damageDone, PocketMonster pocketMonster, float damageMultiplier, PocketMonsterMoves move, int index)
    {
        damageDone /= amplifier;
        return damageDone;
    }
}
