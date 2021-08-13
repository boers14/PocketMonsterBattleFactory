using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttackBooster : PocketMonsterItem
{
    private PlayerBattle playerBattle;
    public override void SetStats()
    {
        onSwitch = true;
        endOfDamageCalc = true;
        name = "Special attack booster";
        itemDescription = "At start of battle increase special attack by 2 stages, but take 50% recoil on attack.";
        itemSort = ItemSort.SpecialOffense;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        effectedPocketMonster.stats.specialAttack.GetStatChanges(2);
        inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " raised it's special attack by two stages from it's " + name + "."
            , false, false, false, false);
        playerBattle = player;
    }

    public override void GrantEndOfDamageCalcEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move, PocketMonster opponentPocketMonster, 
        InBattleTextManager inBattleTextManager, bool defenseTurn)
    {
        if (!defenseTurn)
        {
            return;
        }

        if (move.moveSort == PocketMonsterMoves.MoveSort.Special && !effectedPocketMonster.fainted)
        {
            float recoil = opponentPocketMonster.amountOfDamageTaken;

            if (recoil > 0)
            {
                if (recoil > opponentPocketMonster.health)
                {
                    recoil = opponentPocketMonster.health;
                }

                effectedPocketMonster.health -= Mathf.Ceil(recoil * 0.5f);
                string message = effectedPocketMonster.stats.name + " got recoil damage from it's " + name + ".";
                if (playerBattle.pocketMonsters.Contains(effectedPocketMonster))
                {
                    inBattleTextManager.QueMessage(message, false, true, true, false);
                }
                else
                {
                    inBattleTextManager.QueMessage(message, true, false, false, true);
                }
            }
        }
    }

    public override float CalculateOffensiveStatChanges(float offensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        if (boostableStat.typeStat == BoostableStat.TypeStat.OffensiveSpecial)
        {
            offensiveStat += boostableStat.baseStat;
        }

        return offensiveStat;
    }

    public override int CalcaluteDifferenceInBoostAmount(int boostAmount, BoostableStat boostableStat)
    {
        if (boostableStat.typeStat == BoostableStat.TypeStat.OffensiveSpecial)
        {
            boostAmount += 2;
        }

        return boostAmount;
    }
}
