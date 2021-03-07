using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostableStat
{
    public float actualStat = 0, baseStat = 0;

    public enum BoostAmount
    {
        XNegative6,
        XNegative5,
        XNegative4,
        XNegative3,
        XNegative2,
        XNegative1,
        X0,
        X1,
        X2,
        X3,
        X4,
        X5,
        X6
    }

    public enum TypeStat
    {
        OffensivePhysical,
        DefensivePhysical,
        OffensiveSpecial,
        DefensiveSpecial,
        None
    }

    public BoostAmount boostAmount = BoostAmount.X0;
    public TypeStat typeStat = TypeStat.None;

    public float GetStatChanges(int statChange)
    {
        boostAmount += statChange;
        if (boostAmount > BoostAmount.X6)
        {
            boostAmount = BoostAmount.X6;
        }

        if (boostAmount < BoostAmount.XNegative6)
        {
            boostAmount = BoostAmount.XNegative6;
        }

        switch(boostAmount)
        {
            case BoostAmount.X6:
                actualStat = baseStat * 4;
                break;
            case BoostAmount.X5:
                actualStat = baseStat * 3.5f;
                break;
            case BoostAmount.X4:
                actualStat = baseStat * 3;
                break;
            case BoostAmount.X3:
                actualStat = baseStat * 2.5f;
                break;
            case BoostAmount.X2:
                actualStat = baseStat * 2;
                break;
            case BoostAmount.X1:
                actualStat = baseStat * 1.5f;
                break;
            case BoostAmount.X0:
                actualStat = baseStat * 1;
                break;
            case BoostAmount.XNegative1:
                actualStat = baseStat / 1.5f;
                break;
            case BoostAmount.XNegative2:
                actualStat = baseStat / 2f;
                break;
            case BoostAmount.XNegative3:
                actualStat = baseStat / 2.5f;
                break;
            case BoostAmount.XNegative4:
                actualStat = baseStat / 3f;
                break;
            case BoostAmount.XNegative5:
                actualStat = baseStat / 3.5f;
                break;
            case BoostAmount.XNegative6:
                actualStat = baseStat / 4f;
                break;
            default:
                Debug.Log("outside of stat range");
                break;
        }

        return actualStat;
    }

    public void ResetStats()
    {
        boostAmount = BoostAmount.X0;
        GetStatChanges(0);
    }

    public void SetBaseStat(float stat)
    {
        actualStat = stat;
        baseStat = stat;
    }
}
