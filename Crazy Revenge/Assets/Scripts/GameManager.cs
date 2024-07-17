using UnityEngine;

public class GameManager : Photon.MonoBehaviour
{
    // Поля для префаба игрока и камеры сцены
    [SerializeField]
    GameObject _playerRed, _playerYellow;

    private void Start()
    {
        // Создаем игрока на сцене красного или жёлтого 
        if (PhotonNetwork.countOfPlayers % 2 == 0 && PhotonNetwork.countOfPlayers > 0)
            PhotonNetwork.Instantiate(_playerYellow.name, transform.position, Quaternion.identity, 0);
        else 
            PhotonNetwork.Instantiate(_playerRed.name, transform.position, Quaternion.identity, 0);
    }
}