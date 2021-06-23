using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [SerializeField]
    private GameObject startSpawn = null, firstStop = null, midwayMiddleStop = null, bigStop = null, midwayStopOnSide = null, battleAlley = null,
        endStopSmall = null, endStopMiddle = null, battleArena = null;

    public int lengthOfRun = 6;

    public int currentLenght = 2, currentPlaceInGuantlet = 0;

    private GameManager gameManager;

    public LandingTeleporter.SpawnPosition spawnPosition = LandingTeleporter.SpawnPosition.Middle;

    public enum MapChunksToSpawn
    {
        StartOfMap,
        SecondPocketMonster,
        FirstMoveSelection,
        SecondMoveSelection,
        ThirdPocketMonster,
        StartOfGauntlet,
        MidGauntlet,
        EndOfGuantlet,
        BigItemRound,
        EndOfGame,
        LastBattle
    }

    public MapChunksToSpawn whatToSpawn = MapChunksToSpawn.StartOfMap, currentMapChunk = MapChunksToSpawn.StartOfMap;

    public LandingTeleporter.SpawnPosition path = LandingTeleporter.SpawnPosition.Middle;

    public Vector3 nextChunkPos = Vector3.zero;

    public Vector3 arenaPos = new Vector3(-50, 0, -50);

    public List<GameObject> currentTerrainPieces = new List<GameObject>(), battleSpots = new List<GameObject>();

    private void Start()
    {
        GameObject runSettingManager = GameObject.FindGameObjectWithTag("RunSettingsManager");
        if (runSettingManager != null)
        {
            lengthOfRun = runSettingManager.GetComponent<RunSettingsManager>().lenghtOfRun;
        } else
        {
            lengthOfRun = 6;
        }

        Destroy(runSettingManager);

        GameObject arena = Instantiate(battleArena);
        arena.transform.position = arenaPos;
    }

    public int GetAmountOfPickupsGenerated()
    {
        int amount = 0;

        for (int i = currentLenght; i <= lengthOfRun; i++)
        {
            amount += i;
        }

        return amount;
    }

    public void SpawnStartOfMap(bool createBattleAlley)
    {
        ClearTerrainPieces(true);
        GameObject startOfMap = Instantiate(startSpawn);
        startOfMap.transform.position = nextChunkPos;
        currentTerrainPieces.Add(startOfMap);
        if (createBattleAlley)
        {
            gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        }
        SetNextChunkPos(createBattleAlley);

        List<GameObject> teleporters = new List<GameObject>();
        GetAllTeleporters(startOfMap, teleporters, "LeaveTeleporter");

        if (createBattleAlley)
        {
            GameObject battleSpot = CreateBattleAlley(teleporters, true);
        }

        currentMapChunk = MapChunksToSpawn.StartOfMap;
        whatToSpawn = MapChunksToSpawn.SecondPocketMonster;
    }

    public List<LandingTeleporter> DecideNextSpawnOfMap(LandingTeleporter.SpawnPosition direction)
    {
        List<LandingTeleporter> landingTeleporters = new List<LandingTeleporter>();

        switch (whatToSpawn)
        {
            case MapChunksToSpawn.StartOfMap:
                SpawnStartOfMap(true);
                print("Reset?");
                break;
            case MapChunksToSpawn.SecondPocketMonster:
                landingTeleporters = CreateFirstStop(landingTeleporters, direction, true);
                break;
            case MapChunksToSpawn.FirstMoveSelection:
                landingTeleporters = CreateFirstMoveSelection(landingTeleporters, direction, true);
                break;
            case MapChunksToSpawn.SecondMoveSelection:
                landingTeleporters = CreateSecondMoveSelection(landingTeleporters, direction, true);
                break;
            case MapChunksToSpawn.ThirdPocketMonster:
                landingTeleporters = CreateThirdPocketMonsterSelection(landingTeleporters, direction, true);
                break;
            case MapChunksToSpawn.StartOfGauntlet:
                landingTeleporters = CreateStartOfGauntlet(landingTeleporters, direction, true);
                break;
            case MapChunksToSpawn.MidGauntlet:
                landingTeleporters = CreateMidGuantlet(landingTeleporters, direction, true);
                break;
            case MapChunksToSpawn.EndOfGuantlet:
                landingTeleporters = CreateEndGuantlet(landingTeleporters, direction, true);
                break;
            case MapChunksToSpawn.BigItemRound:
                landingTeleporters = CreateBigItemRound(landingTeleporters, direction, true);
                break;
            case MapChunksToSpawn.EndOfGame:
                landingTeleporters = CreateEndOfGame(landingTeleporters, direction, true);
                break;
            default:
                print("No map pieces found to spawn");
                break;
        }

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateFirstStop(List<LandingTeleporter> landingTeleporters, 
        LandingTeleporter.SpawnPosition direction, bool createBattleAlley)
    {
        ClearTerrainPieces(true);
        GameObject firstStopChunk = Instantiate(firstStop);
        firstStopChunk.transform.position = nextChunkPos;
        currentTerrainPieces.Add(firstStopChunk);
        if (createBattleAlley)
        {
            gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        }
        SetNextChunkPos(createBattleAlley);

        List<GameObject> teleporters = new List<GameObject>();
        GetAllTeleporters(firstStopChunk, teleporters, "LeaveTeleporter");

        if (createBattleAlley)
        {
            GameObject battleSpot = CreateBattleAlley(teleporters, true);
        }

        List<GameObject> arrivalTeleporters = new List<GameObject>();
        GetAllTeleporters(firstStopChunk, arrivalTeleporters, "LandingTeleporter");

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition = LandingTeleporter.SpawnPosition.Middle;
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        currentMapChunk = MapChunksToSpawn.SecondPocketMonster;
        whatToSpawn = MapChunksToSpawn.FirstMoveSelection;

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateFirstMoveSelection(List<LandingTeleporter> landingTeleporters,
        LandingTeleporter.SpawnPosition direction, bool createBattleAlley)
    {
        ClearTerrainPieces(true);
        List<GameObject> teleporters = new List<GameObject>();
        List<GameObject> arrivalTeleporters = new List<GameObject>();

        for (int i = 0; i < 2; i++)
        {
            GameObject firstStopChunk = Instantiate(firstStop);
            SetGauntlerChunkPosAndSetTeleporters(firstStopChunk, teleporters, arrivalTeleporters, i, createBattleAlley);
        }

        if (createBattleAlley)
        {
            GameObject battleSpot = CreateBattleAlley(teleporters, true);
        }

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            LandingTeleporter.SpawnPosition neededSpawnPos = LandingTeleporter.SpawnPosition.OutOfReach;

            if (arrivalTeleporters[i].GetComponent<LandingTeleporter>().chunkIndex == 0)
            {
                neededSpawnPos = LandingTeleporter.SpawnPosition.Left;
            }
            else
            {
                neededSpawnPos = LandingTeleporter.SpawnPosition.Right;
            }

            arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition = neededSpawnPos;
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        path = direction;

        currentMapChunk = MapChunksToSpawn.FirstMoveSelection;
        whatToSpawn = MapChunksToSpawn.SecondMoveSelection;

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateSecondMoveSelection(List<LandingTeleporter> landingTeleporters, 
        LandingTeleporter.SpawnPosition direction, bool createBattleAlley)
    {
        ClearTerrainPieces(true);
        List<GameObject> teleporters = new List<GameObject>();
        List<GameObject> arrivalTeleporters = new List<GameObject>();

        for (int i = 0; i < 2; i++)
        {
            GameObject firstStopChunk = Instantiate(endStopSmall);
            SetGauntlerChunkPosAndSetTeleporters(firstStopChunk, teleporters, arrivalTeleporters, i, createBattleAlley);
        }

        if (createBattleAlley)
        {
            GameObject battleSpot = CreateBattleAlley(teleporters, true);
        }

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            LandingTeleporter.SpawnPosition neededSpawnPos = SmallGauntletTeleporterPosDecision(direction,
                arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition, arrivalTeleporters[i].GetComponent<LandingTeleporter>().chunkIndex);
            arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition = neededSpawnPos;
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        path = direction;

        currentMapChunk = MapChunksToSpawn.SecondMoveSelection;
        whatToSpawn = MapChunksToSpawn.ThirdPocketMonster;

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateThirdPocketMonsterSelection(List<LandingTeleporter> landingTeleporters, 
        LandingTeleporter.SpawnPosition direction, bool createBattleAlley)
    {
        ClearTerrainPieces(true);
        GameObject thirdPocketMonster = Instantiate(midwayMiddleStop);
        thirdPocketMonster.transform.position = nextChunkPos;
        currentTerrainPieces.Add(thirdPocketMonster);
        if (createBattleAlley)
        {
            gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        }
        SetNextChunkPos(createBattleAlley);

        List<GameObject> teleporters = new List<GameObject>();
        GetAllTeleporters(thirdPocketMonster, teleporters, "LeaveTeleporter");

        if (createBattleAlley)
        {
            GameObject battleSpot = CreateBattleAlley(teleporters, true);
        }

        List<GameObject> arrivalTeleporters = new List<GameObject>();
        GetAllTeleporters(thirdPocketMonster, arrivalTeleporters, "LandingTeleporter");

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition = direction;
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        path = LandingTeleporter.SpawnPosition.Middle;

        currentMapChunk = MapChunksToSpawn.ThirdPocketMonster;
        whatToSpawn = MapChunksToSpawn.StartOfGauntlet;

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateStartOfGauntlet(List<LandingTeleporter> landingTeleporters, 
        LandingTeleporter.SpawnPosition direction, bool createBattleAlley)
    {
        ClearTerrainPieces(true);
        List<GameObject> teleporters = new List<GameObject>();
        List<GameObject> arrivalTeleporters = new List<GameObject>();

        for (int i = 0; i < 3; i++)
        {
            GameObject gauntletChunk = null;

            if (i == 1)
            {
                gauntletChunk = Instantiate(midwayMiddleStop);
            }
            else
            {
                gauntletChunk = Instantiate(firstStop);
            }

            SetGauntlerChunkPosAndSetTeleporters(gauntletChunk, teleporters, arrivalTeleporters, i, createBattleAlley);
        }

        gameManager.GoToNextPickUp();

        if (createBattleAlley)
        {
            GameObject battleSpot = CreateBattleAlley(teleporters, true);
        }

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            if (arrivalTeleporters[i].GetComponent<LandingTeleporter>().chunkIndex == 0)
            {
                arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition = LandingTeleporter.SpawnPosition.Left;
            }
            else if (arrivalTeleporters[i].GetComponent<LandingTeleporter>().chunkIndex == 1)
            {
                arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition = LandingTeleporter.SpawnPosition.Middle;
            }
            else
            {
                arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition = LandingTeleporter.SpawnPosition.Right;
            }
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        path = direction;

        currentPlaceInGuantlet++;

        currentMapChunk = MapChunksToSpawn.StartOfGauntlet;
        if (currentLenght - 1 == currentPlaceInGuantlet)
        {
            whatToSpawn = MapChunksToSpawn.EndOfGuantlet;
        }
        else
        {
            whatToSpawn = MapChunksToSpawn.MidGauntlet;
        }

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateMidGuantlet(List<LandingTeleporter> landingTeleporters, 
        LandingTeleporter.SpawnPosition direction, bool createBattleAlley)
    {
        ClearTerrainPieces(true);
        List<GameObject> teleporters = new List<GameObject>();
        List<GameObject> arrivalTeleporters = new List<GameObject>();

        for (int i = 0; i < 3; i++)
        {
            GameObject gauntletChunk = null;

            if (i == 1)
            {
                gauntletChunk = Instantiate(bigStop);
            }
            else
            {
                gauntletChunk = Instantiate(midwayStopOnSide);
            }

            SetGauntlerChunkPosAndSetTeleporters(gauntletChunk, teleporters, arrivalTeleporters, i, createBattleAlley);
        }

        gameManager.GoToNextPickUp();

        if (createBattleAlley)
        {
            GameObject battleSpot = CreateBattleAlley(teleporters, true);
        }

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            LandingTeleporter.SpawnPosition neededSpawnPos = GauntletTeleporterPosDecision(direction,
                arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition, arrivalTeleporters[i].GetComponent<LandingTeleporter>().chunkIndex);
            arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition = neededSpawnPos;
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        LandingTeleporter.SpawnPosition neededDirection = DecideDirection(direction);
        path = neededDirection;

        currentPlaceInGuantlet++;

        currentMapChunk = MapChunksToSpawn.MidGauntlet;
        if (currentLenght - 1 == currentPlaceInGuantlet)
        {
            whatToSpawn = MapChunksToSpawn.EndOfGuantlet;
        }
        else
        {
            whatToSpawn = MapChunksToSpawn.MidGauntlet;
        }

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateEndGuantlet(List<LandingTeleporter> landingTeleporters, 
        LandingTeleporter.SpawnPosition direction, bool createBattleAlley)
    {
        ClearTerrainPieces(true);
        List<GameObject> teleporters = new List<GameObject>();
        List<GameObject> arrivalTeleporters = new List<GameObject>();

        for (int i = 0; i < 3; i++)
        {
            GameObject gauntletChunk = null;

            if (i == 1)
            {
                gauntletChunk = Instantiate(endStopMiddle);
            }
            else
            {
                gauntletChunk = Instantiate(endStopSmall);
            }

            SetGauntlerChunkPosAndSetTeleporters(gauntletChunk, teleporters, arrivalTeleporters, i, createBattleAlley);
        }

        gameManager.GoToNextPickUp();

        if (createBattleAlley)
        {
            GameObject battleSpot = CreateBattleAlley(teleporters, true);
        }

        for (int i = 0; i < teleporters.Count; i++)
        {
            if (i == 0)
            {
                teleporters[i].GetComponent<Teleporter>().spawnPosition = LandingTeleporter.SpawnPosition.Left;
            }
            else if (i == 1)
            {
                teleporters[i].GetComponent<Teleporter>().spawnPosition = LandingTeleporter.SpawnPosition.Middle;
            }
            else if (i == 2)
            {
                teleporters[i].GetComponent<Teleporter>().spawnPosition = LandingTeleporter.SpawnPosition.Right;
            }
        }

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            LandingTeleporter.SpawnPosition neededSpawnPos = GauntletTeleporterPosDecision(direction,
                arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition, arrivalTeleporters[i].GetComponent<LandingTeleporter>().chunkIndex);
            arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition = neededSpawnPos;
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        LandingTeleporter.SpawnPosition neededDirection = DecideDirection(direction);
        path = neededDirection;
        currentPlaceInGuantlet++;

        currentMapChunk = MapChunksToSpawn.EndOfGuantlet;
        if (currentLenght + 1 >= lengthOfRun)
        {
            whatToSpawn = MapChunksToSpawn.EndOfGame;
        }
        else
        {
            whatToSpawn = MapChunksToSpawn.BigItemRound;
        }

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateBigItemRound(List<LandingTeleporter> landingTeleporters, 
        LandingTeleporter.SpawnPosition direction, bool createBattleAlley)
    {
        ClearTerrainPieces(true);
        GameObject bigItemRound = Instantiate(bigStop);
        bigItemRound.transform.position = nextChunkPos;
        currentTerrainPieces.Add(bigItemRound);
        if (createBattleAlley)
        {
            gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        }
        SetNextChunkPos(createBattleAlley);

        List<GameObject> teleporters = new List<GameObject>();
        GetAllTeleporters(bigItemRound, teleporters, "LeaveTeleporter");

        if (createBattleAlley)
        {
            GameObject battleSpot = CreateBattleAlley(teleporters, true);
        }

        List<GameObject> arrivalTeleporters = new List<GameObject>();
        GetAllTeleporters(bigItemRound, arrivalTeleporters, "LandingTeleporter");

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        path = LandingTeleporter.SpawnPosition.Middle;

        currentMapChunk = MapChunksToSpawn.BigItemRound;
        whatToSpawn = MapChunksToSpawn.StartOfGauntlet;

        currentLenght++;
        currentPlaceInGuantlet = 0;

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateEndOfGame(List<LandingTeleporter> landingTeleporters, 
        LandingTeleporter.SpawnPosition direction, bool createBattleAlley)
    {
        ClearTerrainPieces(true);
        GameObject lastItemRound = Instantiate(endStopMiddle);
        lastItemRound.transform.position = nextChunkPos;
        currentTerrainPieces.Add(lastItemRound);
        if (createBattleAlley)
        {
            gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        }
        SetNextChunkPos(createBattleAlley);

        List<GameObject> teleporters = new List<GameObject>();
        GetAllTeleporters(lastItemRound, teleporters, "LeaveTeleporter");

        List<GameObject> arrivalTeleporters = new List<GameObject>();
        GetAllTeleporters(lastItemRound, arrivalTeleporters, "LandingTeleporter");

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        currentTerrainPieces.Add(lastItemRound);
        ClearTerrainPieces(false);
        GameObject battleSpot = Instantiate(startSpawn);
        battleSpot.transform.position = nextChunkPos;
        currentTerrainPieces.Add(battleSpot);
        if (createBattleAlley)
        {
            gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        }
        battleSpot.transform.eulerAngles = new Vector3(0, 180, 0);
        List<GameObject> battleTeleporters = new List<GameObject>();
        GetAllTeleporters(battleSpot, battleTeleporters, "LeaveTeleporter");
        battleTeleporters[0].GetComponent<Teleporter>().teleport = false;
        battleTeleporters[0].GetComponent<Teleporter>().SetGameManager(gameManager);

        for (int i = 0; i < teleporters.Count; i++)
        {
            teleporters[i].GetComponent<Teleporter>().SetGameManager(gameManager);
            teleporters[i].GetComponent<Teleporter>().setData = false;
            teleporters[i].GetComponent<Teleporter>().connectedSpawnPlace = battleTeleporters[0].transform;
            teleporters[i].GetComponent<Teleporter>().lastTeleporter = true;
        }

        path = LandingTeleporter.SpawnPosition.Middle;
        currentMapChunk = MapChunksToSpawn.EndOfGame;
        whatToSpawn = MapChunksToSpawn.LastBattle;

        return landingTeleporters;
    }

    private void SetGauntlerChunkPosAndSetTeleporters(GameObject gauntletChunk, List<GameObject> teleporters, 
        List<GameObject> arrivalTeleporters, int index, bool createBattleAlley)
    {
        gauntletChunk.transform.position = nextChunkPos;
        currentTerrainPieces.Add(gauntletChunk);
        if (createBattleAlley)
        {
            gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        }
        SetNextChunkPos(createBattleAlley);

        GetAllTeleporters(gauntletChunk, teleporters, "LeaveTeleporter");
        GetAllTeleporters(gauntletChunk, arrivalTeleporters, "LandingTeleporter");

        List<GameObject> currentTeleporters = new List<GameObject>();
        GetAllTeleporters(gauntletChunk, currentTeleporters, "LandingTeleporter");
        for (int j = 0; j < currentTeleporters.Count; j++)
        {
            currentTeleporters[j].GetComponent<LandingTeleporter>().chunkIndex = index;
        }
    }

    private GameObject CreateBattleAlley(List<GameObject> teleporters, bool createTrainer)
    {
        GameObject battleSpot = Instantiate(battleAlley);
        battleSpot.transform.position = nextChunkPos;
        if (createTrainer)
        {
            gameManager.ChooseMergantsToSpawn(whatToSpawn, true, nextChunkPos, currentLenght, currentTerrainPieces);
        }
        SetNextChunkPos(createTrainer);
        List<GameObject> teleporterOfAlley = new List<GameObject>();
        GetAllTeleporters(battleSpot, teleporterOfAlley, "LandingTeleporter");

        for (int i = 0; i < teleporters.Count; i++)
        {
            teleporters[i].GetComponent<Teleporter>().SetGameManager(gameManager);
            teleporters[i].GetComponent<Teleporter>().connectedSpawnPlace = teleporterOfAlley[0].transform;
        }

        teleporterOfAlley[0].GetComponent<BattleLandingTeleporter>().SetTerrainManager(this);

        List<GameObject> leaveTeleporterOfAlley = new List<GameObject>();
        GetAllTeleporters(battleSpot, leaveTeleporterOfAlley, "LeaveTeleporter");
        teleporterOfAlley[0].GetComponent<BattleLandingTeleporter>().SetConnectedTeleporter(leaveTeleporterOfAlley[0].GetComponent<BattleAlleyTeleporter>());
        battleSpots.Add(battleSpot);

        return battleSpot;
    }

    private LandingTeleporter.SpawnPosition DecideDirection(LandingTeleporter.SpawnPosition direction)
    {
        LandingTeleporter.SpawnPosition neededDirection = LandingTeleporter.SpawnPosition.OutOfReach;

        switch (direction)
        {
            case LandingTeleporter.SpawnPosition.Left:
                if (path == LandingTeleporter.SpawnPosition.Left || path == LandingTeleporter.SpawnPosition.Middle)
                {
                    neededDirection = LandingTeleporter.SpawnPosition.Left;
                }
                else if (path == LandingTeleporter.SpawnPosition.Right)
                {
                    neededDirection = LandingTeleporter.SpawnPosition.Middle;
                }
                break;
            case LandingTeleporter.SpawnPosition.Right:
                if (path == LandingTeleporter.SpawnPosition.Right || path == LandingTeleporter.SpawnPosition.Middle)
                {
                    neededDirection = LandingTeleporter.SpawnPosition.Right;
                }
                else if (path == LandingTeleporter.SpawnPosition.Left)
                {
                    neededDirection = LandingTeleporter.SpawnPosition.Middle;
                }
                break;
            case LandingTeleporter.SpawnPosition.Middle:
                neededDirection = LandingTeleporter.SpawnPosition.Middle;
                break;
            default:
                print("not neccesary yet");
                break;
        }

        return neededDirection;
    }

    private LandingTeleporter.SpawnPosition GauntletTeleporterPosDecision(LandingTeleporter.SpawnPosition direction,
    LandingTeleporter.SpawnPosition current, int index)
    {
        LandingTeleporter.SpawnPosition neededPos = LandingTeleporter.SpawnPosition.OutOfReach;

        switch (direction)
        {
            case LandingTeleporter.SpawnPosition.Left:
                if (path == LandingTeleporter.SpawnPosition.Left)
                {
                    if (index == 0)
                    {
                        if (current == LandingTeleporter.SpawnPosition.Left)
                        {
                            neededPos = direction;
                        }
                    }
                }
                else if (path == LandingTeleporter.SpawnPosition.Middle)
                {
                    if (index == 0)
                    {
                        if (current == LandingTeleporter.SpawnPosition.Right)
                        {
                            neededPos = direction;
                        }
                    }
                }
                else if (path == LandingTeleporter.SpawnPosition.Right)
                {
                    if (index == 1)
                    {
                        if (current == LandingTeleporter.SpawnPosition.Right)
                        {
                            neededPos = direction;
                        }
                    }
                }
                break;
            case LandingTeleporter.SpawnPosition.Right:
                if (path == LandingTeleporter.SpawnPosition.Right)
                {
                    if (index == 2)
                    {
                        if (current == LandingTeleporter.SpawnPosition.Right)
                        {
                            neededPos = direction;
                        }
                    }
                }
                else if (path == LandingTeleporter.SpawnPosition.Middle)
                {
                    if (index == 2)
                    {
                        if (current == LandingTeleporter.SpawnPosition.Left)
                        {
                            neededPos = direction;
                        }
                    }
                }
                else if (path == LandingTeleporter.SpawnPosition.Left)
                {
                    if (index == 1)
                    {
                        if (current == LandingTeleporter.SpawnPosition.Left)
                        {
                            neededPos = direction;
                        }
                    }
                }
                break;
            case LandingTeleporter.SpawnPosition.Middle:
                if (path == LandingTeleporter.SpawnPosition.Middle)
                {
                    if (index == 1)
                    {
                        if (current == LandingTeleporter.SpawnPosition.Middle)
                        {
                            neededPos = direction;
                        }
                    }
                }
                break;
            default:
                print("not neccesary yet");
                break;
        }

        return neededPos;
    }

    private LandingTeleporter.SpawnPosition SmallGauntletTeleporterPosDecision(LandingTeleporter.SpawnPosition direction,
        LandingTeleporter.SpawnPosition current, int index)
    {
        LandingTeleporter.SpawnPosition neededPos = LandingTeleporter.SpawnPosition.OutOfReach;

        switch (direction)
        {
            case LandingTeleporter.SpawnPosition.Left:
                neededPos = DecideSmallGauntletTeleporterPos(direction, current, index, 0);
                break;
            case LandingTeleporter.SpawnPosition.Right:
                neededPos = DecideSmallGauntletTeleporterPos(direction, current, index, 1);
                break;
            default:
                print("not neccesary yet");
                break;
        }

        return neededPos;
    }

    private LandingTeleporter.SpawnPosition DecideSmallGauntletTeleporterPos(LandingTeleporter.SpawnPosition direction,
        LandingTeleporter.SpawnPosition current, int index, int chunkIndexToCheck)
    {
        LandingTeleporter.SpawnPosition neededPos = LandingTeleporter.SpawnPosition.OutOfReach;
        if (path == LandingTeleporter.SpawnPosition.Left)
        {
            if (index == chunkIndexToCheck)
            {
                if (current == LandingTeleporter.SpawnPosition.Left)
                {
                    neededPos = direction;
                }
            }
        }
        else if (path == LandingTeleporter.SpawnPosition.Right)
        {
            if (index == chunkIndexToCheck)
            {
                if (current == LandingTeleporter.SpawnPosition.Right)
                {
                    neededPos = direction;
                }
            }
        }

        return neededPos;
    }

    private void SetNextChunkPos(bool editPos)
    {
        if (editPos)
        {
            nextChunkPos.x += 50;
            nextChunkPos.z += 50;
        }
    }

    private void GetAllTeleporters(GameObject landPiece, List<GameObject> teleporters, string tag)
    {
        for (int i = 0; i < landPiece.transform.childCount; i++)
        {
            Transform child = landPiece.transform.GetChild(i);
            if (child.tag == tag)
            {
                teleporters.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                GetAllTeleporters(child.gameObject, teleporters, tag);
            }
        }
    }

    public void LoadSavedStats(PlayerData data)
    {
        lengthOfRun = data.lengthOfRun;
        currentLenght = data.currentLenght;
        currentPlaceInGuantlet = data.currentPlaceInGuantlet;
        currentMapChunk = (MapChunksToSpawn)System.Enum.Parse(typeof(MapChunksToSpawn), data.currentMapChunk);
        whatToSpawn = (MapChunksToSpawn)System.Enum.Parse(typeof(MapChunksToSpawn), data.whatToSpawn);
        path = (LandingTeleporter.SpawnPosition)System.Enum.Parse(typeof(LandingTeleporter.SpawnPosition), data.path);

        nextChunkPos.x = data.nextChunkPos[0];
        nextChunkPos.y = data.nextChunkPos[1];
        nextChunkPos.z = data.nextChunkPos[2];

        List<LandingTeleporter> landingTeleporters = new List<LandingTeleporter>();

        switch (currentMapChunk)
        {
            case MapChunksToSpawn.StartOfMap:
                SpawnStartOfMap(false);
                break;
            case MapChunksToSpawn.SecondPocketMonster:
                landingTeleporters = CreateFirstStop(landingTeleporters, path, false);
                break;
            case MapChunksToSpawn.FirstMoveSelection:
                landingTeleporters = CreateFirstMoveSelection(landingTeleporters, path, false);
                break;
            case MapChunksToSpawn.SecondMoveSelection:
                landingTeleporters = CreateSecondMoveSelection(landingTeleporters, path, false);
                break;
            case MapChunksToSpawn.ThirdPocketMonster:
                landingTeleporters = CreateThirdPocketMonsterSelection(landingTeleporters, path, false);
                break;
            case MapChunksToSpawn.StartOfGauntlet:
                landingTeleporters = CreateStartOfGauntlet(landingTeleporters, path, false);
                break;
            case MapChunksToSpawn.MidGauntlet:
                landingTeleporters = CreateMidGuantlet(landingTeleporters, path, false);
                break;
            case MapChunksToSpawn.EndOfGuantlet:
                landingTeleporters = CreateEndGuantlet(landingTeleporters, path, false);
                break;
            case MapChunksToSpawn.BigItemRound:
                landingTeleporters = CreateBigItemRound(landingTeleporters, path, false);
                break;
            case MapChunksToSpawn.EndOfGame:
                landingTeleporters = CreateEndOfGame(landingTeleporters, path, false);
                break;
            default:
                print("No map pieces found to spawn");
                break;
        }

        List<GameObject> teleporters = new List<GameObject>();

        for (int i = 0; i < currentTerrainPieces.Count; i++)
        {
            Vector3 pos = Vector3.zero;
            pos.x = data.currentTerrainPiecesPosses[i][0];
            pos.y = data.currentTerrainPiecesPosses[i][1];
            pos.z = data.currentTerrainPiecesPosses[i][2];
            currentTerrainPieces[i].transform.position = pos;

            GetAllTeleporters(currentTerrainPieces[i], teleporters, "LeaveTeleporter");
        }

        for (int i = 0; i < data.battleSpotsPosses.Count; i++)
        {
            GameObject battleSpot = CreateBattleAlley(teleporters, false);

            Vector3 pos = Vector3.zero;
            pos.x = data.battleSpotsPosses[i][0];
            pos.y = data.battleSpotsPosses[i][1];
            pos.z = data.battleSpotsPosses[i][2];
            battleSpot.transform.position = pos;

            if (i == data.battleSpotsPosses.Count - 2 && currentMapChunk != MapChunksToSpawn.EndOfGame ||
                i == data.battleSpotsPosses.Count - 1 && currentMapChunk == MapChunksToSpawn.EndOfGame)
            {
                List<GameObject> battleLandingTeleporter = new List<GameObject>();
                GetAllTeleporters(battleSpot, battleLandingTeleporter, "LandingTeleporter");

                LandingTeleporter.SpawnPosition spawnPosition = (LandingTeleporter.SpawnPosition)System.Enum.Parse(typeof
                    (LandingTeleporter.SpawnPosition), data.teleporterSpawnPos);
                for (int j = 0; j < landingTeleporters.Count; j++)
                {
                    if (landingTeleporters[j].spawnPosition == spawnPosition)
                    {
                        battleLandingTeleporter[0].GetComponent<BattleLandingTeleporter>().connectedTeleporter.connectedSpawnPlace =
                            landingTeleporters[j].gameObject.transform;
                    }
                }
            }
        }
    }

    private void ClearTerrainPieces(bool destroy)
    {
        if (destroy)
        {
            for (int i = 0; i < currentTerrainPieces.Count; i++)
            {
                Destroy(currentTerrainPieces[i]);
            }
        }
        currentTerrainPieces.Clear();
    }

    public void SetGameManger(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
}
