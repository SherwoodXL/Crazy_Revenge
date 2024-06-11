using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsMine : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Weapon[] _weapon;
    [SerializeField] private GameObject _camera;
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private GameObject _playerUI;

    private void Start()
    {
        if (!_photonView.isMine)
        {
            _playerController.enabled = false;
            _camera.SetActive(false);
            _playerUI.SetActive(false);
            for (int i = 0; i < _weapon.Length; i++)
            {
                _weapon[i].enabled = false;
            }
        }
    }
}
