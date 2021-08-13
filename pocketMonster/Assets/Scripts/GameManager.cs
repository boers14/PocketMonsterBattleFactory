using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerObject = null, trainer = null, pocketMonsterGiver = null, itemGiver = null, teamBuffGiver = null, moveGiver = null, 
        terrainManager = null, inBattleTextManager = null, enemyManager = null, exitGameMenu = null;

    [SerializeField]
    private Vector3 initialPlayerPos = Vector3.zero;

    [System.NonSerialized]
    public List<PocketMonster> playerPocketMonsters = new List<PocketMonster>(), aiPocketMonsters = new List<PocketMonster>();
    [System.NonSerialized]
    public List<PocketMonsterItem> teamBuffsOfPlayer = new List<PocketMonsterItem>();

    public List<PocketMonster> allPocketMonsters = new List<PocketMonster>();

    [System.NonSerialized]
    public List<PocketMonsterMoves> allPocketMonstersMoves = new List<PocketMonsterMoves>();

    [System.NonSerialized]
    public List<PocketMonsterItem> allPocketMonsterItems = new List<PocketMonsterItem>();

    [System.NonSerialized]
    public List<PocketMonsterItem> allTeamBuffs = new List<PocketMonsterItem>();

    [SerializeField]
    private int amountOfPicks = 3, maxAmountOfMoves = 4, maxAmountOfItems = 3, maxAmountOfTeamBuffs = 3, maxTeamSize = 4;

    [SerializeField]
    private Button buttonPrefab = null, hoverableButtonPrefab = null, exitButton = null;

    private List<Button> buttons = new List<Button>();

    [SerializeField]
    private Text textPrefab = null, hoverText = null;

    [SerializeField]
    private Image imagePrefab = null;

    private Text information = null;
    [System.NonSerialized]
    public Text livesText = null;
    private List<Text> extraTexts = new List<Text>();
    private List<Image> bgsForTexts = new List<Image>();

    [System.NonSerialized]
    public List<GameObject> mergants = new List<GameObject>();
    [System.NonSerialized]
    public List<GameObject> trainers = new List<GameObject>();
    [System.NonSerialized]
    public List<PocketMonsterMoves> currentMovesHandedOut = new List<PocketMonsterMoves>();
    [System.NonSerialized]
    public List<PocketMonsterItem> currentItemsHandedOut = new List<PocketMonsterItem>();
    [System.NonSerialized]
    public CurrentMergant currentMergant = CurrentMergant.PocketMonster;

    public int lives = 3;

    public enum CurrentMergant
    {
        Item,
        Move,
        PocketMonster,
        TeamBuff
    }

    public enum PickupsInGauntlet
    {
        Item,
        Move
    }

    public enum BigPickups
    {
        TeamBuff,
        PocketMonster
    }

    [System.NonSerialized]
    public List<PickupsInGauntlet> pickups = new List<PickupsInGauntlet>();

    [System.NonSerialized]
    public BigPickups nextBigPickUp = BigPickups.PocketMonster;

    [System.NonSerialized]
    public int currentPickUp = 0, itemHandOutIndex = 0;

    [System.NonSerialized]
    public List<PocketMonsterMoves> allMovesHandedOut = new List<PocketMonsterMoves>();

    [System.NonSerialized]
    public List<PocketMonsterItem> itemsToHandOut = new List<PocketMonsterItem>();
    [System.NonSerialized]
    public List<PocketMonsterMoves> movesToHandOut = new List<PocketMonsterMoves>();

    [System.NonSerialized]
    public bool lastBattle = false, playerWon = true, loadedData = false;

    [SerializeField]
    private string endScreenName = "";

    private InBattleTextManager battleTextManager;

    private EnemyManager managerOfEnemys;

    private TerrainManager managerOfTheTerrains;

    [System.NonSerialized]
    public PlayerBattle playerBattle = null;

    public Material teamBuffMaterial = null, finalBattleMaterial = null, pocketMonsterMaterial = null, itemMaterial = null, moveMaterial = null;

    [System.NonSerialized]
    public List<int> playerPocketMonsterInInt = new List<int>(), playerPocketMonstersAbilityInInt = new List<int>(), teamBuffsOfPlayerInInt = new List<int>(),
        allMovesHandedOutInInt = new List<int>(), itemsToHandOutInInt = new List<int>(), movesToHandOutInInt = new List<int>(),
        currentMovesHandedOutInInt = new List<int>(), currentItemsHandedOutInInt = new List<int>();

    [System.NonSerialized]
    public List<List<int>> playerPocketMonsterMovesInInt = new List<List<int>>(), playerPocketMonsterItemsInInt = new List<List<int>>();

    private Button exitGameButton = null;

    private Text savingText = null;

    [SerializeField]
    private Color32 savingTextStartColor = Color.black, savingTextEndColor = Color.black;

    private bool canSave = true;

    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("GameManager").Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        FillPocketMonsterMoveList();
        FillPocketMonsterItemList();
        FillTeamBuffsList();

        if (GameObject.FindGameObjectsWithTag("LoadObject").Length > 0)
        {
            Destroy(GameObject.FindGameObjectWithTag("LoadObject"));
            loadedData = true;
            PlayerData data = SaveSytem.LoadGame();

            lastBattle = data.lastBattle;
            itemHandOutIndex = data.itemHandOutIndex;
            currentPickUp = data.currentPickUp;
            nextBigPickUp = (BigPickups)System.Enum.Parse(typeof(BigPickups), data.nextBigPickUp);
            currentMergant = (CurrentMergant)System.Enum.Parse(typeof(CurrentMergant), data.currentMergant);

            GameObject managerOfTerrain = Instantiate(terrainManager);
            managerOfTheTerrains = managerOfTerrain.GetComponent<TerrainManager>();

            GameObject aiManager = Instantiate(enemyManager);
            managerOfEnemys = aiManager.GetComponent<EnemyManager>();
            managerOfEnemys.SetGameManager(this);
            managerOfEnemys.SetTerrainManager(managerOfTerrain.GetComponent<TerrainManager>());
            managerOfEnemys.LoadSavedStats(data);

            GameObject textMessagerInBattle = Instantiate(inBattleTextManager);
            battleTextManager = textMessagerInBattle.GetComponent<InBattleTextManager>();

            managerOfTheTerrains.SetGameManger(this);
            managerOfTheTerrains.LoadSavedStats(data);

            RecoverMoves(data.allMovesHandedOut, allMovesHandedOut);
            RecoverItems(data.itemsToHandOut, itemsToHandOut, allPocketMonsterItems);
            RecoverMoves(data.movesToHandOut, movesToHandOut);
            RecoverItems(data.currentItemsHandedOut, currentItemsHandedOut, allPocketMonsterItems);
            RecoverMoves(data.currentMovesHandedOut, currentMovesHandedOut);

            pickups.Clear();
            for (int i = 0; i < data.pickups.Count; i++)
            {
                pickups.Add((PickupsInGauntlet)System.Enum.Parse(typeof(PickupsInGauntlet), data.pickups[i]));
            }

            for (int i = 0; i < data.mergantPosses.Count; i++)
            {
                Vector3 mergantPos = Vector3.zero;
                mergantPos.x = data.mergantPosses[i][0];
                mergantPos.y = data.mergantPosses[i][1];
                mergantPos.z = data.mergantPosses[i][2];

                switch (currentMergant)
                {
                    case CurrentMergant.PocketMonster:
                        GeneratePocketMonsterMergant(mergantPos, managerOfTheTerrains.currentTerrainPieces);
                        break;
                    case CurrentMergant.TeamBuff:
                        GenerateTeamBuffMergant(mergantPos, managerOfTheTerrains.currentTerrainPieces, false);
                        break;
                    case CurrentMergant.Item:
                        GameObject itemMergant = Instantiate(itemGiver);
                        itemMergant.transform.position = mergantPos;
                        itemMergant.GetComponent<ItemMergant>().SetGameManager(this);
                        itemMergant.GetComponent<ItemMergant>().SetItem(currentItemsHandedOut[i]);
                        mergants.Add(itemMergant);
                        ColorGroundTerrainPieces(managerOfTheTerrains.currentTerrainPieces, itemMaterial);
                        break;
                    case CurrentMergant.Move:
                        GameObject moveMergant = Instantiate(moveGiver);
                        moveMergant.transform.position = mergantPos;
                        moveMergant.GetComponent<MoveMergant>().SetGameManager(this);
                        moveMergant.GetComponent<MoveMergant>().SetMove(currentMovesHandedOut[i]);
                        mergants.Add(moveMergant);
                        ColorGroundTerrainPieces(managerOfTheTerrains.currentTerrainPieces, moveMaterial);
                        break;
                }
            }

            for (int i = 0; i < data.trainerPosses.Count; i++)
            {
                Vector3 trainerPos = Vector3.zero;
                trainerPos.x = data.trainerPosses[i][0];
                trainerPos.y = data.trainerPosses[i][1];
                trainerPos.z = data.trainerPosses[i][2];
                GenerateTrainer(trainerPos);
            }

            if (managerOfTheTerrains.currentMapChunk == TerrainManager.MapChunksToSpawn.EndOfGame)
            {
                ColorGroundTerrainPieces(managerOfTheTerrains.currentTerrainPieces, finalBattleMaterial);
            }

            GameObject go = Instantiate(playerObject);
            Vector3 playerPos = Vector3.zero;
            playerPos.x = data.playerPos[0];
            playerPos.y = data.playerPos[1];
            playerPos.z = data.playerPos[2];
            go.transform.position = playerPos;
            go.GetComponent<PlayerPocketMonsterMenu>().SetGameManager(this);
            go.GetComponent<CheckGauntletMap>().SetGameManager(this);
            go.GetComponent<CheckGauntletMap>().SetTerrainManager(managerOfTerrain.GetComponent<TerrainManager>());
            go.GetComponent<PlayerBattle>().SetInBattleTextManager(textMessagerInBattle.GetComponent<InBattleTextManager>());
            playerBattle = go.GetComponent<PlayerBattle>();

            RecoverItems(data.teamBuffsOfPlayer, teamBuffsOfPlayer, allTeamBuffs);

            for (int i = 0; i < data.playerPocketMonsters.Count; i++)
            {
                PocketMonster addedPocketMonster = Instantiate(allPocketMonsters[data.playerPocketMonsters[i]]);
                addedPocketMonster.SetAllStats();
                addedPocketMonster.transform.position = new Vector3(-100, -100, -100);

                addedPocketMonster.chosenAbility = addedPocketMonster.possibleAbilitys[data.playerPocketMonstersAbilitys[i]];
                addedPocketMonster.chosenAbility.SetAbilityStats(playerBattle);

                for (int j = 0; j < data.playerPocketMonsterMoves[i].Count; j++)
                {
                    PocketMonsterMoves moveToAdd = allPocketMonstersMoves[data.playerPocketMonsterMoves[i][j]];
                    PocketMonsterMoves addedMove = (PocketMonsterMoves)System.Activator.CreateInstance(moveToAdd.GetType());
                    addedMove.SetMoveStats();
                    addedPocketMonster.moves.Add(addedMove);
                }

                for (int j = 0; j < data.playerPocketMonsterItems[i].Count; j++)
                {
                    PocketMonsterItem itemToAdd = allPocketMonsterItems[data.playerPocketMonsterItems[i][j]];
                    PocketMonsterItem addedItem = (PocketMonsterItem)System.Activator.CreateInstance(itemToAdd.GetType());
                    addedItem.SetStats();
                    addedPocketMonster.items.Add(addedItem);
                }

                playerPocketMonsters.Add(addedPocketMonster);
            }

            textMessagerInBattle.GetComponent<InBattleTextManager>().SetPlayer(playerBattle);

            lives = data.lives;
        }
        else
        {
            GameObject managerOfTerrain = Instantiate(terrainManager);
            managerOfTheTerrains = managerOfTerrain.GetComponent<TerrainManager>();

            GameObject aiManager = Instantiate(enemyManager);
            managerOfEnemys = aiManager.GetComponent<EnemyManager>();
            managerOfEnemys.SetGameManager(this);
            managerOfEnemys.SetTerrainManager(managerOfTerrain.GetComponent<TerrainManager>());

            GameObject textMessagerInBattle = Instantiate(inBattleTextManager);
            battleTextManager = textMessagerInBattle.GetComponent<InBattleTextManager>();

            managerOfTerrain.GetComponent<TerrainManager>().SetGameManger(this);
            managerOfTerrain.GetComponent<TerrainManager>().SpawnStartOfMap(true);

            int amount = managerOfTerrain.GetComponent<TerrainManager>().GetAmountOfPickupsGenerated();
            for (int i = 0; i < amount; i++)
            {
                PickupsInGauntlet pickup = PickupsInGauntlet.Item;

                if (Random.Range(1, 101) > 50)
                {
                    pickup = PickupsInGauntlet.Move;
                }
                pickups.Add(pickup);
            }

            FillItemList();

            GameObject go = Instantiate(playerObject);
            go.transform.position = initialPlayerPos;
            go.GetComponent<PlayerPocketMonsterMenu>().SetGameManager(this);
            go.GetComponent<CheckGauntletMap>().SetGameManager(this);
            go.GetComponent<CheckGauntletMap>().SetTerrainManager(managerOfTerrain.GetComponent<TerrainManager>());
            go.GetComponent<PlayerBattle>().SetInBattleTextManager(textMessagerInBattle.GetComponent<InBattleTextManager>());
            playerBattle = go.GetComponent<PlayerBattle>();

            textMessagerInBattle.GetComponent<InBattleTextManager>().SetPlayer(playerBattle);
        }

        livesText = Instantiate(textPrefab);
        SetUIStats.SetUIPosition(canvas, livesText.gameObject, 6, 12, 2, 2, 1, 1);
        livesText.text = "Lives: " + lives;

        exitGameButton = Instantiate(exitButton);
        SetUIStats.SetUIPosition(canvas, exitGameButton.gameObject, 25, 6, 2, 2, 1, -1, 0, false, false, true, true);

        GameObject exitMenu = Instantiate(exitGameMenu);
        SetUIStats.SetUIPosition(canvas, exitMenu.gameObject, 4, 4, 0, 0, 1, 1, 0, false, false, true);
        exitMenu.GetComponentInChildren<ExitGameNoButton>().exitGameButton = exitGameButton.GetComponent<ExitGameButton>();
        exitMenu.SetActive(false);
        exitGameButton.GetComponent<ExitGameButton>().exitGameMenu = exitMenu;

        savingText = Instantiate(textPrefab);
        SetUIStats.SetUIPosition(canvas, savingText.gameObject, 2, 4, 0, 0, 1, 1);
        savingText.text = "Saved game!";
        savingText.color = savingTextEndColor;

        DontDestroyOnLoad(playerBattle.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && !playerBattle.isInBattle && canSave)
        {
            SaveData();
        }
    }

    public void GoToNextPickUp()
    {
        currentPickUp++;
    }

    public void ChooseMergantsToSpawn(TerrainManager.MapChunksToSpawn whatIsSpawned, bool trainer, Vector3 pos, int lenghtOfGauntlet, 
        List<GameObject> currentTerrainPieces)
    {
        if (trainer)
        {
            GenerateTrainer(pos);
            if (whatIsSpawned == TerrainManager.MapChunksToSpawn.EndOfGame || whatIsSpawned == TerrainManager.MapChunksToSpawn.LastBattle)
            {
                ColorGroundTerrainPieces(currentTerrainPieces, finalBattleMaterial);
            }
        } else
        {
            switch (whatIsSpawned)
            {
                case TerrainManager.MapChunksToSpawn.StartOfMap:
                    GeneratePocketMonsterMergant(pos, currentTerrainPieces);
                    currentMergant = CurrentMergant.PocketMonster;
                    break;
                case TerrainManager.MapChunksToSpawn.SecondPocketMonster:
                    FillMoveList(2, 2);
                    GeneratePocketMonsterMergant(pos, currentTerrainPieces);
                    currentMergant = CurrentMergant.PocketMonster;
                    break;
                case TerrainManager.MapChunksToSpawn.FirstMoveSelection:
                    GenerateMoveMergant(pos, currentTerrainPieces);
                    currentMergant = CurrentMergant.Move;
                    break;
                case TerrainManager.MapChunksToSpawn.SecondMoveSelection:
                    FillMoveList(lenghtOfGauntlet, 3);
                    GenerateMoveMergant(pos, currentTerrainPieces);
                    currentMergant = CurrentMergant.Move;
                    break;
                case TerrainManager.MapChunksToSpawn.ThirdPocketMonster:
                    GeneratePocketMonsterMergant(pos, currentTerrainPieces);
                    currentMergant = CurrentMergant.PocketMonster;
                    break;
                case TerrainManager.MapChunksToSpawn.StartOfGauntlet:
                    GenerateItemOrMoveMergant(pos, currentTerrainPieces);
                    break;
                case TerrainManager.MapChunksToSpawn.MidGauntlet:
                    GenerateItemOrMoveMergant(pos, currentTerrainPieces);
                    break;
                case TerrainManager.MapChunksToSpawn.EndOfGuantlet:
                    FillMoveList(lenghtOfGauntlet, 3);
                    GenerateItemOrMoveMergant(pos, currentTerrainPieces);
                    break;
                case TerrainManager.MapChunksToSpawn.BigItemRound:
                    GenerateTeamBuffOrPocketMonsterMergant(pos, currentTerrainPieces);
                    break;
                case TerrainManager.MapChunksToSpawn.EndOfGame:
                    GenerateTeamBuffOrPocketMonsterMergant(pos, currentTerrainPieces);
                    break;
            }
        }
    }

    private void GenerateTeamBuffOrPocketMonsterMergant(Vector3 pos, List<GameObject> currentTerrainPieces)
    {
        if (nextBigPickUp == BigPickups.PocketMonster)
        {
            GeneratePocketMonsterMergant(pos, currentTerrainPieces);
            nextBigPickUp = BigPickups.TeamBuff;
            currentMergant = CurrentMergant.PocketMonster;
        }
        else
        {
            GenerateTeamBuffMergant(pos, currentTerrainPieces, true);
            nextBigPickUp = BigPickups.PocketMonster;
            currentMergant = CurrentMergant.TeamBuff;
        }
    }

    private void GenerateItemOrMoveMergant(Vector3 pos, List<GameObject> currentTerrainPieces)
    {
        if (pickups[currentPickUp] == PickupsInGauntlet.Item)
        {
            GenerateItemMergant(pos, currentTerrainPieces);
            currentMergant = CurrentMergant.Item;
        }
        else
        {
            GenerateMoveMergant(pos, currentTerrainPieces);
            currentMergant = CurrentMergant.Move;
        }
    }

    private void GeneratePocketMonsterMergant(Vector3 pos, List<GameObject> currentTerrainPieces)
    {
        GameObject pocketMonsterMergant = Instantiate(pocketMonsterGiver);
        pocketMonsterMergant.transform.position = pos;
        pocketMonsterMergant.GetComponent<PocketMonsterMergant>().SetGameManager(this);
        mergants.Add(pocketMonsterMergant);
        ColorGroundTerrainPieces(currentTerrainPieces, pocketMonsterMaterial);
    }

    private void GenerateMoveMergant(Vector3 pos, List<GameObject> currentTerrainPieces)
    {
        GameObject moveMergant = Instantiate(moveGiver);
        moveMergant.transform.position = pos;
        moveMergant.GetComponent<MoveMergant>().SetGameManager(this);
        currentMovesHandedOut.Add(movesToHandOut[0]);
        moveMergant.GetComponent<MoveMergant>().SetMove(movesToHandOut[0]);
        movesToHandOut.RemoveAt(0);
        mergants.Add(moveMergant);
        ColorGroundTerrainPieces(currentTerrainPieces, moveMaterial);
    }

    private void GenerateItemMergant(Vector3 pos, List<GameObject> currentTerrainPieces)
    {
        GameObject itemMergant = Instantiate(itemGiver);
        itemMergant.transform.position = pos;
        itemMergant.GetComponent<ItemMergant>().SetGameManager(this);
        currentItemsHandedOut.Add(itemsToHandOut[itemHandOutIndex]);
        itemMergant.GetComponent<ItemMergant>().SetItem(itemsToHandOut[itemHandOutIndex]);
        itemHandOutIndex++;
        mergants.Add(itemMergant);
        ColorGroundTerrainPieces(currentTerrainPieces, itemMaterial);
    }

    private void GenerateTeamBuffMergant(Vector3 pos, List<GameObject> currentTerrainPieces, bool addTeamBuffsToEnemy)
    {
        if (managerOfEnemys.amountOfTeamBuffs < 3 && addTeamBuffsToEnemy)
        {
            managerOfEnemys.amountOfTeamBuffs++;
        }
        GameObject teamBuffMergant = Instantiate(teamBuffGiver);
        teamBuffMergant.transform.position = pos;
        teamBuffMergant.GetComponent<TeamBuffMergant>().SetGameManager(this);
        mergants.Add(teamBuffMergant);
        ColorGroundTerrainPieces(currentTerrainPieces, teamBuffMaterial);
    }

    private void GenerateTrainer(Vector3 pos)
    {
        GameObject opponent = Instantiate(trainer);
        opponent.transform.position = pos;
        opponent.GetComponent<OverworldTrainer>().SetGameManager(this);
        opponent.GetComponent<OverworldTrainer>().SetTerrainManager(managerOfTheTerrains);
        opponent.GetComponent<OverworldTrainer>().SetEnemyManager(managerOfEnemys);
        opponent.GetComponent<TrainerAi>().SetInBattleTextManager(battleTextManager);
        trainers.Add(opponent);
    }

    public void ColorGroundTerrainPieces(List<GameObject> currentTerrainPieces, Material material)
    {
        for (int i = 0; i < currentTerrainPieces.Count; i++)
        {
            GameObject groundPiece = null;
            for (int j = 0; j < currentTerrainPieces[i].transform.childCount; j++)
            {
                if (currentTerrainPieces[i].transform.GetChild(j).tag == "Ground")
                {
                    groundPiece = currentTerrainPieces[i].transform.GetChild(j).gameObject;
                }
            }
            groundPiece.GetComponent<MeshRenderer>().material = material;
        }
    }

    public void CreatePickAPocketMonsterMenu(GameObject player, List<PocketMonster> randomListOfPocketMonsters = null, 
        PocketMonster removedPocketMonster = null)
    {
        if (randomListOfPocketMonsters == null)
        {
            randomListOfPocketMonsters = new List<PocketMonster>();

            for (int i = 0; i < amountOfPicks; i++)
            {
                PocketMonster randomPocketMonster = ChooseRandomPocketMonster(randomListOfPocketMonsters);
                while (randomPocketMonster == null)
                {
                    randomPocketMonster = ChooseRandomPocketMonster(randomListOfPocketMonsters);
                }
                randomListOfPocketMonsters.Add(randomPocketMonster);
            }
        }

        if (playerPocketMonsters.Count >= maxTeamSize)
        {
            CreateAskIfWantToRemove(player, randomListOfPocketMonsters);
            return;
        }

        InstantiateText(false);
        information.text = "Pick a pocketmonster:";

        CreatePocketMonsterInformation(GameObject.Find("Canvas"), randomListOfPocketMonsters, player.GetComponent<PlayerBattle>());

        for (int i = 0; i < randomListOfPocketMonsters.Count; i++)
        {
            Button button = Instantiate(buttonPrefab);
            buttons.Add(button);
            SetButtonPosition(button, i, 3);
            button.GetComponentInChildren<Text>().text = randomListOfPocketMonsters[i].stats.name;

            SetPocketMonsterAddButton(i, randomListOfPocketMonsters, player, removedPocketMonster);
        }
    }

    private void SetPocketMonsterAddButton(int index, List<PocketMonster> randomListOfPocketMonsters, GameObject player, 
        PocketMonster removedPocketMonster)
    {
        buttons[index].onClick.AddListener(() => OnPocketMonsterAdd(index, randomListOfPocketMonsters, player, removedPocketMonster));
    }

    private void OnPocketMonsterAdd(int index, List<PocketMonster> randomListOfPocketMonsters, GameObject player,
        PocketMonster removedPocketMonster)
    {
        PocketMonster addedPocketMonster = Instantiate(randomListOfPocketMonsters[index]);
        addedPocketMonster.SetAllStats();
        addedPocketMonster.transform.position = new Vector3(-100, -100, -100);
        playerPocketMonsters.Add(addedPocketMonster);

        if (removedPocketMonster != null)
        {
            addedPocketMonster.items = removedPocketMonster.items;
        }

        List<PocketMonsterMoves> specifickMoveList = new List<PocketMonsterMoves>();
        specifickMoveList = CreateListForStabMoveForPocketMonster(addedPocketMonster, specifickMoveList);

        PocketMonsterMoves moveToAdd = specifickMoveList[Random.Range(0, specifickMoveList.Count)];
        PocketMonsterMoves addedMove = (PocketMonsterMoves)System.Activator.CreateInstance(moveToAdd.GetType());
        addedMove.SetMoveStats();
        addedPocketMonster.moves.Add(addedMove);

        CreatePickAMoveForPocketMonsterMenu(addedPocketMonster, player, removedPocketMonster);
    }

    private void CreatePickAMoveForPocketMonsterMenu(PocketMonster addedPocketMonster, GameObject player,
        PocketMonster removedPocketMonster)
    {
        ClearButtons();
        InstantiateText(false);
        information.text = "Pick a move to give:";

        List<PocketMonsterMoves> pocketMonsterSpecifickList = new List<PocketMonsterMoves>();
        pocketMonsterSpecifickList = GetMovesToChooseFrom(pocketMonsterSpecifickList, addedPocketMonster);

        List<PocketMonsterMoves> randomListOfPocketMonsterMoves = new List<PocketMonsterMoves>();
        for (int i = 0; i < amountOfPicks; i++)
        {
            Button button = Instantiate(hoverableButtonPrefab);
            buttons.Add(button);
            SetButtonPosition(button, i, 3);

            PocketMonsterMoves randomPocketMonsterMove = ChooseRandomPocketMonsterMove(randomListOfPocketMonsterMoves, addedPocketMonster, pocketMonsterSpecifickList);
            while (randomPocketMonsterMove == null)
            {
                randomPocketMonsterMove = ChooseRandomPocketMonsterMove(randomListOfPocketMonsterMoves, addedPocketMonster, pocketMonsterSpecifickList);
            }

            randomListOfPocketMonsterMoves.Add(randomPocketMonsterMove);
            button.GetComponentInChildren<Text>().text = randomPocketMonsterMove.moveName;
            button.GetComponent<HoverableUiElement>().SetSetInformationWithImages(true);
            button.GetComponent<HoverableUiElement>().SetText(randomPocketMonsterMove.moveDescription);
            playerBattle.SetButtonColor(button, randomPocketMonsterMove, randomPocketMonsterMove.moveName, 0.01f);

            SetPocketMonsterMoveAddButton(i, randomListOfPocketMonsterMoves, addedPocketMonster, player, removedPocketMonster);
        }
    }

    private void SetPocketMonsterMoveAddButton(int index, List<PocketMonsterMoves> randomListOfPocketMonsterMoves, PocketMonster pocketMonster, 
        GameObject player, PocketMonster removedPocketMonster)
    {
        buttons[index].onClick.AddListener(() => OnPocketMonsterMoveAdd(index, randomListOfPocketMonsterMoves, pocketMonster, player, removedPocketMonster));
    }

    private void OnPocketMonsterMoveAdd(int index, List<PocketMonsterMoves> randomListOfPocketMonsterMoves, PocketMonster pocketMonster, GameObject player,
        PocketMonster removedPocketMonster)
    {
        PocketMonsterMoves moveToAdd = randomListOfPocketMonsterMoves[index];
        PocketMonsterMoves addedMove = (PocketMonsterMoves)System.Activator.CreateInstance(moveToAdd.GetType());
        addedMove.SetMoveStats();
        pocketMonster.moves.Add(addedMove);

        if (removedPocketMonster != null)
        {
            if (pocketMonster.moves.Count != removedPocketMonster.moves.Count)
            {
                CreatePickAMoveForPocketMonsterMenu(pocketMonster, player, removedPocketMonster);
            } else if (pocketMonster.moves.Count >= removedPocketMonster.moves.Count)
            {
                Destroy(removedPocketMonster.gameObject);
                ClearButtons();
                CreateChooseAbilityMenu(pocketMonster, player);
            }
        }
        else
        {
            ClearButtons();
            CreateChooseAbilityMenu(pocketMonster, player);
        }
    }

    private void CreateChooseAbilityMenu(PocketMonster pocketMonster, GameObject player)
    {
        InstantiateText(false);
        information.text = "Pick an ability:";

        for (int i = 0; i < pocketMonster.possibleAbilitys.Count; i++)
        {
            pocketMonster.possibleAbilitys[i].SetAbilityStats(player.GetComponent<PlayerBattle>());

            Button button = Instantiate(hoverableButtonPrefab);
            buttons.Add(button);
            SetButtonPosition(button, i, pocketMonster.possibleAbilitys.Count);

            button.GetComponent<HoverableUiElement>().SetText(pocketMonster.possibleAbilitys[i].abilityDescription);
            button.GetComponentInChildren<Text>().text = pocketMonster.possibleAbilitys[i].abilityName;

            SetPickAbility(i, pocketMonster, player);
        }
    }

    private void SetPickAbility(int index, PocketMonster pocketMonster, GameObject player)
    {
        buttons[index].onClick.AddListener(() => PickAbility(index, pocketMonster, player));
    }

    private void PickAbility(int index, PocketMonster pocketMonster, GameObject player)
    {
        pocketMonster.chosenAbility = pocketMonster.possibleAbilitys[index];
        EndConversation(player);
    }

    private void CreateAskIfWantToRemove(GameObject player, List<PocketMonster> randomListOfPocketMonsters)
    {
        InstantiateText(false);
        information.text = "Remove pocketmonster from team for: ";

        GameObject canvas = GameObject.Find("Canvas");

        CreatePocketMonsterInformation(canvas, randomListOfPocketMonsters, player.GetComponent<PlayerBattle>());

        for (int i = 0; i < 2; i++)
        {
            CreateYesNoMenu(i);
            SetYesOrNoButton(i, player, randomListOfPocketMonsters);
        }
    }

    private void SetYesOrNoButton(int index, GameObject player, List<PocketMonster> randomListOfPocketMonsters)
    {
        buttons[index].onClick.AddListener(() => MakeDecision(index, player, randomListOfPocketMonsters));
    }

    private void MakeDecision(int index, GameObject player, List<PocketMonster> randomListOfPocketMonsters)
    {
        if (index == 0)
        {
            ClearButtons();
            CreateRemovePocketMonsterFromPartyUI(player, randomListOfPocketMonsters);
        }
        else
        {
            EndConversation(player);
        }
    }

    private void CreateRemovePocketMonsterFromPartyUI(GameObject player, List<PocketMonster> randomListOfPocketMonsters)
    {
        InstantiateText(false);
        information.text = "What Pocketmonster to remove?";
        for (int i = 0; i < playerPocketMonsters.Count; i++)
        {
            Button button = Instantiate(hoverableButtonPrefab);
            buttons.Add(button);

            SetButtonPosition(button, i, 4);

            button.GetComponentInChildren<Text>().text = playerPocketMonsters[i].stats.name;
            button.GetComponent<HoverableUiElement>().SetYSize(2.5f);
            button.GetComponent<HoverableUiElement>().SetText(SetPocketMonstersInformation(playerPocketMonsters[i], false));
            SetRemovePocketMonsterButton(i, player, randomListOfPocketMonsters);
        }
    }

    private void SetRemovePocketMonsterButton(int index, GameObject player, List<PocketMonster> randomListOfPocketMonsters)
    {
        buttons[index].onClick.AddListener(() => RemovePocketMonster(index, player, randomListOfPocketMonsters));
    }

    private void RemovePocketMonster(int index, GameObject player, List<PocketMonster> randomListOfPocketMonsters)
    {
        PocketMonster removedPocketMonster = playerPocketMonsters[index];
        playerPocketMonsters.RemoveAt(index);
        ClearButtons();
        CreatePickAPocketMonsterMenu(player, randomListOfPocketMonsters, removedPocketMonster);
    }

    public void CreatePocketMonsterMenuForMoveToGive(PocketMonsterMoves move, GameObject player)
    {
        int countOfPocketmonstersThatHaveTheMove = 0;

        for (int i = 0; i < playerPocketMonsters.Count; i++)
        {
            List<string> moveTypes = new List<string>();
            for (int j = 0; j < playerPocketMonsters[i].moves.Count; j++)
            {
                string moveType = playerPocketMonsters[i].moves[j].GetType().ToString();
                moveTypes.Add(moveType);
            }

            if (moveTypes.Contains(move.GetType().ToString()))
            {
                countOfPocketmonstersThatHaveTheMove++;
            }
        }

        if (countOfPocketmonstersThatHaveTheMove == playerPocketMonsters.Count)
        {
            InstantiateText(false);
            information.text = "Can't give " + move.moveName + " to any of the pocketmonsters since they already have it";

            Button button = Instantiate(buttonPrefab);
            buttons.Add(button);

            SetButtonPosition(button, 0, 1);

            button.GetComponentInChildren<Text>().text = "Ok";
            button.onClick.AddListener(() => EndConversation(player));
            return;
        }

        InstantiateText(true);
        information.text = "Which pocketmonster to give the move " + move.moveName +"?";
        information.GetComponent<HoverableUiElement>().SetText(move.moveDescription);
        playerBattle.GetComponent<CheckGauntletMap>().SetTextDescription(information, move);

        for (int i = 0; i < playerPocketMonsters.Count; i++)
        {
            Button button = Instantiate(hoverableButtonPrefab);
            buttons.Add(button);

            SetButtonPosition(button, i, playerPocketMonsters.Count);

            button.GetComponentInChildren<Text>().text = playerPocketMonsters[i].stats.name;
            button.GetComponent<HoverableUiElement>().SetYSize(2.5f);
            button.GetComponent<HoverableUiElement>().SetText(SetPocketMonstersInformation(playerPocketMonsters[i], false));
            SetAddMoveToPocketMonster(i, playerPocketMonsters[i], move, player);
        }
    }

    private void SetAddMoveToPocketMonster(int index, PocketMonster pocketMonster, PocketMonsterMoves move, GameObject player)
    {
        buttons[index].onClick.AddListener(() => AddMoveToPocketMonster(index, pocketMonster, move, player));
    }

    private void AddMoveToPocketMonster(int index, PocketMonster pocketMonster, PocketMonsterMoves move, GameObject player)
    {
        List<string> moveTypes = new List<string>();
        for (int i = 0; i < pocketMonster.moves.Count; i++)
        {
            string moveType = pocketMonster.moves[i].GetType().ToString();
            moveTypes.Add(moveType);
        }

        if (moveTypes.Contains(move.GetType().ToString()))
        {
            information.text = "Pick another pocketmonster since " + pocketMonster.stats.name + " already learned the move " + move.moveName + ".";
        } else if (pocketMonster.moves.Count >= maxAmountOfMoves)
        {
            ClearButtons();
            CreateMoveDeletion(pocketMonster, move, player);
        } else {
            PocketMonsterMoves addedMove = (PocketMonsterMoves)System.Activator.CreateInstance(move.GetType());
            addedMove.SetMoveStats();
            pocketMonster.moves.Add(addedMove);
            EndConversation(player);
        }
    }

    private void CreateMoveDeletion(PocketMonster pocketMonster, PocketMonsterMoves move, GameObject player)
    {
        InstantiateText(false);
        information.text = "Remove move from pocketmonster?";
        for (int i = 0; i < 2; i++)
        {
            CreateYesNoMenu(i);
            SetChooseIfWantToDeleteMove(i, pocketMonster, move, player);
        }
    }

    private void SetChooseIfWantToDeleteMove(int index, PocketMonster pocketMonster, PocketMonsterMoves move, GameObject player)
    {
        buttons[index].onClick.AddListener(() => ChooseIfWantToDeleteMove(index, pocketMonster, move, player));
    }

    private void ChooseIfWantToDeleteMove(int index, PocketMonster pocketMonster, PocketMonsterMoves move, GameObject player)
    {
        if (index == 0)
        {
            ClearButtons();
            CreateRemoveMoveFromPocketMonster(pocketMonster, move, player);
        }
        else
        {
            ClearButtons();
            CreateCheckIfWantToEndConversation(player, move);
        }
    }

    private void CreateRemoveMoveFromPocketMonster(PocketMonster pocketMonster, PocketMonsterMoves move, GameObject player)
    {
        InstantiateText(true);
        information.text = "What move to remove for " + move.moveName +"?";
        information.GetComponent<HoverableUiElement>().SetText(move.moveDescription);
        playerBattle.GetComponent<CheckGauntletMap>().SetTextDescription(information, move);

        for (int i = 0; i < pocketMonster.moves.Count; i++)
        {
            Button button = Instantiate(hoverableButtonPrefab);
            buttons.Add(button);

            SetButtonPosition(button, i, 4);

            button.GetComponent<HoverableUiElement>().SetSetInformationWithImages(true);
            button.GetComponent<HoverableUiElement>().SetText(pocketMonster.moves[i].moveDescription);
            button.GetComponentInChildren<Text>().text = pocketMonster.moves[i].moveName;
            playerBattle.SetButtonColor(button, pocketMonster.moves[i], pocketMonster.moves[i].moveName, 0.01f);
            SetRemoveMoveFromPocketMonster(i, pocketMonster, move, player);
        }
    }

    private void SetRemoveMoveFromPocketMonster(int index, PocketMonster pocketMonster, PocketMonsterMoves move, GameObject player)
    {
        buttons[index].onClick.AddListener(() => RemoveMoveFromPocketMonster(index, pocketMonster, move, player));
    }

    private void RemoveMoveFromPocketMonster(int index, PocketMonster pocketMonster, PocketMonsterMoves move, GameObject player)
    {
        pocketMonster.moves.RemoveAt(index);
        PocketMonsterMoves addedMove = new PocketMonsterMoves();
        addedMove = move;
        pocketMonster.moves.Add(addedMove);
        EndConversation(player);
    }

    private void CreateCheckIfWantToEndConversation(GameObject player, PocketMonsterMoves move)
    {
        InstantiateText(false);
        information.text = "End conversation?";
        for (int i = 0; i < 2; i++)
        {
            CreateYesNoMenu(i);
            SetCheckIfWantToEndConversation(i, player, move);
        }
    }

    private void SetCheckIfWantToEndConversation(int index, GameObject player, PocketMonsterMoves move)
    {
        buttons[index].onClick.AddListener(() => CheckIfWantToEndConversation(index, player, move));
    }

    private void CheckIfWantToEndConversation(int index, GameObject player, PocketMonsterMoves move)
    {
        if (index == 0)
        {
            EndConversation(player);
        }
        else
        {
            ClearButtons();
            CreatePocketMonsterMenuForMoveToGive(move, player);
        }
    }

    public void CreatePocketMonsterMenuForItemToGive(PocketMonsterItem item, GameObject player)
    {
        InstantiateText(true);
        information.text = "Which pocketmonster to give the item " + item.name + "?";
        information.GetComponent<HoverableUiElement>().SetText(item.itemDescription);

        for (int i = 0; i < playerPocketMonsters.Count; i++)
        {
            Button button = Instantiate(hoverableButtonPrefab);
            buttons.Add(button);

            SetButtonPosition(button, i, playerPocketMonsters.Count);

            button.GetComponent<HoverableUiElement>().SetYSize(2.5f);
            button.GetComponent<HoverableUiElement>().SetText(SetPocketMonstersInformation(playerPocketMonsters[i], false));
            button.GetComponentInChildren<Text>().text = playerPocketMonsters[i].stats.name;
            SetAddItemToPocketMonster(i, playerPocketMonsters[i], item, player);
        }
    }

    private void SetAddItemToPocketMonster(int index, PocketMonster pocketMonster, PocketMonsterItem item, GameObject player)
    {
        buttons[index].onClick.AddListener(() => AddItemToPocketMonster(index, pocketMonster, item, player));
    }

    private void AddItemToPocketMonster(int index, PocketMonster pocketMonster, PocketMonsterItem item, GameObject player)
    {
        if (pocketMonster.items.Count >= maxAmountOfItems)
        {
            ClearButtons();
            CreateItemDeletion(pocketMonster, item, player);
        }
        else
        {
            PocketMonsterItem addedItem = (PocketMonsterItem)System.Activator.CreateInstance(item.GetType());
            addedItem.SetStats();
            pocketMonster.items.Add(addedItem);
            EndConversation(player);
        }
    }

    private void CreateItemDeletion(PocketMonster pocketMonster, PocketMonsterItem item, GameObject player)
    {
        InstantiateText(false);
        information.text = "Remove item from pocketmonster?";
        for (int i = 0; i < 2; i++)
        {
            CreateYesNoMenu(i);
            SetChooseIfWantToRemoveItem(i, pocketMonster, item, player);
        }
    }

    private void SetChooseIfWantToRemoveItem(int index, PocketMonster pocketMonster, PocketMonsterItem item, GameObject player)
    {
        buttons[index].onClick.AddListener(() => ChooseIfWantToRemoveItem(index, pocketMonster, item, player));
    }

    private void ChooseIfWantToRemoveItem(int index, PocketMonster pocketMonster, PocketMonsterItem item, GameObject player)
    {
        if (index == 0)
        {
            ClearButtons();
            CreateRemoveItemFromPocketMonster(pocketMonster, item, player);
        }
        else
        {
            ClearButtons();
            CreateCheckIfWantToEndConversation(player, item);
        }
    }

    private void CreateRemoveItemFromPocketMonster(PocketMonster pocketMonster, PocketMonsterItem item, GameObject player)
    {
        InstantiateText(true);
        information.text = "What item to remove for " + item.name + "?";
        information.GetComponent<HoverableUiElement>().SetText(item.itemDescription);

        for (int i = 0; i < pocketMonster.items.Count; i++)
        {
            Button button = Instantiate(hoverableButtonPrefab);
            buttons.Add(button);

            SetButtonPosition(button, i, 3);

            button.GetComponent<HoverableUiElement>().SetText(pocketMonster.items[i].itemDescription);
            button.GetComponentInChildren<Text>().text = pocketMonster.items[i].name;
            SetRemoveItemFromPocketMonster(i, pocketMonster, item, player);
        }
    }

    private void SetRemoveItemFromPocketMonster(int index, PocketMonster pocketMonster, PocketMonsterItem item, GameObject player)
    {
        buttons[index].onClick.AddListener(() => RemoveItemFromPocketMonster(index, pocketMonster, item, player));
    }

    private void RemoveItemFromPocketMonster(int index, PocketMonster pocketMonster, PocketMonsterItem item, GameObject player)
    {
        pocketMonster.items.RemoveAt(index);
        PocketMonsterItem addedItem = (PocketMonsterItem)System.Activator.CreateInstance(item.GetType());
        addedItem.SetStats();
        pocketMonster.items.Add(addedItem);
        EndConversation(player);
    }

    private void CreateCheckIfWantToEndConversation(GameObject player, PocketMonsterItem item)
    {
        InstantiateText(false);
        information.text = "End conversation?";
        for (int i = 0; i < 2; i++)
        {
            CreateYesNoMenu(i);
            SetCheckIfWantToEndConversation(i, player, item);
        }
    }

    private void SetCheckIfWantToEndConversation(int index, GameObject player, PocketMonsterItem item)
    {
        buttons[index].onClick.AddListener(() => CheckIfWantToEndConversation(index, player, item));
    }

    private void CheckIfWantToEndConversation(int index, GameObject player, PocketMonsterItem item)
    {
        if (index == 0)
        {
            EndConversation(player);
        }
        else
        {
            ClearButtons();
            CreatePocketMonsterMenuForItemToGive(item, player);
        }
    }

    public void CreatePickATeamBuffMenu(GameObject player, List<PocketMonsterItem> randomListOfTeamBuffs = null)
    {
        if (randomListOfTeamBuffs == null)
        {
            randomListOfTeamBuffs = new List<PocketMonsterItem>();
            for (int i = 0; i < amountOfPicks; i++)
            {
                PocketMonsterItem randomTeamBuff = ChooseRandomTeamBuff(randomListOfTeamBuffs);
                while (randomTeamBuff == null)
                {
                    randomTeamBuff = ChooseRandomTeamBuff(randomListOfTeamBuffs);
                }

                randomListOfTeamBuffs.Add(randomTeamBuff);
            }
        }


        if (teamBuffsOfPlayer.Count >= maxAmountOfTeamBuffs)
        {
            CreateAskIfWantToRemoveTeamBuff(player, randomListOfTeamBuffs);
            return;
        }

        InstantiateText(false);
        information.text = "Pick a teambuff:";

        for (int i = 0; i < amountOfPicks; i++)
        {
            Button button = Instantiate(hoverableButtonPrefab);
            buttons.Add(button);
            SetButtonPosition(button, i, 3);

            button.GetComponent<HoverableUiElement>().SetText(randomListOfTeamBuffs[i].itemDescription);
            button.GetComponentInChildren<Text>().text = randomListOfTeamBuffs[i].name;

            SetTeamBuffAddButton(i, randomListOfTeamBuffs, player);
        }
    }

    private void SetTeamBuffAddButton(int index, List<PocketMonsterItem> randomListOfTeamBuffs, GameObject player)
    {
        buttons[index].onClick.AddListener(() => OnTeamBuffAddButton(index, randomListOfTeamBuffs, player));
    }

    private void OnTeamBuffAddButton(int index, List<PocketMonsterItem> randomListOfTeamBuffs, GameObject player)
    {
        PocketMonsterItem addedTeamBuff = (PocketMonsterItem)System.Activator.CreateInstance(randomListOfTeamBuffs[index].GetType());
        addedTeamBuff.SetStats();
        teamBuffsOfPlayer.Add(addedTeamBuff);
        EndConversation(player);
    }

    private void CreateAskIfWantToRemoveTeamBuff(GameObject player, List<PocketMonsterItem> randomListOfTeamBuffs)
    {
        InstantiateText(false);
        information.text = "Remove teambuff for:";

        GameObject canvas = GameObject.Find("Canvas");

        Image bg = Instantiate(imagePrefab);
        bg.transform.SetParent(canvas.transform);
        bgsForTexts.Add(bg);

        for (int i = 0; i < randomListOfTeamBuffs.Count; i++)
        {
            Text teamBuffInfo = Instantiate(textPrefab);
            teamBuffInfo.color = Color.white;

            SetExtraInfoPos(canvas, teamBuffInfo, bg, i, false);

            teamBuffInfo.text += randomListOfTeamBuffs[i].name + ": " + randomListOfTeamBuffs[i].itemDescription;
        }

        for (int i = 0; i < 2; i++)
        {
            CreateYesNoMenu(i);
            SetAskRemoveTeambuff(i, player, randomListOfTeamBuffs);
        }
    }

    private void SetAskRemoveTeambuff(int index, GameObject player, List<PocketMonsterItem> randomListOfTeamBuffs)
    {
        buttons[index].onClick.AddListener(() => AskRemoveTeambuff(index, player, randomListOfTeamBuffs));
    }

    private void AskRemoveTeambuff(int index, GameObject player, List<PocketMonsterItem> randomListOfTeamBuffs)
    {
        if (index == 0)
        {
            ClearButtons();
            CreateRemoveTeamBuff(player, randomListOfTeamBuffs);
        }
        else
        {
            EndConversation(player);
        }
    }

    private void CreateRemoveTeamBuff(GameObject player, List<PocketMonsterItem> randomListOfTeamBuffs)
    {
        InstantiateText(false);
        information.text = "What teambuff to remove?";
        for (int i = 0; i < teamBuffsOfPlayer.Count; i++)
        {
            Button button = Instantiate(hoverableButtonPrefab);
            buttons.Add(button);

            SetButtonPosition(button, i, 3);

            button.GetComponent<HoverableUiElement>().SetText(teamBuffsOfPlayer[i].itemDescription);
            button.GetComponentInChildren<Text>().text = teamBuffsOfPlayer[i].name;
            SetRemoveTeamBuff(i, player, randomListOfTeamBuffs);
        }
    }

    private void SetRemoveTeamBuff(int index, GameObject player, List<PocketMonsterItem> randomListOfTeamBuffs)
    {
        buttons[index].onClick.AddListener(() => RemoveTeamBuff(index, player, randomListOfTeamBuffs));
    }

    private void RemoveTeamBuff(int index, GameObject player, List<PocketMonsterItem> randomListOfTeamBuffs)
    {
        teamBuffsOfPlayer.RemoveAt(index);
        ClearButtons();
        CreatePickATeamBuffMenu(player, randomListOfTeamBuffs);
    }

    private void CreateYesNoMenu(int index)
    {
        Button button = Instantiate(buttonPrefab);
        buttons.Add(button);
        SetButtonPosition(button, index, 2);

        if (index == 0)
        {
            button.GetComponentInChildren<Text>().text = "Yes";
        }
        else
        {
            button.GetComponentInChildren<Text>().text = "No";
        }
    }

    private void EndConversation(GameObject player)
    {
        player.GetComponent<PlayerMovement>().enabled = true;
        ClearButtons();
        ClearMergants();
    }

    public void ClearMergants()
    {
        for (int i = 0; i < mergants.Count; i++)
        {
            Destroy(mergants[i].gameObject);
        }

        currentItemsHandedOut.Clear();
        currentMovesHandedOut.Clear();
        mergants.Clear();
    }

    private PocketMonsterMoves ChooseRandomPocketMonsterMove(List<PocketMonsterMoves> randomListOfPocketMonstersMoves, PocketMonster addedPocketMonster,
        List<PocketMonsterMoves> listToChooseFrom)
    {
        PocketMonsterMoves randomPocketMonsterMove = listToChooseFrom[Random.Range(0, listToChooseFrom.Count)];
        List<string> moveTypes = new List<string>(); 
        for (int i = 0; i < addedPocketMonster.moves.Count; i++)
        {
            string moveType = addedPocketMonster.moves[i].GetType().ToString();
            moveTypes.Add(moveType);
        }

        if (randomListOfPocketMonstersMoves.Contains(randomPocketMonsterMove) || moveTypes.Contains(randomPocketMonsterMove.GetType().ToString()))
        {
            return null;
        }
        else
        {
            return randomPocketMonsterMove;
        }
    }

    private PocketMonster ChooseRandomPocketMonster(List<PocketMonster> randomListOfPocketMonsters)
    {
        PocketMonster randomPocketMonster = allPocketMonsters[Random.Range(0, allPocketMonsters.Count)];

        if (randomListOfPocketMonsters.Contains(randomPocketMonster))
        {
            return null;
        } else
        {
            return randomPocketMonster;
        }
    }

    private void InstantiateText(bool createHoverText)
    {
        if (createHoverText)
        {
            information = Instantiate(hoverText);
            information.GetComponent<HoverableUiElement>().showUnder = true;
        }
        else
        {
            information = Instantiate(textPrefab);
        }

        GameObject canvas = GameObject.Find("Canvas");
        information.transform.SetParent(canvas.transform);

        Vector2 size = Vector2.zero;
        size.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 12;
        size.x = canvas.GetComponent<RectTransform>().sizeDelta.x / 2;
        information.GetComponent<RectTransform>().sizeDelta = size;

        Vector3 pos = Vector3.zero;
        pos.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 2 - size.y / 2;
        information.GetComponent<RectTransform>().localPosition = pos;

        information.alignment = TextAnchor.MiddleCenter;
    }

    private void SetExtraInfoPos(GameObject canvas, Text info, Image bg, int index, bool setExtraYSize)
    {
        info.transform.SetParent(canvas.transform);

        Vector2 size = Vector2.zero;
        size.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 1.75f;
        size.x = information.rectTransform.sizeDelta.x / 1.75f;
        info.rectTransform.sizeDelta = size;

        Vector3 pos = Vector3.zero;
        pos.y = information.rectTransform.localPosition.y - information.rectTransform.sizeDelta.y / 2 - size.y / 2;
        pos.x = information.rectTransform.localPosition.x - size.x + (index * size.x) + (index * size.x * 0.1f);
        info.rectTransform.localPosition = pos;

        if (index == 1)
        {
            if (setExtraYSize)
            {
                float addedY = (size.y / 11) * 2;
                size.y += addedY;
                pos.y -= addedY / 2;
            }

            bg.rectTransform.sizeDelta = new Vector2(size.x * 3 + size.x * 0.2f, size.y);
            bg.rectTransform.localPosition = pos;
        }
        extraTexts.Add(info);
    }

    private void CreatePocketMonsterInformation(GameObject canvas, List<PocketMonster> randomListOfPocketMonsters, PlayerBattle player)
    {
        Image bg = Instantiate(imagePrefab);
        bg.transform.SetParent(canvas.transform);
        bgsForTexts.Add(bg);

        for (int i = 0; i < randomListOfPocketMonsters.Count; i++)
        {
            Text pocketMonsterInfo = Instantiate(textPrefab);

            SetExtraInfoPos(canvas, pocketMonsterInfo, bg, i, true);

            pocketMonsterInfo.text += "Pocketmonster" + (i + 1) + ":\n" + SetPocketMonstersInformation(randomListOfPocketMonsters[i], true);
            pocketMonsterInfo.color = Color.white;

            for (int j = 0; j < randomListOfPocketMonsters[i].possibleAbilitys.Count; j++)
            {
                randomListOfPocketMonsters[i].possibleAbilitys[j].SetAbilityStats(player);
                Text abilityInfo = Instantiate(hoverText);
                extraTexts.Add(abilityInfo);
                abilityInfo.transform.SetParent(canvas.transform);

                Vector2 size = Vector2.zero;
                size.x = pocketMonsterInfo.rectTransform.sizeDelta.x;
                size.y = pocketMonsterInfo.rectTransform.sizeDelta.y / 11;
                abilityInfo.rectTransform.sizeDelta = size;

                Vector3 pos = Vector3.zero;
                pos.x = pocketMonsterInfo.rectTransform.localPosition.x;
                pos.y = pocketMonsterInfo.rectTransform.localPosition.y - pocketMonsterInfo.rectTransform.sizeDelta.y / 2 - size.y / 2 - size.y * j;
                abilityInfo.rectTransform.localPosition = pos;

                abilityInfo.text = randomListOfPocketMonsters[i].possibleAbilitys[j].abilityName;
                abilityInfo.GetComponent<HoverableUiElement>().SetText(randomListOfPocketMonsters[i].possibleAbilitys[j].abilityDescription);
                abilityInfo.alignment = TextAnchor.MiddleLeft;
                abilityInfo.color = Color.white;
            }
        }
    }

    private string SetPocketMonstersInformation(PocketMonster pocketMonster, bool createPossibleAbilityText)
    {
        string typing = "";

        for (int i = 0; i < pocketMonster.stats.typing.Count; i++)
        {
            if (i < pocketMonster.stats.typing.Count - 1)
            {
                typing += " " + pocketMonster.stats.typing[i] + ",";
            }
            else
            {
                typing += " " + pocketMonster.stats.typing[i];
            }
        }

        string text = "Name: " + pocketMonster.stats.name + "\nTyping:" + typing + "\nHealth: " + pocketMonster.stats.maxHealth + 
            "\nAttack: " + pocketMonster.stats.baseAttack + "\nDefense: " + pocketMonster.stats.baseDefense + "\nSpecial attack: " + 
            pocketMonster.stats.baseSpecialAttack + "\nSpecial defense: " + pocketMonster.stats.baseSpecialDefense + "\nSpeed: " + 
            pocketMonster.stats.baseSpeed + "\nCritchance: " + pocketMonster.stats.critChance + "%";

        if (createPossibleAbilityText)
        {
            text += "\nPossible abilities:";
        }

        return text;
    }

    private void SetButtonPosition(Button button, int index, float xSize)
    {
        GameObject canvas = GameObject.Find("Canvas");
        button.transform.SetParent(canvas.transform);

        Vector2 size = Vector2.zero;
        size.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 10;
        size.x = canvas.GetComponent<RectTransform>().sizeDelta.x / xSize;
        button.GetComponent<RectTransform>().sizeDelta = size;

        Vector3 pos = Vector3.zero;
        pos.y = -canvas.GetComponent<RectTransform>().sizeDelta.y / 2 + size.y / 2;
        pos.x = -canvas.GetComponent<RectTransform>().sizeDelta.x / 2 + size.x / 2 + index * size.x;
        button.GetComponent<RectTransform>().localPosition = pos;
    }

    private void ClearButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            Destroy(buttons[i].gameObject);
        }
        buttons.Clear();

        for (int i = 0; i < extraTexts.Count; i++)
        {
            Destroy(extraTexts[i].gameObject);
        }
        extraTexts.Clear();

        for (int i = 0; i < bgsForTexts.Count; i++)
        {
            Destroy(bgsForTexts[i].gameObject);
        }
        bgsForTexts.Clear();

        Destroy(information.gameObject);
    }

    public void FillItemList()
    {
        for (int i = 0; i < pickups.Count; i++)
        {
            if (pickups[i] == PickupsInGauntlet.Item)
            {
                List<PocketMonsterItem> randomListOfItems = new List<PocketMonsterItem>();
                for (int j = 0; j < 3; j++)
                {
                    PocketMonsterItem randomItem = ChooseRandomItem(randomListOfItems);
                    while (randomItem == null)
                    {
                        randomItem = ChooseRandomItem(randomListOfItems);
                    }

                    randomListOfItems.Add(randomItem);
                }
                itemsToHandOut.AddRange(randomListOfItems);
            }
        }
    }

    public void FillMoveList(int amountOfRounds, int amountOfMovesPerRound)
    {
        for (int i = 0; i < amountOfRounds; i++)
        {
            AddRandomMovesToHandoutList(amountOfMovesPerRound);
        }
    }

    private void AddRandomMovesToHandoutList(int amount)
    {
        List<PocketMonsterMoves> randomListOfMoves = new List<PocketMonsterMoves>();
        List<PocketMonsterMoves> listToChooseFrom = new List<PocketMonsterMoves>();

        for (int i = 0; i < playerPocketMonsters.Count; i++)
        {
            List<PocketMonsterMoves> pocketMonsterSpecifickList = new List<PocketMonsterMoves>();
            pocketMonsterSpecifickList = GetMovesToChooseFrom(pocketMonsterSpecifickList, playerPocketMonsters[i]);
            for (int j = 0; j < pocketMonsterSpecifickList.Count; j++)
            {
                if (!listToChooseFrom.Contains(pocketMonsterSpecifickList[j]))
                {
                    listToChooseFrom.Add(pocketMonsterSpecifickList[j]);
                }
            }
        }

        for (int i = 0; i < amount; i++)
        {
            PocketMonsterMoves randomMove = ChooseRandomMove(randomListOfMoves, listToChooseFrom);
            while (randomMove == null)
            {
                randomMove = ChooseRandomMove(randomListOfMoves, listToChooseFrom);
            }

            randomListOfMoves.Add(randomMove);
        }
        movesToHandOut.AddRange(randomListOfMoves);
        allMovesHandedOut.AddRange(randomListOfMoves);
    }

    public List<PocketMonsterMoves> GetMovesToChooseFrom(List<PocketMonsterMoves> listToChooseFrom, PocketMonster pocketMonster)
    {
        List<PocketMonsterMoves.TypeOfBoostMove> typesOfBoostAccepted = new List<PocketMonsterMoves.TypeOfBoostMove>();
        
        if (pocketMonster.stats.baseSpeed >= 50)
        {
            typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.Speed);
        }

        if (pocketMonster.stats.baseDefense >= 65)
        {
            typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.PhysicalDefensive);
            typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.GeneralDefense);
            typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.Recovery);
        }

        if (pocketMonster.stats.baseSpecialDefense >= 65)
        {
            typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.SpecialDefensive);
            typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.GeneralDefense);
            typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.Recovery);
        }

        if (pocketMonster.stats.maxHealth >= 400)
        {
            typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.PhysicalDefensive);
            typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.SpecialDefensive);
            typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.GeneralDefense);
            typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.Recovery);
        }

        for (int i = 0; i < allPocketMonstersMoves.Count; i++)
        {
            if (allPocketMonstersMoves[i].moveSort != PocketMonsterMoves.MoveSort.Status)
            {
                if (pocketMonster.stats.baseAttack >= 100 && pocketMonster.stats.baseSpecialAttack >= 100)
                {
                    if (allPocketMonstersMoves[i].baseDamage >= 65)
                    {
                        listToChooseFrom.Add(allPocketMonstersMoves[i]);
                    }

                    if (!typesOfBoostAccepted.Contains(PocketMonsterMoves.TypeOfBoostMove.PhysicalOffensive))
                    {
                        typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.PhysicalOffensive);
                        typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.GeneralOffense);
                        typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.SpecialOffensive);
                    }
                }
                else
                {
                    if (pocketMonster.stats.baseAttack > pocketMonster.stats.baseSpecialAttack)
                    {
                        if (pocketMonster.stats.baseSpecialAttack + 15 >= pocketMonster.stats.baseAttack)
                        {
                            listToChooseFrom.Add(allPocketMonstersMoves[i]);

                            if (!typesOfBoostAccepted.Contains(PocketMonsterMoves.TypeOfBoostMove.PhysicalOffensive))
                            {
                                typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.PhysicalOffensive);
                                typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.GeneralOffense);
                                typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.SpecialOffensive);
                            }
                        }
                        else
                        {
                            if (allPocketMonstersMoves[i].moveSort == PocketMonsterMoves.MoveSort.Physical)
                            {
                                listToChooseFrom.Add(allPocketMonstersMoves[i]);
                            }

                            if (!typesOfBoostAccepted.Contains(PocketMonsterMoves.TypeOfBoostMove.PhysicalOffensive))
                            {
                                typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.PhysicalOffensive);
                            }
                        }
                    }
                    else
                    {
                        if (pocketMonster.stats.baseAttack + 15 >= pocketMonster.stats.baseSpecialAttack)
                        {
                            listToChooseFrom.Add(allPocketMonstersMoves[i]);

                            if (!typesOfBoostAccepted.Contains(PocketMonsterMoves.TypeOfBoostMove.PhysicalOffensive))
                            {
                                typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.PhysicalOffensive);
                                typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.GeneralOffense);
                                typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.SpecialOffensive);
                            }
                        }
                        else
                        {
                            if (allPocketMonstersMoves[i].moveSort == PocketMonsterMoves.MoveSort.Special)
                            {
                                listToChooseFrom.Add(allPocketMonstersMoves[i]);
                            }

                            if (!typesOfBoostAccepted.Contains(PocketMonsterMoves.TypeOfBoostMove.SpecialOffensive))
                            {
                                typesOfBoostAccepted.Add(PocketMonsterMoves.TypeOfBoostMove.SpecialOffensive);
                            }
                        }
                    }
                }
            }
            else
            {
                if (typesOfBoostAccepted.Contains(allPocketMonstersMoves[i].typeOfBoost))
                {
                    listToChooseFrom.Add(allPocketMonstersMoves[i]);
                }
            }
        }

        return listToChooseFrom;
    }

    public List<PocketMonsterMoves> CreateListForStabMoveForPocketMonster(PocketMonster addedPocketMonster, List<PocketMonsterMoves> specifickMoveList)
    {
        for (int i = 0; i < allPocketMonstersMoves.Count; i++)
        {
            for (int j = 0; j < addedPocketMonster.stats.typing.Count; j++)
            {
                if (allPocketMonstersMoves[i].moveType == addedPocketMonster.stats.typing[j])
                {
                    if (addedPocketMonster.stats.baseAttack > addedPocketMonster.stats.baseSpecialAttack)
                    {
                        if (allPocketMonstersMoves[i].moveSort == PocketMonsterMoves.MoveSort.Physical)
                        {
                            specifickMoveList = CheckForCertainStatsToAddMove(addedPocketMonster.stats.baseAttack, allPocketMonstersMoves[i], specifickMoveList);
                        }
                    }
                    else if (addedPocketMonster.stats.baseSpecialAttack > addedPocketMonster.stats.baseAttack)
                    {
                        if (allPocketMonstersMoves[i].moveSort == PocketMonsterMoves.MoveSort.Special)
                        {
                            specifickMoveList = CheckForCertainStatsToAddMove(addedPocketMonster.stats.baseSpecialAttack, allPocketMonstersMoves[i], specifickMoveList);
                        }
                    }
                    else
                    {
                        if (allPocketMonstersMoves[i].moveSort != PocketMonsterMoves.MoveSort.Status)
                        {
                            specifickMoveList = CheckForCertainStatsToAddMove(addedPocketMonster.stats.baseAttack, allPocketMonstersMoves[i], specifickMoveList);
                        }
                    }
                }
            }
        }

        return specifickMoveList;
    }

    private List<PocketMonsterMoves> CheckForCertainStatsToAddMove(float baseStat, PocketMonsterMoves move, List<PocketMonsterMoves> specifickMoveList)
    {
        if (baseStat >= 100)
        {
            if (move.baseDamage >= 65)
            {
                specifickMoveList.Add(move);
            }
        }
        else
        {
            specifickMoveList.Add(move);
        }

        return specifickMoveList;
    }

    private PocketMonsterItem ChooseRandomItem(List<PocketMonsterItem> randomListOfItems)
    {
        PocketMonsterItem randomItem = allPocketMonsterItems[Random.Range(0, allPocketMonsterItems.Count)];

        if (randomListOfItems.Contains(randomItem))
        {
            return null;
        }
        else
        {
            return randomItem;
        }
    }

    public PocketMonsterMoves ChooseRandomMove(List<PocketMonsterMoves> randomListOfMoves, List<PocketMonsterMoves> listToChooseFrom)
    {
        PocketMonsterMoves randomPocketMonsterMove = listToChooseFrom[Random.Range(0, listToChooseFrom.Count)];
        List<string> moveTypes = new List<string>();
        for (int i = 0; i < randomListOfMoves.Count; i++)
        {
            string moveType = randomListOfMoves[i].GetType().ToString();
            moveTypes.Add(moveType);
        }

        if (moveTypes.Contains(randomPocketMonsterMove.GetType().ToString()))
        {
            return null;
        }
        else
        {
            return randomPocketMonsterMove;
        }
    }

    private PocketMonsterItem ChooseRandomTeamBuff(List<PocketMonsterItem> randomListOfTeamBuffs)
    {
        PocketMonsterItem randomTeamBuff = allTeamBuffs[Random.Range(0, allTeamBuffs.Count)];

        if (randomListOfTeamBuffs.Contains(randomTeamBuff))
        {
            return null;
        }
        else
        {
            return randomTeamBuff;
        }
    }

    public void HandleLives()
    {
        lives--;
        livesText.text = "Lives: " + lives;
        if (lives <= 0)
        {
            SwitchToEndScreen(false);
        }
    }

    public void SwitchToEndScreen(bool playerWon)
    {
        this.playerWon = playerWon;
        SaveSytem.DeleteGame();
        SceneManager.LoadScene(endScreenName);
    }

    public void EnableWorldUI(bool enabled)
    {
        livesText.gameObject.SetActive(enabled);
        exitGameButton.gameObject.SetActive(enabled);

        if (exitGameButton.GetComponent<ExitGameButton>().menuIsEnabled)
        {
            exitGameButton.GetComponent<ExitGameButton>().EnableMenu();
        }
    }

    private void SaveData()
    {
        playerPocketMonsterInInt.Clear();
        playerPocketMonstersAbilityInInt.Clear();
        playerPocketMonsterItemsInInt.Clear();
        playerPocketMonsterMovesInInt.Clear();

        for (int i = 0; i < playerPocketMonsters.Count; i++)
        {
            string pocketMonsterName = playerPocketMonsters[i].name;
            string pocketMonsterNumberString = "";
            int pocketMonsterNumber = 0;

            for (int j = 0; j < pocketMonsterName.Length; j++)
            {
                if (char.IsDigit(pocketMonsterName[j]))
                {
                    pocketMonsterNumberString += pocketMonsterName[j];
                }
            }

            if (pocketMonsterNumberString.Length > 0)
            {
                pocketMonsterNumber = int.Parse(pocketMonsterNumberString);
            }

            playerPocketMonsterInInt.Add(pocketMonsterNumber - 1);

            playerPocketMonstersAbilityInInt.Add(playerPocketMonsters[i].possibleAbilitys.IndexOf(playerPocketMonsters[i].chosenAbility));

            List<int> pocketMonsterItems = new List<int>();
            for (int j = 0; j < playerPocketMonsters[i].items.Count; j++)
            {
                int sameTypeValue = 0;
                for (int k = 0; k < allPocketMonsterItems.Count; k++)
                {
                    if (playerPocketMonsters[i].items[j].GetType() == allPocketMonsterItems[k].GetType())
                    {
                        sameTypeValue = k;
                    }
                }
                pocketMonsterItems.Add(sameTypeValue);
            }
            playerPocketMonsterItemsInInt.Add(pocketMonsterItems);

            List<int> pocketMonsterMoves = new List<int>();
            for (int j = 0; j < playerPocketMonsters[i].moves.Count; j++)
            {
                int sameTypeValue = 0;
                for (int k = 0; k < allPocketMonstersMoves.Count; k++)
                {
                    if (playerPocketMonsters[i].moves[j].GetType() == allPocketMonstersMoves[k].GetType())
                    {
                        sameTypeValue = k;
                    }
                }
                pocketMonsterMoves.Add(sameTypeValue);
            }
            playerPocketMonsterMovesInInt.Add(pocketMonsterMoves);
        }

        TranslateItemsToInt(teamBuffsOfPlayerInInt, teamBuffsOfPlayer, allTeamBuffs);
        TranslateMovesToInt(allMovesHandedOutInInt, allMovesHandedOut);
        TranslateItemsToInt(itemsToHandOutInInt, itemsToHandOut, allPocketMonsterItems);
        TranslateMovesToInt(movesToHandOutInInt, movesToHandOut);
        TranslateItemsToInt(currentItemsHandedOutInInt, currentItemsHandedOut, allPocketMonsterItems);
        TranslateMovesToInt(currentMovesHandedOutInInt, currentMovesHandedOut);
        SaveSytem.SaveGame(playerBattle.gameObject, this, managerOfEnemys, managerOfTheTerrains);

        TweenSavingTextColor();
    }

    private void TweenSavingTextColor()
    {
        canSave = false;
        savingText.color = savingTextStartColor;
        iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "time", 1, "onupdate", "UpdateSavingTextColor",
            "oncomplete", "SetToCanSaveAgain", "oncompletetarget", gameObject));
    }

    private void UpdateSavingTextColor(float val)
    {
        Color32 newColor = savingText.color;
        newColor.r = (byte)(((1f - val) * savingTextStartColor.r) + (val * savingTextEndColor.r));
        newColor.b = (byte)(((1f - val) * savingTextStartColor.b) + (val * savingTextEndColor.b));
        newColor.g = (byte)(((1f - val) * savingTextStartColor.g) + (val * savingTextEndColor.g));
        newColor.a = (byte)(((1f - val) * savingTextStartColor.a) + (val * savingTextEndColor.a));
        savingText.color = newColor;
    }

    private void SetToCanSaveAgain()
    {
        canSave = true;
    }

    private void TranslateItemsToInt(List<int> intItemList, List<PocketMonsterItem> actualItemList, List<PocketMonsterItem> itemListToSearchThrough)
    {
        intItemList.Clear();
        for (int i = 0; i < actualItemList.Count; i++)
        {
            for (int j = 0; j < itemListToSearchThrough.Count; j++)
            {
                if (actualItemList[i].GetType() == itemListToSearchThrough[j].GetType())
                {
                    intItemList.Add(j);
                }
            }
        }
    }

    private void TranslateMovesToInt(List<int> intMovesList, List<PocketMonsterMoves> actualMovesList)
    {
        intMovesList.Clear();
        for (int i = 0; i < actualMovesList.Count; i++)
        {
            for (int j = 0; j < allPocketMonstersMoves.Count; j++)
            {
                if (actualMovesList[i].GetType() == allPocketMonstersMoves[j].GetType())
                {
                    intMovesList.Add(j);
                }
            }
        }
    }

    private void RecoverMoves(List<int> data, List<PocketMonsterMoves> movesToRecover)
    {
        movesToRecover.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            movesToRecover.Add(allPocketMonstersMoves[data[i]]);
        }
    }

    private void RecoverItems(List<int> data, List<PocketMonsterItem> itemsToRecover, List<PocketMonsterItem> itemListToSearchThrough)
    {
        itemsToRecover.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            itemsToRecover.Add(itemListToSearchThrough[data[i]]);
        }
    }

    private void FillPocketMonsterMoveList()
    {
        ChemicalBarrage chemicalBarrage = new ChemicalBarrage();
        ChemicalJab chemicalJab = new ChemicalJab();
        ChemicalInjection chemicalInjection = new ChemicalInjection();
        ChemicalStorm chemicalStorm = new ChemicalStorm();

        Ember ember = new Ember();
        FireTouch fireTouch = new FireTouch();
        SmokeCloud smokeCloud = new SmokeCloud();
        FireBlast fireBlast = new FireBlast();

        GrassBlast grassBlast = new GrassBlast();
        VineWhip vineWhip = new VineWhip();
        WoodHammer woodHammer = new WoodHammer();
        LeechBlast leechBlast = new LeechBlast();

        LightBlast lightBlast = new LightBlast();
        LightJab lightJab = new LightJab();
        BlindingLight blindingLight = new BlindingLight();
        SunRay sunRay = new SunRay();

        Magnitude magnitude = new Magnitude();
        EarthPower earthPower = new EarthPower();
        ForceOfNature forceOfNature = new ForceOfNature();
        GroundTrap groundTrap = new GroundTrap();

        SpookBall spookBall = new SpookBall();
        SpookPunch spookPunch = new SpookPunch();
        NetherRay netherRay = new NetherRay();
        CursedTouch cursedTouch = new CursedTouch();

        Tackle tackle = new Tackle();
        MindTrick mindTrick = new MindTrick();
        DrainPunch drainPunch = new DrainPunch();
        MentalDrain mentalDrain = new MentalDrain();

        ThunderShock thunderShock = new ThunderShock();
        ThunderPunch thunderPunch = new ThunderPunch();
        ThunderBolt thunderBolt = new ThunderBolt();
        ParalyzingTouch paralyzingTouch = new ParalyzingTouch();
        
        WaterGun waterGun = new WaterGun();
        WaterSlap waterSlap = new WaterSlap();
        DrownShot drownShot = new DrownShot();
        HydroSlap hydroSlap = new HydroSlap();

        WindBlast windBlast = new WindBlast();
        AerialAssualt aerialAssualt = new AerialAssualt();
        ToTheSkies toTheSkies = new ToTheSkies();
        SkyStrike skyStrike = new SkyStrike();

        Agility agility = new Agility();
        ArmorUp armorUp = new ArmorUp();
        MentalFortification mentalFortification = new MentalFortification();
        RaisePhysique raisePhysique = new RaisePhysique();
        Recover recover = new Recover();
        RaiseMental raiseMental = new RaiseMental();
        DefensiveTraining defensiveTraining = new DefensiveTraining();
        OffensiveTraining offensiveTraining = new OffensiveTraining();

        PocketMonsterMoves[] pocketMonsterMoves = { chemicalBarrage, ember, fireTouch, grassBlast, lightBlast, magnitude, spookBall, tackle,
            thunderShock, vineWhip, waterGun, waterSlap, windBlast, chemicalJab, lightJab, earthPower, spookPunch, mindTrick, thunderPunch,
            aerialAssualt, toTheSkies, skyStrike, chemicalStorm, chemicalInjection, groundTrap, forceOfNature, smokeCloud, fireBlast, woodHammer,
            leechBlast, blindingLight, sunRay, netherRay, cursedTouch, drainPunch, mentalDrain, thunderBolt, paralyzingTouch, drownShot, hydroSlap,
            agility, armorUp, mentalFortification, raiseMental, raisePhysique, recover, defensiveTraining, offensiveTraining};

        allPocketMonstersMoves.AddRange(pocketMonsterMoves);

        for (int i = 0; i < allPocketMonstersMoves.Count; i++)
        {
            allPocketMonstersMoves[i].SetMoveStats();
        }
    }

    private void FillPocketMonsterItemList()
    {
        AttackBooster attackBooster = new AttackBooster();
        Leftovers leftovers = new Leftovers();
        JuggerNaut juggerNaut = new JuggerNaut();
        SpecialAttackBooster specialAttackBooster = new SpecialAttackBooster();
        ArmorOrb armorOrb = new ArmorOrb();
        BottledCurse bottledCurse = new BottledCurse();
        InsightOrb insightOrb = new InsightOrb();
        MuscleAmplifier muscleAmplifier = new MuscleAmplifier();
        VampericOrb vampericOrb = new VampericOrb();
        PowerPyramid powerPyramid = new PowerPyramid();
        ScopeLens scopeLens = new ScopeLens();
        LuckyCharm luckyCharm = new LuckyCharm();
        StabBooster stabBooster = new StabBooster();
        CleansingSpray cleansingSpray = new CleansingSpray();
        NeutralizingSpray neutralizingSpray = new NeutralizingSpray();
        GravityPotion gravityPotion = new GravityPotion();
        Jetpack jetpack = new Jetpack();
        WeaknessModifier weaknessModifier = new WeaknessModifier();
        ResistanceModifier resistanceModifier = new ResistanceModifier();
        WonderPotion wonderPotion = new WonderPotion();

        PocketMonsterItem[] pocketMonsterItems = { attackBooster, leftovers, juggerNaut, specialAttackBooster, armorOrb, bottledCurse, insightOrb,
        muscleAmplifier, vampericOrb, powerPyramid, scopeLens, luckyCharm, stabBooster, cleansingSpray, neutralizingSpray, gravityPotion, jetpack,
        weaknessModifier, resistanceModifier, wonderPotion};

        allPocketMonsterItems.AddRange(pocketMonsterItems);

        for (int i = 0; i < allPocketMonsterItems.Count; i++)
        {
            allPocketMonsterItems[i].SetStats();
        }
    }

    private void FillTeamBuffsList()
    {
        SoMuchFood soMuchFood = new SoMuchFood();
        DrainLife drainLife = new DrainLife();
        Accelerator accelerator = new Accelerator();
        MuscleBand muscleBand = new MuscleBand();
        MindBand mindBand = new MindBand();
        MindfulBand mindfulBand = new MindfulBand();
        ArmorBand armorBand = new ArmorBand();
        CriticalDefenses criticalDefenses = new CriticalDefenses();
        CriticalEyeSight criticalEyeSight = new CriticalEyeSight();
        RingOfSickness ringOfSickness = new RingOfSickness();

        PocketMonsterItem[] teamBuffs = { soMuchFood, drainLife, accelerator, muscleBand, mindBand, mindfulBand, armorBand,
        criticalEyeSight, criticalDefenses, ringOfSickness};

        allTeamBuffs.AddRange(teamBuffs);

        for (int i = 0; i < allTeamBuffs.Count; i++)
        {
            allTeamBuffs[i].SetStats();
        }
    }

    private void OnDestroy()
    {
        if (playerBattle != null)
        {
            Destroy(playerBattle.gameObject);
        }
    }
}
