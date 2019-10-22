using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private int _multiPlayerSceneIndex;
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private Transform _playersContainer;
    [SerializeField] private GameObject _playerListPrefab;
    [SerializeField] private Text _roomNameDisplay;

    private void _ClearPlayerListings()
    {
        for (int i = _playersContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_playersContainer.GetChild(i).gameObject);
        }
    }

    private void _ListPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject tempListing = Instantiate(_playerListPrefab, _playersContainer);
            Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
            tempText.text = player.NickName;
        }
    }

    public override void OnJoinedRoom()
    {
        _roomPanel.SetActive(true);
        _lobbyPanel.SetActive(false);
        _roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient)
            _startButton.SetActive(true);
        else
            _startButton.SetActive(false);

        // Update the player list 
        _ClearPlayerListings();
        _ListPlayers();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _ClearPlayerListings();
        _ListPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _ClearPlayerListings();
        _ListPlayers();
        if (PhotonNetwork.IsMasterClient)
            _startButton.SetActive(true);
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(_multiPlayerSceneIndex);
        }
    }
    
    IEnumerator rejoinLobby()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.JoinLobby();
    }    
    
    // Paired to the back button in the room panel.
    // Return to the lobby panel
    public void BackOnClick()
    {
        _lobbyPanel.SetActive(true);
        _roomPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();     // rejoin in the lobby to prevent some bugs that may be caused by fast quit
        StartCoroutine(rejoinLobby());
    }
}
