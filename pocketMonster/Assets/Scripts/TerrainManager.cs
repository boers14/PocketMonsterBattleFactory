using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [SerializeField]
    private GameObject startSpawn = null, firstStop = null, midwayMiddleStop = null, bigStop = null, midwayStopOnSide = null, battleAlley = null,
        endStopSmall = null, endStopMiddle = null, battleArena = null;

    [SerializeField]
    private int lengthOfRun = 6;

    public int currentLenght = 2, currentPlaceInGuantlet = 0;

    private GameManager gameManager;

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

    public MapChunksToSpawn whatToSpawn = MapChunksToSpawn.StartOfMap;

    public LandingTeleporter.SpawnPosition path = LandingTeleporter.SpawnPosition.Middle;

    [SerializeField]
    private Vector3 nextChunkPos = Vector3.zero;

    public Vector3 arenaPos = new Vector3(-50, 0, -50);

    private List<GameObject> currentTerrainPieces = new List<GameObject>();

    private void Start()
    {
        GameObject runSettingManager = GameObject.FindGameObjectsWithTag("RunSettingsManager")[0];
        lengthOfRun = runSettingManager.GetComponent<RunSettingsManager>().lenghtOfRun;
        Destroy(runSettingManager);
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

    public void SpawnStartOfMap()
    {
        ClearTerrainPieces(true);
        GameObject arena = Instantiate(battleArena);
        arena.transform.position = arenaPos;

        GameObject startOfMap = Instantiate(startSpawn);
        startOfMap.transform.position = nextChunkPos;
        currentTerrainPieces.Add(startOfMap);
        gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        SetNextChunkPos();

        List<GameObject> teleporters = new List<GameObject>();
        GetAllTeleporters(startOfMap, teleporters, "LeaveTeleporter");

        GameObject battleSpot = CreateBattleAlley(teleporters);

        whatToSpawn = MapChunksToSpawn.SecondPocketMonster;
    }

    public List<LandingTeleporter> DecideNextSpawnOfMap(LandingTeleporter.SpawnPosition direction)
    {
        List<LandingTeleporter> landingTeleporters = new List<LandingTeleporter>();

        switch (whatToSpawn)
        {
            case MapChunksToSpawn.StartOfMap:
                SpawnStartOfMap();
                print("Reset?");
                break;
            case MapChunksToSpawn.SecondPocketMonster:
                landingTeleporters = CreateFirstStop(landingTeleporters, direction);
                break;
            case MapChunksToSpawn.FirstMoveSelection:
                landingTeleporters = CreateFirstMoveSelection(landingTeleporters, direction);
                break;
            case MapChunksToSpawn.SecondMoveSelection:
                landingTeleporters = CreateSecondMoveSelection(landingTeleporters, direction);
                break;
            case MapChunksToSpawn.ThirdPocketMonster:
                landingTeleporters = CreateThirdPocketMonsterSelection(landingTeleporters, direction);
                break;
            case MapChunksToSpawn.StartOfGauntlet:
                landingTeleporters = CreateStartOfGauntlet(landingTeleporters, direction);
                break;
            case MapChunksToSpawn.MidGauntlet:
                landingTeleporters = CreateMidGuantlet(landingTeleporters, direction);
                break;
            case MapChunksToSpawn.EndOfGuantlet:
                landingTeleporters = CreateEndGuantlet(landingTeleporters, direction);
                break;
            case MapChunksToSpawn.BigItemRound:
                landingTeleporters = CreateBigItemRound(landingTeleporters, direction);
                break;
            case MapChunksToSpawn.EndOfGame:
                landingTeleporters = CreateEndOfGame(landingTeleporters, direction);
                break;
            default:
                print("No map pieces found to spawn");
                break;
        }

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateFirstStop(List<LandingTeleporter> landingTeleporters, LandingTeleporter.SpawnPosition direction)
    {
        ClearTerrainPieces(true);
        GameObject firstStopChunk = Instantiate(firstStop);
        firstStopChunk.transform.position = nextChunkPos;
        currentTerrainPieces.Add(firstStopChunk);
        gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        SetNextChunkPos();

        List<GameObject> teleporters = new List<GameObject>();
        GetAllTeleporters(firstStopChunk, teleporters, "LeaveTeleporter");

        GameObject battleSpot = CreateBattleAlley(teleporters);

        List<GameObject> arrivalTeleporters = new List<GameObject>();
        GetAllTeleporters(firstStopChunk, arrivalTeleporters, "LandingTeleporter");

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition = LandingTeleporter.SpawnPosition.Middle;
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        whatToSpawn = MapChunksToSpawn.FirstMoveSelection;

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateFirstMoveSelection(List<LandingTeleporter> landingTeleporters, LandingTeleporter.SpawnPosition direction)
    {
        ClearTerrainPieces(true);
        List<GameObject> teleporters = new List<GameObject>();
        List<GameObject> arrivalTeleporters = new List<GameObject>();

        for (int i = 0; i < 2; i++)
        {
            GameObject firstStopChunk = Instantiate(firstStop);
            SetGauntlerChunkPosAndSetTeleporters(firstStopChunk, teleporters, arrivalTeleporters, i);
        }

        GameObject battleSpot = CreateBattleAlley(teleporters);

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

        whatToSpawn = MapChunksToSpawn.SecondMoveSelection;

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateSecondMoveSelection(List<LandingTeleporter> landingTeleporters, LandingTeleporter.SpawnPosition direction)
    {
        ClearTerrainPieces(true);
        List<GameObject> teleporters = new List<GameObject>();
        List<GameObject> arrivalTeleporters = new List<GameObject>();

        for (int i = 0; i < 2; i++)
        {
            GameObject firstStopChunk = Instantiate(endStopSmall);
            SetGauntlerChunkPosAndSetTeleporters(firstStopChunk, teleporters, arrivalTeleporters, i);
        }

        GameObject battleSpot = CreateBattleAlley(teleporters);

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            LandingTeleporter.SpawnPosition neededSpawnPos = SmallGauntletTeleporterPosDecision(direction,
                arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition, arrivalTeleporters[i].GetComponent<LandingTeleporter>().chunkIndex);
            arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition = neededSpawnPos;
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        path = direction;

        whatToSpawn = MapChunksToSpawn.ThirdPocketMonster;

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateThirdPocketMonsterSelection(List<LandingTeleporter> landingTeleporters, LandingTeleporter.SpawnPosition direction)
    {
        ClearTerrainPieces(true);
        GameObject thirdPocketMonster = Instantiate(midwayMiddleStop);
        thirdPocketMonster.transform.position = nextChunkPos;
        currentTerrainPieces.Add(thirdPocketMonster);
        gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        SetNextChunkPos();

        List<GameObject> teleporters = new List<GameObject>();
        GetAllTeleporters(thirdPocketMonster, teleporters, "LeaveTeleporter");

        GameObject battleSpot = CreateBattleAlley(teleporters);

        List<GameObject> arrivalTeleporters = new List<GameObject>();
        GetAllTeleporters(thirdPocketMonster, arrivalTeleporters, "LandingTeleporter");

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            arrivalTeleporters[i].GetComponent<LandingTeleporter>().spawnPosition = direction;
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        path = LandingTeleporter.SpawnPosition.Middle;

        whatToSpawn = MapChunksToSpawn.StartOfGauntlet;

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateStartOfGauntlet(List<LandingTeleporter> landingTeleporters, LandingTeleporter.SpawnPosition direction)
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

            SetGauntlerChunkPosAndSetTeleporters(gauntletChunk, teleporters, arrivalTeleporters, i);
        }

        gameManager.GoToNextPickUp();

        GameObject battleSpot = CreateBattleAlley(teleporters);

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

    private List<LandingTeleporter> CreateMidGuantlet(List<LandingTeleporter> landingTeleporters, LandingTeleporter.SpawnPosition direction)
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

            SetGauntlerChunkPosAndSetTeleporters(gauntletChunk, teleporters, arrivalTeleporters, i);
        }

        gameManager.GoToNextPickUp();

        GameObject battleSpot = CreateBattleAlley(teleporters);

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

    private List<LandingTeleporter> CreateEndGuantlet(List<LandingTeleporter> landingTeleporters, LandingTeleporter.SpawnPosition direction)
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

            SetGauntlerChunkPosAndSetTeleporters(gauntletChunk, teleporters, arrivalTeleporters, i);
        }

        gameManager.GoToNextPickUp();

        GameObject battleSpot = CreateBattleAlley(teleporters);

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

    private List<LandingTeleporter> CreateBigItemRound(List<LandingTeleporter> landingTeleporters, LandingTeleporter.SpawnPosition direction)
    {
        ClearTerrainPieces(true);
        GameObject bigItemRound = Instantiate(bigStop);
        bigItemRound.transform.position = nextChunkPos;
        currentTerrainPieces.Add(bigItemRound);
        gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        SetNextChunkPos();

        List<GameObject> teleporters = new List<GameObject>();
        GetAllTeleporters(bigItemRound, teleporters, "LeaveTeleporter");

        GameObject battleSpot = CreateBattleAlley(teleporters);

        List<GameObject> arrivalTeleporters = new List<GameObject>();
        GetAllTeleporters(bigItemRound, arrivalTeleporters, "LandingTeleporter");

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        path = LandingTeleporter.SpawnPosition.Middle;

        whatToSpawn = MapChunksToSpawn.StartOfGauntlet;

        currentLenght++;
        currentPlaceInGuantlet = 0;

        return landingTeleporters;
    }

    private List<LandingTeleporter> CreateEndOfGame(List<LandingTeleporter> landingTeleporters, LandingTeleporter.SpawnPosition direction)
    {
        ClearTerrainPieces(true);
        GameObject lastItemRound = Instantiate(endStopMiddle);
        lastItemRound.transform.position = nextChunkPos;
        currentTerrainPieces.Add(lastItemRound);
        gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        SetNextChunkPos();

        List<GameObject> teleporters = new List<GameObject>();
        GetAllTeleporters(lastItemRound, teleporters, "LeaveTeleporter");

        List<GameObject> arrivalTeleporters = new List<GameObject>();
        GetAllTeleporters(lastItemRound, arrivalTeleporters, "LandingTeleporter");

        for (int i = 0; i < arrivalTeleporters.Count; i++)
        {
            landingTeleporters.Add(arrivalTeleporters[i].GetComponent<LandingTeleporter>());
        }

        ClearTerrainPieces(false);
        GameObject battleSpot = Instantiate(startSpawn);
        battleSpot.transform.position = nextChunkPos;
        currentTerrainPieces.Add(battleSpot);
        gameManager.ChooseMergantsToSpawn(whatToSpawn, true, nextChunkPos, currentLenght,currentTerrainPieces);
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
        whatToSpawn = MapChunksToSpawn.LastBattle;

        return landingTeleporters;
    }

    private void SetGauntlerChunkPosAndSetTeleporters(GameObject gauntletChunk, List<GameObject> teleporters, List<GameObject> arrivalTeleporters, int index)
    {
        gauntletChunk.transform.position = nextChunkPos;
        currentTerrainPieces.Add(gauntletChunk);
        gameManager.ChooseMergantsToSpawn(whatToSpawn, false, nextChunkPos, currentLenght, currentTerrainPieces);
        SetNextChunkPos();

        GetAllTeleporters(gauntletChunk, teleporters, "LeaveTeleporter");
        GetAllTeleporters(gauntletChunk, arrivalTeleporters, "LandingTeleporter");

        List<GameObject> currentTeleporters = new List<GameObject>();
        GetAllTeleporters(gauntletChunk, currentTeleporters, "LandingTeleporter");
        for (int j = 0; j < currentTeleporters.Count; j++)
        {
            currentTeleporters[j].GetComponent<LandingTeleporter>().chunkIndex = index;
        }
    }

    private GameObject CreateBattleAlley(List<GameObject> teleporters)
    {
        GameObject battleSpot = Instantiate(battleAlley);
        battleSpot.transform.position = nextChunkPos;
        gameManager.ChooseMergantsToSpawn(whatToSpawn, true, nextChunkPos, currentLenght, currentTerrainPieces);
        SetNextChunkPos();
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

    private void SetNextChunkPos()
    {
        nextChunkPos.x += 50;
        nextChunkPos.z += 50;
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
