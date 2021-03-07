using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPyramid : PocketMonsterItem
{
    private int counter = 0;
    private float initialDamageDone = 0;
    private bool calculateInItem = true;

    public override void SetStats()
    {
        attackTurn = true;
        onSwitch = true;
        name = "Power pyramid";
        itemDescription = "Every third attack is used twice. Counter resets on switch.";
        itemSort = ItemSort.General;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        counter = 0;
    }

    public override void GrantAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move,
        PocketMonster opponentPocketmonster, InBattleTextManager inBattleTextManager)
    {
        if (calculateInItem)
        {
            counter++;
            if (counter == 3)
            {
                calculateInItem = false;
                endOfAttackTurn = true;
                counter = 0;
            }
        } else
        {
            calculateInItem = true;
        }
    }

    public override void GrantEndOfAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move,
    PocketMonster opponentPocketmonster, InBattleTextManager inBattleTextManager)
    {
        endOfAttackTurn = false;
        if (!opponentPocketmonster.fainted)
        {
            GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
            PlayerBattle playerBattle = player.GetComponent<PlayerBattle>();

            inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " used the move " + move.moveName + " twice due to the " + name + "."
                , false, false, false, false);

            if (playerBattle.pocketMonsters.Contains(effectedPocketMonster))
            {
                effectedPocketMonster.DealDamage(move, opponentPocketmonster, playerBattle, false);
            }
            else
            {
                effectedPocketMonster.DealDamage(move, opponentPocketmonster, playerBattle, true);
            }
        }
    }

    public override float CalculateDamageForAi(float damageDone, PocketMonster pocketMonster, float damageMultiplier, PocketMonsterMoves move, int index)
    {
        if (index == 0)
        {
            initialDamageDone = damageDone;
        }

        if (counter + 1 == 3)
        {
            damageDone += initialDamageDone;
        }

        return damageDone;
    }
}
