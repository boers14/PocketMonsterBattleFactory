using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerPocketMonster : NetworkManager
{
    [SerializeField]
    private OnlineGameManager onlineGameManagerPrefab = null;

    private OnlineGameManager onlineGameManager = null;

    [SerializeField]
    private Transform player1Pos = null, player2Pos = null;

    [System.NonSerialized]
    public List<NetworkConnection> connectedPlayers = new List<NetworkConnection>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        connectedPlayers.Add(conn);

        Transform start = numPlayers == 0 ? player1Pos : player2Pos;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        if (GameObject.FindGameObjectWithTag("GameManager") == null)
        {
            onlineGameManager = Instantiate(onlineGameManagerPrefab);
            NetworkServer.Spawn(onlineGameManager.gameObject);
            onlineGameManager.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
        }

        if (numPlayers == 2)
        {
            StartCoroutine(StartTeamCreation());
        }
    }

    private IEnumerator StartTeamCreation()
    {
        yield return new WaitForSeconds(0.1f);
        onlineGameManager.CmdCreatePlayerTeams();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        connectedPlayers.Remove(conn);
    }
}
