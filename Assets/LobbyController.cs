using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;

public class LobbyController : MonoBehaviour
{
    public static LobbyController instance;

    //UI Elements
    public Text lobbyNameText;

    //Player Data
    public GameObject playerListViewContent;
    public GameObject playerListItemPrefab;
    public GameObject localPlayerObject;

    //Other Data
    public ulong currentLobbyID;
    public bool playerItemCreated = false;
    private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
    public PlayerObjectController localPlayerController;

    //Ready
    public Button StartGameButton;

    //Manager
    private CustomNetworkManager manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
                return manager;
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ReadyPlayer()
    {
        localPlayerController.ChangeReady();
    }

    public void CheckIfAllReady()
    {
        bool AllReady = false;
        foreach (PlayerObjectController player in Manager.gamePlayers)
        {
            if (player.ready)
            {
                AllReady = true;
            }
            else
            {
                AllReady = false;
                break;
            }
        }

        if (AllReady)
        {
            if (localPlayerController.playerIdNumber == 1)
            {
                StartGameButton.interactable = true;
            }
        }
        else
        {
            StartGameButton.interactable = false;
        }
    }

    public void UpdateLobbyName()
    {
        currentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if (!playerItemCreated)
        {
            CreateHostPlayerItem(); //Host
        }

        if (playerListItems.Count < Manager.gamePlayers.Count)
        {
            CreateClientPlayerItem();
        }

        if (playerListItems.Count > Manager.gamePlayers.Count)
        {
            RemovePlayerItem();
        }

        if (playerListItems.Count == Manager.gamePlayers.Count)
        {
            UpdatePlayerItem();
        }
    }

    public void FindLocalPlayer()
    {
        localPlayerObject = GameObject.Find("LocalGamePlayer");
        localPlayerController = localPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.gamePlayers)
        {
            GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
            PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

            newPlayerItemScript.playerName = player.playerName;
            newPlayerItemScript.connectionID = player.connectionID;
            newPlayerItemScript.playerSteamID = player.playerSteamId;
            newPlayerItemScript.Ready = player.ready;
            newPlayerItemScript.SetPlayerValues();

            newPlayerItemScript.transform.SetParent(playerListViewContent.transform);
            newPlayerItemScript.transform.localScale = Vector3.one;

            playerListItems.Add(newPlayerItemScript);
        }

        playerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.gamePlayers)
        {
            if (!playerListItems.Any(b => b.connectionID == player.connectionID))
            {
                GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
                PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

                newPlayerItemScript.playerName = player.playerName;
                newPlayerItemScript.connectionID = player.connectionID;
                newPlayerItemScript.playerSteamID = player.playerSteamId;
                newPlayerItemScript.Ready = player.ready;
                newPlayerItemScript.SetPlayerValues();

                newPlayerItemScript.transform.SetParent(playerListViewContent.transform);
                newPlayerItemScript.transform.localScale = Vector3.one;

                playerListItems.Add(newPlayerItemScript);
            }
        }

    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.gamePlayers)
        {
            foreach (PlayerListItem playerListItemScript in playerListItems)
            {
                if (playerListItemScript.connectionID == player.connectionID)
                {
                    playerListItemScript.playerName = player.playerName;
                    playerListItemScript.Ready = player.ready;
                    playerListItemScript.SetPlayerValues();

                    //Se esse jogador for o usuario
                    if (player == localPlayerController)
                    {
                        
                    }
                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemsToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerListItem in playerListItems)
        {
            if (!Manager.gamePlayers.Any(b => b.connectionID == playerListItem.connectionID))
            {
                playerListItemsToRemove.Add(playerListItem);
            }
        }

        if (playerListItemsToRemove.Count > 0)
        {
            foreach (PlayerListItem playerListItemToRemove in playerListItemsToRemove)
            {
                GameObject objectToRemove = playerListItemToRemove.gameObject;
                playerListItems.Remove(playerListItemToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }

    public void StartGame(string sceneName)
    {
        localPlayerController.CanStartGame(sceneName);
    }
}
