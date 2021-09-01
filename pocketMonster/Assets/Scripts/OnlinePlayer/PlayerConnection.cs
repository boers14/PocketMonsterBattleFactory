using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerConnection : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerObject = null;

    private GameObject player = null;

    private OnlineGameManager onlineGameManager = null;

    [System.NonSerialized]
    public OnlinePlayerBattle onlinePlayerBattle = null;

    // Sync vars automatically change when editing data on the SERVER for all the clients
    // Syncvars can hook, look at it when neccesary
    [SyncVar, System.NonSerialized]
    public int playerNumber = 0;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        CmdSetPlayerNumber();

        name = "You're player";
        tag = "PlayerConnection";

        SpawnUnit();
    }

    // Commands only run on the server
    [Command]
    private void SpawnUnit()
    {
        player = Instantiate(playerObject, transform.position, transform.rotation);
        onlinePlayerBattle = player.GetComponent<OnlinePlayerBattle>();
        NetworkServer.Spawn(player);
        player.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }

    [Command]
    private void CmdSetPlayerNumber()
    {
        playerNumber = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManagerPocketMonster>().numPlayers;
    }

    [Command]
    public void CmdSetConnectionToGamemanager(int moveIndex, bool wantsSwitch, int switchIndex)
    {
        SetOnlineGameManager();
        onlineGameManager.CmdSetMoveForPlayer(moveIndex, wantsSwitch, switchIndex, playerNumber);
    }

    private void SetOnlineGameManager()
    {
        if (!onlineGameManager)
        {
            onlineGameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<OnlineGameManager>();
        }
    }
}
