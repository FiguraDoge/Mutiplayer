using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _lobbyJoinButton;
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private InputField _playerNameInput;

    private string _roomName;
    private int _roomSize;

    private List<RoomInfo> _roomListings;
    [SerializeField] private Transform _roomsContainer;
    [SerializeField] private GameObject _roomListingPrefab;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _lobbyJoinButton.SetActive(true);
        _roomListings = new List<RoomInfo>();

        // Check for player name saved to player prefs
        if (PlayerPrefs.HasKey("NickName"))
        {
            // Empty name
            if (PlayerPrefs.GetString("NickName") == "")
                PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
            else
                PhotonNetwork.NickName = PlayerPrefs.GetString("NickName");
        }
        else
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
        }
        _playerNameInput.text = PhotonNetwork.NickName;
    }

    public void PlayerNameUpdate(string nameInput)
    {
        PhotonNetwork.NickName = nameInput;
        PlayerPrefs.SetString("NickName", nameInput);
    }

    public void JoinLobby()
    {
        _startPanel.SetActive(false);
        _lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int tempIndex;
        foreach (RoomInfo room in roomList)
        {
            if (_roomListings != null)
                tempIndex = _roomListings.FindIndex(Byname(room.Name));   
            else
                tempIndex = -1;

            // Remove listing from UI because it has been closed
            if (tempIndex != -1)
            {
                _roomListings.RemoveAt(tempIndex);
                Destroy(_roomsContainer.GetChild(tempIndex).gameObject);
            }

            // Add room listing because it is new
            if (room.PlayerCount > 0)
            {
                _roomListings.Add(room);
                _ListRoom(room);
            }
        }
    }

    // Predict function for search through room
    static System.Predicate<RoomInfo> Byname(string name)
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }

    // Display new room listing for the current room
    private void _ListRoom(RoomInfo room)
    {
        if (room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(_roomListingPrefab, _roomsContainer);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount);
        }
    }

    // Input funciton for changing room name
    public void OnRoomNameChanged(string name)
    {
        _roomName = name;
    }

    // Input function for changing room size
    public void OnRoomSizeChanged(string size)
    {
        _roomSize = int.Parse(size);
    }

    // function paired to the create room buttom
    public void CreateRoom()
    {
        Debug.Log("Creating room now");
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)_roomSize };
        PhotonNetwork.CreateRoom(_roomName, roomOps);   //create a new room
    }

    // If create room is failed
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed. Please try on other room name");
    }

    // Paired to the back button.
    // Used to go back to the start menu
    public void MatchmakingCancel()
    {
        _startPanel.SetActive(true);
        _lobbyPanel.SetActive(false);
        PhotonNetwork.LeaveLobby();
    }
}
