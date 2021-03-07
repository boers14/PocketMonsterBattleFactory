using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketMonsterMoves
{
    public string moveName, moveDescription;
    public float baseDamage, accuracy, chanceOfSideEffect, powerPoints, currentPowerPoints;

    public bool hasSideEffect;

    public PocketMonsterStats.Typing moveType;

    public enum MoveSort
    {
        Physical,
        Special,
        Status
    }

    public enum TypeOfBoostMove
    {
        PhysicalOffensive,
        SpecialOffensive,
        Speed,
        PhysicalDefensive,
        SpecialDefensive,
        GeneralOffense,
        GeneralDefense,
        Recovery
    }

    public MoveSort moveSort;
    public TypeOfBoostMove typeOfBoost;

    public virtual void SetMoveStats()
    {

    }

    public virtual void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager,
        PlayerBattle player)
    {

    }

    public void GainStatBoost(PocketMonster ownPocketMonster, BoostableStat boostedStat, InBattleTextManager inBattleTextManager, string boostedStatText,
        int raiseAmount)
    {
        if (boostedStat.boostAmount < BoostableStat.BoostAmount.X6)
        {
            int currenBoostAmount = (int)boostedStat.boostAmount;
            string message = ownPocketMonster.stats.name + " used " + moveName + ". ";
            boostedStat.GetStatChanges(raiseAmount);
            currenBoostAmount = (int)boostedStat.boostAmount - currenBoostAmount;

            if (currenBoostAmount > 1)
            {
                message += ownPocketMonster.stats.name + " raised it's " + boostedStatText + " by " + currenBoostAmount + " stages.";
            }
            else
            {
                message += ownPocketMonster.stats.name + " raised it's " + boostedStatText + " by " + currenBoostAmount + " stage.";
            }

            inBattleTextManager.QueMessage(message, false, false, false, false);
        }
        else
        {
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " failed to raise it's " + boostedStatText + ".", false, false, false, false);
        }
    }
}
