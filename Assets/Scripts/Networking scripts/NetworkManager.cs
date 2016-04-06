using UnityEngine;
using System;
using System.Collections;

using UnityEngine.UI;

public class NetworkManager : MonoBehaviour 
{
	private const string roomName = "RoomName";
	private RoomInfo[] roomsList;

    private string characterToPlayAs;

    public Image playerSelection;
    public Button ethanButton;
    public Button chompieButton;

    public GameObject abilityPanel;

    void Start()
	{
        //if (character_To_Play_As == "")
          //  character_To_Play_As = "Player";
        //PhotonNetwork.sendRate = 500;
        //PhotonNetwork.sendRateOnSerialize = 500;
        PhotonNetwork.ConnectUsingSettings ("0.1");

        ethanButton.onClick.AddListener(ChooseEthan);
        chompieButton.onClick.AddListener(ChooseChompie);

    }

	void OnGUI()
	{
		if (!PhotonNetwork.connected) 
		{
			GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());
		} 
		else if (PhotonNetwork.room == null) {
			// Create Room
			if (GUI.Button (new Rect (100, 100, 250, 100), "Start Server")) {
				TypedLobby typed_lobby = new TypedLobby ();
				RoomOptions room_options = new RoomOptions () {
					isVisible = true,
					isOpen = true,
					cleanupCacheOnLeave = true,
					maxPlayers = 5
				};
				PhotonNetwork.CreateRoom (roomName + Guid.NewGuid ().ToString ("N"), room_options, typed_lobby);
			}

			// Join Room
			if (roomsList != null) {
				for (int i = 0; i < roomsList.Length; i++) {
					if (GUI.Button (new Rect (100, 250 + (110 * i), 250, 100), "Join " + roomsList [i].name)) {
						PhotonNetwork.JoinRoom (roomsList [i].name);
					}
				}
			}
		} 
		else 
		{
			GUI.Label (new Rect (Screen.width - 200, Screen.height - 24, 200, 80), "" + PhotonNetwork.GetPing ());
		}
	}

	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
	}

	void OnJoinedRoom()
	{
        playerSelection.gameObject.SetActive(true);
	}


    void PlayerChosen()
    {
        playerSelection.gameObject.SetActive(false);
        GameObject player = PhotonNetwork.Instantiate(characterToPlayAs, new Vector3(0, 2, 0), Quaternion.identity, 0);
        player.tag = "Player";
        
        player.transform.GetComponentInChildren<CameraRotate>().enabled = true;
        player.transform.GetComponentInChildren<Camera>().enabled = true;

        abilityPanel.SetActive(true);
    }


    public void ChooseEthan()
    {
        characterToPlayAs = "Player";
        PlayerChosen();
    }

    public void ChooseChompie()
    {
        characterToPlayAs = "Chompie";
        PlayerChosen();
    }

}