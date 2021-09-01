using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OnlineGameManager : GameManager
{
    [System.NonSerialized]
    public List<PocketMonster> player2PocketMonsters= new List<PocketMonster>();

    [System.NonSerialized]
    public List<PocketMonsterItem> player2Teambuffs= new List<PocketMonsterItem>();

    [SerializeField]
    private OnlineTeamCreator onlineTeamCreator = null;

    private PlayerConnection playerConnection = null;

    private OnlinePlayerBattle onlinePlayerBattle = null;

    private List<OnlinePlayerMove> selectedMovesFromPlayers = new List<OnlinePlayerMove>();

    private bool aPlayerHasFaintedPocketmonster = false;

    private bool startedTurn = false;

    public override void Start()
    {
        FillPocketMonsterMoveList();
        FillPocketMonsterItemList();
        FillTeamBuffsList();

        onlineTeamCreator = GameObject.FindGameObjectWithTag("TeamCreator").GetComponent<OnlineTeamCreator>();
        onlineTeamCreator.gameManager = this;

        GameObject[] activePocketMonsters = GameObject.FindGameObjectsWithTag("PocketMonster");
        for (int i = 0; i < activePocketMonsters.Length; i++)
        {
            Destroy(activePocketMonsters[i].gameObject);
        }
    }

    public override void Update()
    {
        if (!hasAuthority) { return; }

        if (selectedMovesFromPlayers.Count == 2 && !startedTurn)
        {
            print("start turn");
            StartCoroutine(StartTurnCoroutine());
        }
    }

    private IEnumerator StartTurnCoroutine()
    {
        startedTurn = true;
        yield return new WaitForSeconds(1);
        CmdPlayTurn();
    }

    [Command]
    public void CmdSetMoveForPlayer(int moveIndex, bool wantsSwitch, int switchIndex, int playerIndex)
    {
        OnlinePlayerMove onlinePlayerMove = new OnlinePlayerMove(playerIndex, moveIndex, switchIndex, wantsSwitch);
        selectedMovesFromPlayers.Add(onlinePlayerMove);

        RpcSetMoveForPlayer(moveIndex, wantsSwitch, switchIndex, playerIndex);
    }

    [ClientRpc]
    private void RpcSetMoveForPlayer(int moveIndex, bool wantsSwitch, int switchIndex, int playerIndex)
    {
        OnlinePlayerMove move = selectedMovesFromPlayers.Find(i => i.playerIndex == playerIndex);
        if (move == null)
        {
            OnlinePlayerMove onlinePlayerMove = new OnlinePlayerMove(playerIndex, moveIndex, switchIndex, wantsSwitch);
            selectedMovesFromPlayers.Add(onlinePlayerMove);
        }
    }

    [Command]
    private void CmdPlayTurn()
    {
        PlayerConnection[] currentConnections = FindObjectsOfType<PlayerConnection>();

        PlayerConnection player1Connection = null;
        PlayerConnection player2Connection = null;

        for (int i = 0; i < currentConnections.Length; i++)
        {
            if (currentConnections[i].playerNumber == 1)
            {
                player1Connection = currentConnections[i];
            } else if (currentConnections[i].playerNumber == 2)
            {
                player2Connection = currentConnections[i];
            }
        }

        OnlinePlayerMove player1Move = selectedMovesFromPlayers.Find(i => i.playerIndex == 1);
        OnlinePlayerMove player2Move = selectedMovesFromPlayers.Find(i => i.playerIndex == 2);

        PocketMonster player1PocketMonster = null;
        PocketMonster player2PocketMonster = null;

        if (!player1Move.wantsSwitch && !player2Move.wantsSwitch)
        {
            player1PocketMonster = player1Connection.onlinePlayerBattle.currentPocketMonster;
            player2PocketMonster = player2Connection.onlinePlayerBattle.currentPocketMonster;

            if (player1PocketMonster.stats.speed.actualStat > player2PocketMonster.stats.speed.actualStat)
            {
                CalculateDamageForPocketMonsters(player1PocketMonster, player2PocketMonster, player1Move, player2Move);
            }
            else if (player1PocketMonster.stats.speed.actualStat < player2PocketMonster.stats.speed.actualStat)
            {
                CalculateDamageForPocketMonsters(player2PocketMonster, player1PocketMonster, player2Move, player1Move);
            }
            else
            {
                int speedTie = Random.Range(1, 101);

                if (speedTie <= 50)
                {
                    CalculateDamageForPocketMonsters(player1PocketMonster, player2PocketMonster, player1Move, player2Move);
                }
                else
                {
                    CalculateDamageForPocketMonsters(player2PocketMonster, player1PocketMonster, player2Move, player1Move);
                }
            }
        } else if (player1Move.wantsSwitch && player2Move.wantsSwitch)
        {
            player1PocketMonster = player1Connection.onlinePlayerBattle.pocketMonsters[player1Move.switchIndex];
            player2PocketMonster = player2Connection.onlinePlayerBattle.pocketMonsters[player2Move.switchIndex];
        } else
        {
            if (player1Move.wantsSwitch)
            {
                player1PocketMonster = player1Connection.onlinePlayerBattle.pocketMonsters[player1Move.switchIndex];
                player2PocketMonster = player2Connection.onlinePlayerBattle.currentPocketMonster;

                DealDamageToPocketMonser(player2PocketMonster, player1PocketMonster, player2Move);
            } else if (player2Move.wantsSwitch)
            {
                player1PocketMonster = player1Connection.onlinePlayerBattle.currentPocketMonster;
                player2PocketMonster = player2Connection.onlinePlayerBattle.pocketMonsters[player2Move.switchIndex];

                DealDamageToPocketMonser(player1PocketMonster, player2PocketMonster, player1Move);
            }
        }

        print(player1PocketMonster.stats.name + ": " + player1PocketMonster.health);
        print(player2PocketMonster.stats.name + ": " + player2PocketMonster.health);

        CheckIfPocketmonsterFainted(player1PocketMonster.health);
        CheckIfPocketmonsterFainted(player2PocketMonster.health);

        RpcPlayTurn(player1PocketMonster.health, player1PocketMonster.currentStatus.ToString(), player2PocketMonster.health, 
            player2PocketMonster.currentStatus.ToString(), aPlayerHasFaintedPocketmonster);
    }

    private void CalculateDamageForPocketMonsters(PocketMonster fasterPocketMonster, PocketMonster slowerPocketMonster, OnlinePlayerMove fasterMove,
        OnlinePlayerMove slowerMove)
    {
        DealDamageToPocketMonser(fasterPocketMonster, slowerPocketMonster, fasterMove);
        
        if (!slowerPocketMonster.fainted)
        {
            DealDamageToPocketMonser(slowerPocketMonster, fasterPocketMonster, slowerMove);
        }
    }

    private void DealDamageToPocketMonser(PocketMonster aggressor, PocketMonster target, OnlinePlayerMove attackIndex)
    {
        float damage = aggressor.CalculateDealingDamage(target, aggressor.moves[attackIndex.moveIndex],
            onlinePlayerBattle);

        target.health -= damage;
        if (target.health <= 0)
        {
            target.health = 0;
            target.fainted = true;
        }
    }

    private void CheckIfPocketmonsterFainted(float health)
    {
        if (!aPlayerHasFaintedPocketmonster && health <= 0)
        {
            aPlayerHasFaintedPocketmonster = true;
        }
    }

    [ClientRpc]
    private void RpcPlayTurn(float player1PocketmonsterHealth, string player1PocketmonsterStatus, float player2PocketmonsterHealth,
        string player2PocketmonsterStatus, bool aPlayerHasFaintedPocketmonster)
    {
        this.aPlayerHasFaintedPocketmonster = aPlayerHasFaintedPocketmonster;

        PocketMonster player1PocketMonster = null;
        PocketMonster player2PocketMonster = null;

        OnlinePlayerMove player1Move = selectedMovesFromPlayers.Find(i => i.playerIndex == 1);
        OnlinePlayerMove player2Move = selectedMovesFromPlayers.Find(i => i.playerIndex == 2);

        if (playerConnection.playerNumber == 1)
        {
            CheckIfPlayerSwitched(player1Move, onlinePlayerBattle);
            CheckIfPlayerSwitched(player2Move, onlinePlayerBattle.opponentPlayer);

            player1PocketMonster = onlinePlayerBattle.currentPocketMonster;
            player2PocketMonster = onlinePlayerBattle.opponentPlayer.currentPocketMonster;

            onlinePlayerBattle.opponentPocketMonster = player2PocketMonster;
        }
        else
        {
            CheckIfPlayerSwitched(player1Move, onlinePlayerBattle.opponentPlayer);
            CheckIfPlayerSwitched(player2Move, onlinePlayerBattle);

            player1PocketMonster = onlinePlayerBattle.opponentPlayer.currentPocketMonster;
            player2PocketMonster = onlinePlayerBattle.currentPocketMonster;

            onlinePlayerBattle.opponentPocketMonster = player1PocketMonster;
        }

        player1PocketMonster.health = player1PocketmonsterHealth;
        player1PocketMonster.currentStatus = (PocketMonster.StatusEffects)System.Enum.Parse(typeof(PocketMonster.StatusEffects),
            player1PocketmonsterStatus);

        if (player1PocketmonsterHealth <= 0)
        {
            player1PocketMonster.fainted = true;
        }

        player2PocketMonster.health = player2PocketmonsterHealth;
        player2PocketMonster.currentStatus = (PocketMonster.StatusEffects)System.Enum.Parse(typeof(PocketMonster.StatusEffects),
            player2PocketmonsterStatus);

        if (player2PocketmonsterHealth <= 0)
        {
            player2PocketMonster.fainted = true;
        }

        PocketMonsterTextUpdate player1TextUpdate = new PocketMonsterTextUpdate(player1PocketMonster, null, player1PocketMonster.currentStatus,
            player1PocketmonsterHealth.ToString(), "bob", true, true, true, false);
        PocketMonsterTextUpdate player2TextUpdate = new PocketMonsterTextUpdate(player2PocketMonster, null, player2PocketMonster.currentStatus,
            player2PocketmonsterHealth.ToString(), "bob", true, true, true, false);

        if (playerConnection.playerNumber == 1)
        {
            player1TextUpdate.player = "You";
            player2TextUpdate.player = "Opponent";
            onlinePlayerBattle.UpdatePlayerText(player1TextUpdate);
            onlinePlayerBattle.UpdateOpponentText(player2TextUpdate);
        }
        else
        {
            player1TextUpdate.player = "Opponent";
            player2TextUpdate.player = "You";
            onlinePlayerBattle.UpdatePlayerText(player2TextUpdate);
            onlinePlayerBattle.UpdateOpponentText(player1TextUpdate);
        }

        onlinePlayerBattle.CreateOwnPocketMosterUI();
        onlinePlayerBattle.SetSwitchButtonDescription();

        if (!aPlayerHasFaintedPocketmonster)
        {
            onlinePlayerBattle.EnableButtons(true, false);
        }
        else if (playerConnection.playerNumber == 1 && player1PocketMonster.fainted ||
        playerConnection.playerNumber == 2 && player2PocketMonster.fainted)
        {
            onlinePlayerBattle.SetSwitchButtonsForWhenPocketMonsterIsFainted();
        }
        else if (playerConnection.playerNumber == 1 && !player1PocketMonster.fainted ||
        playerConnection.playerNumber == 2 && !player2PocketMonster.fainted)
        {
            int index = onlinePlayerBattle.pocketMonsters.IndexOf(onlinePlayerBattle.currentPocketMonster);
            playerConnection.CmdSetConnectionToGamemanager(-1, true, index);
            onlinePlayerBattle.EnableButtons(false, false);
        }

        selectedMovesFromPlayers.Clear();
        startedTurn = false;
        this.aPlayerHasFaintedPocketmonster = false;

        if (hasAuthority)
        {
            CmdClearMovesList();
        }

        print("end turn");
    }

    [Command]
    private void CmdClearMovesList()
    {
        selectedMovesFromPlayers.Clear();
        aPlayerHasFaintedPocketmonster = false;
    }

    private void CheckIfPlayerSwitched(OnlinePlayerMove playerMove, OnlinePlayerBattle playerBattle)
    {
        if (playerMove.wantsSwitch)
        {
            playerBattle.currentPocketMonster.gameObject.SetActive(false);
            playerBattle.currentPocketMonster = playerBattle.pocketMonsters[playerMove.switchIndex];
            playerBattle.currentPocketMonster.gameObject.SetActive(true);
        }
    }

    [Command]
    public void CmdCreatePlayerTeams()
    {
        ClearPocketMonsterList(playerPocketMonsters);
        ClearPocketMonsterList(player2PocketMonsters);

        playerPocketMonsters = onlineTeamCreator.createTeamForAi(null, false);
        teamBuffsOfPlayer = onlineTeamCreator.CreateTeambuffsList();

        player2PocketMonsters = onlineTeamCreator.createTeamForAi(null, false);
        player2Teambuffs = onlineTeamCreator.CreateTeambuffsList();

        List<int> player1PocketMonstersInInt = new List<int>();
        List<int> player1PocketMonstersAbilitysInInt = new List<int>();
        List<List<int>> player1PocketMonstersItemsInInt = new List<List<int>>();
        List<List<int>> player1PocketMonstersMovesInInt = new List<List<int>>();
        List<int> player1TeamBuffsInInt = new List<int>();

        TranslatePocketmonsterTeamToInt(playerPocketMonsters, player1PocketMonstersInInt, player1PocketMonstersAbilitysInInt, 
            player1PocketMonstersItemsInInt, player1PocketMonstersMovesInInt);
        TranslateItemsToInt(player1TeamBuffsInInt, teamBuffsOfPlayer, allTeamBuffs);

        List<int> player2PocketMonstersInInt = new List<int>();
        List<int> player2PocketMonstersAbilitysInInt = new List<int>();
        List<List<int>> player2PocketMonstersItemsInInt = new List<List<int>>();
        List<List<int>> player2PocketMonstersMovesInInt = new List<List<int>>();
        List<int> player2TeamBuffsInInt = new List<int>();

        TranslatePocketmonsterTeamToInt(player2PocketMonsters, player2PocketMonstersInInt, player2PocketMonstersAbilitysInInt,
            player2PocketMonstersItemsInInt, player2PocketMonstersMovesInInt);
        TranslateItemsToInt(player2TeamBuffsInInt, player2Teambuffs, allTeamBuffs);

        RpcSetPlayerTeams(player1PocketMonstersInInt, player1PocketMonstersAbilitysInInt, player1PocketMonstersItemsInInt,
            player1PocketMonstersMovesInInt, player1TeamBuffsInInt, player2PocketMonstersInInt, player2PocketMonstersAbilitysInInt, 
            player2PocketMonstersItemsInInt, player2PocketMonstersMovesInInt, player2TeamBuffsInInt);
    }

    [ClientRpc]
    private void RpcSetPlayerTeams(List<int> player1PocketMonstersInInt, List<int> player1PocketMonstersAbilitysInInt,
        List<List<int>> player1PocketMonstersItemsInInt, List<List<int>> player1PocketMonstersMovesInInt, List<int> player1TeamBuffsInInt,
        List<int> player2PocketMonstersInInt, List<int> player2PocketMonstersAbilitysInInt, List<List<int>> player2PocketMonstersItemsInInt, 
        List<List<int>> player2PocketMonstersMovesInInt, List<int> player2TeamBuffsInInt)
    {
        FetchPlayerConnection();

        ClearPocketMonsterList(playerPocketMonsters);
        ClearPocketMonsterList(player2PocketMonsters);

        teamBuffsOfPlayer.Clear();
        player2Teambuffs.Clear();

        if (playerConnection.playerNumber == 1)
        {
            GetPocketMonsterTeamFromIntList(playerPocketMonsters, player1PocketMonstersInInt, player1PocketMonstersAbilitysInInt,
                player1PocketMonstersItemsInInt, player1PocketMonstersMovesInInt);
            RecoverItems(player1TeamBuffsInInt, teamBuffsOfPlayer, allTeamBuffs);

            GetPocketMonsterTeamFromIntList(player2PocketMonsters, player2PocketMonstersInInt, player2PocketMonstersAbilitysInInt,
                player2PocketMonstersItemsInInt, player2PocketMonstersMovesInInt);
            RecoverItems(player2TeamBuffsInInt, player2Teambuffs, allTeamBuffs);
        } else
        {
            GetPocketMonsterTeamFromIntList(playerPocketMonsters, player2PocketMonstersInInt, player2PocketMonstersAbilitysInInt,
                player2PocketMonstersItemsInInt, player2PocketMonstersMovesInInt);
            RecoverItems(player2TeamBuffsInInt, teamBuffsOfPlayer, allTeamBuffs);

            GetPocketMonsterTeamFromIntList(player2PocketMonsters, player1PocketMonstersInInt, player1PocketMonstersAbilitysInInt,
                player1PocketMonstersItemsInInt, player1PocketMonstersMovesInInt);
            RecoverItems(player1TeamBuffsInInt, player2Teambuffs, allTeamBuffs);
        }

        battleTextManager = GameObject.FindGameObjectWithTag("InBattleTextManager").GetComponent<InBattleTextManager>();
        onlinePlayerBattle = GameObject.FindGameObjectWithTag("Player").GetComponent<OnlinePlayerBattle>();
        battleTextManager.SetPlayer(onlinePlayerBattle);
        onlinePlayerBattle.SetInBattleTextManager(battleTextManager);
        onlinePlayerBattle.AddpocketMonster(playerPocketMonsters, teamBuffsOfPlayer, player2PocketMonsters, player2Teambuffs);
        onlinePlayerBattle.CreateUI();
    }

    private void ClearPocketMonsterList(List<PocketMonster> pocketMonsters)
    {
        for (int i = 0; i < pocketMonsters.Count; i++)
        {
            Destroy(pocketMonsters[i].gameObject);
        }

        pocketMonsters.Clear();
    }

    private void FetchPlayerConnection()
    {
        if (playerConnection == null)
        {
            playerConnection = GameObject.FindGameObjectWithTag("PlayerConnection").GetComponent<PlayerConnection>();
        }
    }
}
