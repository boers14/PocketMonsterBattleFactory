using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBooster : PocketMonsterItem
{
    private PlayerBattle playerBattle;

    public override void SetStats()
    {
        onSwitch = true;
        attackTurn = true;
        name = "Attack booster";
        itemDescription = "At start of battle increase attack by 2 stages, but take 50% recoil on attack.";
        itemSort = ItemSort.PhysicalOffense;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        effectedPocketMonster.stats.attack.GetStatChanges(2);
        inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " raised it's attack by two stages from it's " + name + ".", 
            false, false, false, false);
        playerBattle = player;
    }

    public override void GrantAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move,
        PocketMonster opponentPocketmonster, InBattleTextManager inBattleTextManager)
    {
        if (move.moveSort == PocketMonsterMoves.MoveSort.Physical)
        {
            float recoil = opponentPocketmonster.amountOfDamageTaken;

            if (recoil > 0)
            {
                if (recoil > opponentPocketmonster.health)
                {
                    recoil = opponentPocketmonster.health;
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
        if (boostableStat.typeStat == BoostableStat.TypeStat.OffensivePhysical)
        {
            offensiveStat += boostableStat.baseStat;
        }

        return offensiveStat;
    }

    public override int CalcaluteDifferenceInBoostAmount(int boostAmount, BoostableStat boostableStat)
    {
        if (boostableStat.typeStat == BoostableStat.TypeStat.OffensivePhysical)
        {
            boostAmount += 2;
        }

        return boostAmount;
    }
}
