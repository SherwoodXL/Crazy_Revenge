using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsMine : MonoBehaviour
{
    [SerializeField] PlayerController _playerController;
    [SerializeField] Weapon _weapon;
    [SerializeField] GameObject _camera;
    [SerializeField] PhotonView _photonView;
    [SerializeField] GameObject _playerUI;

    private void Start()
    {
        if (!_photonView.isMine)
        {
            _playerController.enabled = false;
            _camera.SetActive(false);
            _playerUI.SetActive(false);
            _weapon.enabled = false;
        }
    }
}