using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainerStats
{
    public float intelligence;

    public List<PocketMonster> pocketMonsters = new List<PocketMonster>();

    public List<PocketMonsterItem> teamBuffs = new List<PocketMonsterItem>(); 
}
