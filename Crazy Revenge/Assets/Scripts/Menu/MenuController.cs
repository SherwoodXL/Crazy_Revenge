using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    ColorBlock cb;

    // ��������� ���� ��� �������� ������
    [SerializeField]
    string _versionName = "1";

    // ������� ������� -- ������ � ������
    [SerializeField]
    GameObject _joinOrCreatePanel, _menuPanel;

    [SerializeField]
    Button _playButton;

    // ���� �����
    [SerializeField]
    TMP_InputField _usernameInput, _createGameInput, _joinGameInput;


    // ������������ Photon Engine
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(_versionName);
    }

    private void Update()
    {
        if (_usernameInput.text.Length >= 5)
        {
            _playButton.enabled = true;
            cb.normalColor = new Color(1, 1, 1);
            cb.highlightedColor = new Color(0.5f, 0.5f, 0.5f);
            cb.pressedColor = new Color(0.3f, 0.3f, 0.3f);
            _playButton.colors = cb;
        }
        else
        {
            _playButton.enabled = false;
            cb.normalColor = new Color(0.3f, 0.3f, 0.3f);
            cb.highlightedColor = new Color(0.3f, 0.3f, 0.3f);
            cb.pressedColor = new Color(0.3f, 0.3f, 0.3f);
            _playButton.colors = cb;
        }
    }

    // ����������� � �������
    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("�����������");
    }

    // ����� ����� ������������
    public void ChangePanel()
    {
        if (_usernameInput.text.Length >= 5)
        {
            _joinOrCreatePanel.SetActive(true);
            _menuPanel.SetActive(false);
        }
    }

    // ��������� ����� ������������
    public void SetUserNameToGame()
    {
        _joinOrCreatePanel.SetActive(false);
        PhotonNetwork.playerName = _usernameInput.text;
    }

    // �������� �������
    public void CreateGame()
    {
        PhotonNetwork.CreateRoom(_createGameInput.text, new RoomOptions() { maxPlayers = 10 }, null);
    }

    // ����������� � �������
    public void JoinGame()
    {
        PhotonNetwork.JoinOrCreateRoom(_joinGameInput.text, new RoomOptions() { maxPlayers = 10 }, TypedLobby.Default);
    }

    // ����������� � �����
    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Forest");
    }

    public void Quit()
    {
        Application.Quit();
    }
}