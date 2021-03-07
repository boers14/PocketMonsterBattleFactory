using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PocketMonsterStats
{
    public float maxHealth, critChance, baseAttack, baseDefense, baseSpecialAttack, baseSpecialDefense, baseSpeed, originalCritChance;

    public BoostableStat attack = new BoostableStat(), defense = new BoostableStat(), specialAttack = new BoostableStat(),
        specialDefense = new BoostableStat(), speed = new BoostableStat();

    public List<BoostableStat> boostableStats = new List<BoostableStat>();
    public List<float> baseStats = new List<float>();

    public string name;

    public enum Typing
    {
        Fire,
        Grass,
        Water,
        Wind,
        Earth,
        Light,
        Spooky,
        Chemical,
        Static,
        Regular,
        None
    }

    public List<Typing> typing = new List<Typing>();

    public void FillBoostableStatsList()
    {
        attack.typeStat = BoostableStat.TypeStat.OffensivePhysical;
        defense.typeStat = BoostableStat.TypeStat.DefensivePhysical;
        specialAttack.typeStat = BoostableStat.TypeStat.OffensiveSpecial;
        specialDefense.typeStat = BoostableStat.TypeStat.DefensiveSpecial;

        BoostableStat[] boostables = { attack, defense, specialAttack, specialDefense, speed };
        boostableStats.AddRange(boostables);

        float[] bases = { baseAttack, baseDefense, baseSpecialAttack, baseSpecialDefense, baseSpeed };
        baseStats.AddRange(bases);
    }
}
