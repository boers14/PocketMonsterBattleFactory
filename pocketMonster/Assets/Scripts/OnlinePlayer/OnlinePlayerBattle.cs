using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class OnlinePlayerBattle : PlayerBattle
{
    private bool battleStarted = false;

    [System.NonSerialized]
    public OnlinePlayerBattle opponentPlayer = null;

    private OnlineGameManager onlineGameManager = null;

    private PlayerConnection playerConnection = null;

    private void Start()
    {
        onlineGameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<OnlineGameManager>();
        GetComponent<PlayerPocketMonsterMenu>().SetGameManager(onlineGameManager);

        StartCoroutine(CheckIfHasAutority());
    }

    public override void Update()
    {
        if (!hasAuthority) { return; }

        if (battleStarted)
        {
            base.Update();
        }
    }

    private IEnumerator CheckIfHasAutority()
    {
        yield return new WaitForEndOfFrame();

        if (hasAuthority)
        {
            tag = "Player";
        } else
        {
            tag = "OpponentPlayer";
        }
    }

    public override void AddpocketMonster(List<PocketMonster> pocketMonstersThisBattle, List<PocketMonsterItem> teamBuffsThisBattle,
        List<PocketMonster> opponentPocketMonstersThisBattle, List<PocketMonsterItem> opponentTeamBuffsThisBattle)
    {
        battleStarted = true;
        opponentPlayer = GameObject.FindGameObjectWithTag("OpponentPlayer").GetComponent<OnlinePlayerBattle>();

        base.AddpocketMonster(pocketMonstersThisBattle, teamBuffsThisBattle);
        AddOpponentTeam(opponentPocketMonstersThisBattle, opponentTeamBuffsThisBattle);
    }

    private void AddOpponentTeam(List<PocketMonster> opponentPocketMonstersThisBattle, List<PocketMonsterItem> opponentTeamBuffsThisBattle)
    {
        opponentPlayer.teamBuffs.Clear();
        for (int i = 0; i < opponentTeamBuffsThisBattle.Count; i++)
        {
            opponentPlayer.teamBuffs.Add(opponentTeamBuffsThisBattle[i]);
        }

        for (int i = 0; i < opponentPocketMonstersThisBattle.Count; i++)
        {
            PocketMonster newPocketMonster = Instantiate(opponentPocketMonstersThisBattle[i]);
            newPocketMonster.moves = opponentPocketMonstersThisBattle[i].moves;
            newPocketMonster.items = opponentPocketMonstersThisBattle[i].items;
            newPocketMonster.transform.position = new Vector3(opponentPlayer.transform.position.x + opponentPlayer.transform.forward.x * disFromPlayer,
                opponentPlayer.transform.position.y, opponentPlayer.transform.position.z + opponentPlayer.transform.forward.z * disFromPlayer);
            newPocketMonster.transform.eulerAngles = opponentPlayer.transform.eulerAngles;
            newPocketMonster.SetAllStats();
            newPocketMonster.chosenAbility = opponentPocketMonstersThisBattle[i].chosenAbility;

            newPocketMonster.gameObject.SetActive(false);
            newPocketMonster.ResetHealth();
            newPocketMonster.SetInBattleTextManager(inBattleTextManager);
            newPocketMonster.FillItemsAffectedStatsList(opponentTeamBuffsThisBattle, opponentPlayer);
            opponentPlayer.pocketMonsters.Add(newPocketMonster);
        }

        opponentPlayer.currentPocketMonster = opponentPlayer.pocketMonsters[0];
        opponentPlayer.currentPocketMonster.gameObject.SetActive(true);
        opponentPlayer.previousPocketMonster = opponentPlayer.currentPocketMonster;

        opponentPocketMonster = opponentPlayer.currentPocketMonster;
    }

    public override void OnAbilityUsage(int index)
    {
        EnableAbilityMenu(false);
    }

    public override void SetUsableMove(int index)
    {
        if (currentPocketMonster.moves[index].currentPowerPoints <= 0)
        {
            return;
        }

        SetPlayerConnection();
        playerConnection.CmdSetConnectionToGamemanager(index, false, -1);

        EnableButtons(false, true);
    }

    public override void OnSwitchAction(int index)
    {
        if (pocketMonsters[index].fainted || currentPocketMonster.currentStatus == PocketMonster.StatusEffects.Trapped) { return; }

        SetPlayerConnection();

        SetSwitchButtonText(index);

        playerConnection.CmdSetConnectionToGamemanager(-1, true, index);

        EnableButtons(false, true);
    }


    private void SetPlayerConnection()
    {
        if (!playerConnection)
        {
            playerConnection = GameObject.FindGameObjectWithTag("PlayerConnection").GetComponent<PlayerConnection>();
        }
    }
}
