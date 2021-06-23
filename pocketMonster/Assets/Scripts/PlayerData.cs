using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // Player
    public float[] playerPos;

    // Game manager
    public List<int> playerPocketMonsters, playerPocketMonstersAbilitys, teamBuffsOfPlayer, allMovesHandedOut, itemsToHandOut, movesToHandOut,
        currentItemsHandedOut, currentMovesHandedOut;
    public List<List<int>> playerPocketMonsterMoves, playerPocketMonsterItems;
    public List<float[]> mergantPosses, trainerPosses;
    public int lives, itemHandOutIndex, currentPickUp;
    public List<string> pickups;
    public string nextBigPickUp, currentMergant;
    public bool lastBattle = false;

    // Enemy manager
    public int teamsCreated, amountOfPocketMonsters, amountOfMoves, amountOfItems, currentIntelligence, amountOfTeamBuffs;
    public bool addMove = false;

    // Terrain manager
    public int lengthOfRun, currentLenght, currentPlaceInGuantlet;
    public string whatToSpawn, currentMapChunk;
    public string path, teleporterSpawnPos;
    public float[] nextChunkPos;
    public List<float[]> battleSpotsPosses, currentTerrainPiecesPosses;

    public PlayerData(GameObject player, GameManager gameManager, EnemyManager enemyManager, TerrainManager terrainManager)
    {
        // Player
        playerPos = new float[3];
        playerPos[0] = player.transform.position.x;
        playerPos[1] = player.transform.position.y;
        playerPos[2] = player.transform.position.z;

        // Game manager
        lives = gameManager.lives;
        itemHandOutIndex = gameManager.itemHandOutIndex;
        currentPickUp = gameManager.currentPickUp;
        pickups = new List<string>();
        for (int i = 0; i < gameManager.pickups.Count; i++)
        {
            pickups.Add(gameManager.pickups[i].ToString());
        }
        nextBigPickUp = gameManager.nextBigPickUp.ToString();
        lastBattle = gameManager.lastBattle;

        playerPocketMonsters = CreateIntList(gameManager.playerPocketMonsterInInt, playerPocketMonsters);
        playerPocketMonstersAbilitys = CreateIntList(gameManager.playerPocketMonstersAbilityInInt, playerPocketMonstersAbilitys);
        playerPocketMonsterItems = CreateIntIntList(gameManager.playerPocketMonsterItemsInInt, playerPocketMonsterItems);
        playerPocketMonsterMoves = CreateIntIntList(gameManager.playerPocketMonsterMovesInInt, playerPocketMonsterMoves);
        teamBuffsOfPlayer = CreateIntList(gameManager.teamBuffsOfPlayerInInt, teamBuffsOfPlayer);
        allMovesHandedOut = CreateIntList(gameManager.allMovesHandedOutInInt, allMovesHandedOut);
        itemsToHandOut = CreateIntList(gameManager.itemsToHandOutInInt, itemsToHandOut);
        movesToHandOut = CreateIntList(gameManager.movesToHandOutInInt, movesToHandOut);
        currentItemsHandedOut = CreateIntList(gameManager.currentItemsHandedOutInInt, currentItemsHandedOut);
        currentMovesHandedOut = CreateIntList(gameManager.currentMovesHandedOutInInt, currentMovesHandedOut);

        trainerPosses = FillPosList(gameManager.trainers, trainerPosses);
        mergantPosses = FillPosList(gameManager.mergants, mergantPosses);
        currentMergant = gameManager.currentMergant.ToString();

        // Enemy manager
        teamsCreated = enemyManager.teamsCreated;
        amountOfPocketMonsters = enemyManager.amountOfPocketMonsters;
        amountOfMoves = enemyManager.amountOfMoves;
        amountOfItems = enemyManager.amountOfItems;
        currentIntelligence = enemyManager.currentIntelligence;
        amountOfTeamBuffs = enemyManager.amountOfTeamBuffs;
        addMove = enemyManager.addMove;

        // Terrain manager
        lengthOfRun = terrainManager.lengthOfRun;
        currentLenght = terrainManager.currentLenght;
        currentPlaceInGuantlet = terrainManager.currentPlaceInGuantlet;
        whatToSpawn = terrainManager.whatToSpawn.ToString();
        currentMapChunk = terrainManager.currentMapChunk.ToString();
        path = terrainManager.path.ToString();
        nextChunkPos = new float[3];
        nextChunkPos[0] = terrainManager.nextChunkPos.x;
        nextChunkPos[1] = terrainManager.nextChunkPos.y;
        nextChunkPos[2] = terrainManager.nextChunkPos.z;
        battleSpotsPosses = FillPosList(terrainManager.battleSpots, battleSpotsPosses);
        currentTerrainPiecesPosses = FillPosList(terrainManager.currentTerrainPieces, currentTerrainPiecesPosses);
        teleporterSpawnPos = terrainManager.spawnPosition.ToString();
    }

    private List<float[]> FillPosList(List<GameObject> neededPossesList, List<float[]> ownList)
    {
        ownList = new List<float[]>();

        for (int i = 0; i < neededPossesList.Count; i++)
        {
            float[] pos = new float[3];
            pos[0] = neededPossesList[i].transform.position.x;
            pos[1] = neededPossesList[i].transform.position.y;
            pos[2] = neededPossesList[i].transform.position.z;
            ownList.Add(pos);
        }

        return ownList;
    }

    private List<int> CreateIntList(List<int> gameManagerList, List<int> ownList)
    {
        ownList = new List<int>();
        for (int i = 0; i < gameManagerList.Count; i++)
        {
            ownList.Add(gameManagerList[i]);
        }
        return ownList;
    }

    private List<List<int>> CreateIntIntList(List<List<int>> gameManagerList, List<List<int>> ownList)
    {
        ownList = new List<List<int>>();
        for (int i = 0; i < gameManagerList.Count; i++)
        {
            ownList.Add(gameManagerList[i]);
        }
        return ownList;
    }
}
