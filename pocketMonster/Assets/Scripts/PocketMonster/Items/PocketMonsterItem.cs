using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketMonsterItem
{
    public bool onSwitch = false, everyTurn = false, attackTurn = false, defenseTurn = false, endOfAttackTurn = false, endOfDamageCalc = false;

    public string name = "", itemDescription = "", startOfBattleMessage = "";

    public enum ItemSort
    {
        PhysicalOffense,
        SpecialOffense,
        GeneralOffense,
        Defense,
        General,
        None
    }

    public ItemSort itemSort = ItemSort.None;

    public virtual void SetStats()
    {

    }

    public virtual void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {

    }

    public virtual void GrantEveryTurnEffect(PocketMonster effectedPocketMonster, InBattleTextManager inBattleTextManager)
    {

    }

    public virtual void GrantAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move,
        PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {

    }

    public virtual void GrantEndOfDamageCalcEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move,
    PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, bool defenseTurn)
    {

    }

    public virtual void GrantEndOfAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move,
    PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {

    }

    public virtual float CalculateDamageForAi(float damageDone, PocketMonster pocketMonster, float damageMultiplier, PocketMonsterMoves move, int index)
    {
        return damageDone;
    }

    public virtual float CalculateOffensiveStatChanges(float offensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        return offensiveStat;
    }

    public virtual float CalculateDefensiveStatChanges(float defensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        return defensiveStat;
    }

    public virtual float CalculateSpeedStatChanges(float speedStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        return speedStat;
    }

    public virtual int CalcaluteDifferenceInBoostAmount(int boostAmount, BoostableStat boostableStat)
    {
        return boostAmount;
    }
}
