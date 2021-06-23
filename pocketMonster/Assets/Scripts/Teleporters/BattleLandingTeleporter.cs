using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLandingTeleporter : LandingTeleporter
{
    public BattleAlleyTeleporter connectedTeleporter = null;

    private TerrainManager terrainManager;
     
    public void SetTeleporterData(SpawnPosition spawnPosOfteleporter)
    {
        spawnPosition = spawnPosOfteleporter;

        List<LandingTeleporter> landingTeleporters = terrainManager.DecideNextSpawnOfMap(spawnPosition);

        terrainManager.spawnPosition = spawnPosition;

        for (int i = 0; i < landingTeleporters.Count; i++)
        {
            if (landingTeleporters[i].spawnPosition == spawnPosition)
            {
                connectedTeleporter.connectedSpawnPlace = landingTeleporters[i].gameObject.transform;
            }
        } 
    }

    public void SetTerrainManager(TerrainManager terrainManager)
    {
        this.terrainManager = terrainManager;
    }

    public void SetConnectedTeleporter(BattleAlleyTeleporter connectedTeleporter)
    {
        this.connectedTeleporter = connectedTeleporter;
    }
}
