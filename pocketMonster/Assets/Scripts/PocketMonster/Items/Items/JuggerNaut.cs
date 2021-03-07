using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggerNaut : PocketMonsterItem
{
    public override void SetStats()
    {
        onSwitch = true;
        name = "Juggernaut";
        itemDescription = "On switch in drop attacking stats by one stage but increase defensive stats by one stage.";
        itemSort = ItemSort.Defense;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        effectedPocketMonster.stats.attack.GetStatChanges(-1);
        effectedPocketMonster.stats.defense.GetStatChanges(1);
        effectedPocketMonster.stats.specialAttack.GetStatChanges(-1);
        effectedPocketMonster.stats.specialDefense.GetStatChanges(1);
        inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " dropped it's attacking stats by one stage and raised it's " +
            "defensive stats by one stage from it's " + name + ".", false, false, false, false);
    }

    public override float CalculateOffensiveStatChanges(float offensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        offensiveStat = offensiveStat / 1.5f;
        return offensiveStat;
    }

    public override float CalculateDefensiveStatChanges(float defensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        defensiveStat += boostableStat.baseStat * 0.5f;
        return defensiveStat;
    }

    public override int CalcaluteDifferenceInBoostAmount(int boostAmount, BoostableStat boostableStat)
    {
        if (boostableStat.typeStat == BoostableStat.TypeStat.DefensivePhysical || boostableStat.typeStat == BoostableStat.TypeStat.DefensiveSpecial)
        {
            boostAmount++;
        } else
        {
            boostAmount--;
        }
        return boostAmount;
    }
}
