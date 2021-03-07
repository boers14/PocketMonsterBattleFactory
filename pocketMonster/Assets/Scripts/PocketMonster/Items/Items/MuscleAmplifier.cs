using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleAmplifier : PocketMonsterItem
{
    private float amplifier = 1;
    private int amplifiedCounter = 0;

    public override void SetStats()
    {
        onSwitch = true;
        attackTurn = true;
        everyTurn = true;
        name = "Muscle amplifier";
        itemDescription = "Boosts the damage done by physical moves by 5% every turn. This effects stacks till a max of a 50% boost. The amplified power" +
            " resets on switch.";
        itemSort = ItemSort.PhysicalOffense;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        amplifier = 1;
        amplifiedCounter = 0;
    }

    public override void GrantAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        if (move.moveSort == PocketMonsterMoves.MoveSort.Physical)
        {
            if (amplifier > 1 && opponentPocketMonster.amountOfDamageTaken > 0)
            {
                opponentPocketMonster.amountOfDamageTaken *= amplifier;
                inBattleTextManager.QueMessage("The power of " + move.moveName + " got amplified by the " + name + " of " +
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
        if (move.moveSort == PocketMonsterMoves.MoveSort.Physical)
        {
            damageDone *= amplifier;
        }

        return damageDone;
    }
}
