using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> gamePlayers { get; } = new List<PlayerObjectController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
            GamePlayerInstance.connectionID = conn.connectionId;
            GamePlayerInstance.playerIdNumber = gamePlayers.Count + 1;
            GamePlayerInstance.playerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(
                                                    (CSteamID)SteamLobby.instance.CurrentLobbyID, 
                                                    gamePlayers.Count
                                                    );
            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }

    public void StartGame(string sceneName)
    {
        ServerChangeScene(sceneName);
    }
}
