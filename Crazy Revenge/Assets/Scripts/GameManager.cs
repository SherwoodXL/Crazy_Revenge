using UnityEngine;

public class GameManager : Photon.MonoBehaviour
{
    // ���� ��� ������� ������ � ������ �����
    [SerializeField]
    GameObject _playerRed, _playerYellow;

    private void Start()
    {
        // ������� ������ �� ����� �������� ��� ������ 
        if (PhotonNetwork.countOfPlayers % 2 == 0 && PhotonNetwork.countOfPlayers > 0)
            PhotonNetwork.Instantiate(_playerYellow.name, new Vector3(-22, 4.25f, 0), Quaternion.identity, 0);
        else 
            PhotonNetwork.Instantiate(_playerRed.name, new Vector3(22, 4.25f, 0), Quaternion.identity, 0);
    }
}