using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineTeamCreator : EnemyManager
{
    private void Start()
    {
        teamsCreated = 15;
        amountOfItems = 0;
        amountOfMoves = 4;
        amountOfPocketMonsters = 4;
        amountOfTeamBuffs = 0;
    }

    public override List<PocketMonster> createTeamForAi(TrainerAi trainerAi, bool filterStatusMoves)
    {
        return base.createTeamForAi(trainerAi, filterStatusMoves);
    }

    public override void CalculateTeamStats()
    {
        return;
    }
}
