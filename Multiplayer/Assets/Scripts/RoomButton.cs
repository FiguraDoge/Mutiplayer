using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomButton : MonoBehaviour
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _sizeText;

    private string _roomName;
    private int _roomSize;
    private int _playerCount;

    // Paired the button that is the room listing
    // Joins the player a room
    public void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(_roomName);
    }

    public void SetRoom(string name, int size, int count)
    {
        _roomName = name;
        _roomSize = size;
        _playerCount = count;
        _nameText.text = name;
        _sizeText.text = count + "/" + size;
    }
}
