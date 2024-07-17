using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundsManager : Photon.MonoBehaviour
{
    bool plusPoint = true;

    float time = 0;

    [SerializeField]
    public int gameEvent = -1;
    public int _redPoint;
    public int _yellowPoint;

    public int _redScore = 0;
    public int _yellowScore = 0;

    [Header("Event Ice")]
    public int jumpForce;
    public int groundDrag;
    public int moveSpeed;

    [Header("Event Collection")]
    [SerializeField]
    GameObject[] collectZone;
    [SerializeField]
    public GameObject bacon;
    [SerializeField]
    Vector3[] baconPos;

    public bool bring = true;

    [Header("Catch-Up")]
    [SerializeField]
    GameObject saveZone;
    [SerializeField]
    GameObject[] player;

    float endOfTime = 18;
    public float timeCU = 18;
    public bool catchup = false;

    private void Start()
    {
        if (PhotonNetwork.isMasterClient)
            Randomaizer(Random.Range(0, 3));
    }

    private void Randomaizer(int num)
    {
        photonView.RPC("GameEvent", PhotonTargets.AllBufferedViaServer, num);
    }

    [PunRPC]
    public void GameEvent(int numEvent)
    {
        plusPoint = true;
        bring = true;
        catchup = false;
        timeCU = 18;
        endOfTime = 18;
        _redScore = 0;
        _yellowScore = 0;
        gameEvent = numEvent;

        if (numEvent == 1)
            bacon.SetActive(true);
    }

    private void Update()
    {
        time += Time.deltaTime;

        EventIce();
        EventCollection();
        EventCatchUp();

        if (time > 3 && PhotonNetwork.isMasterClient)
            photonView.RPC(nameof(OtherGetStatistic), PhotonTargets.OthersBuffered, _redPoint, _yellowPoint, _redScore, _yellowScore);
    }

    private void EventIce()
    {
        if (gameEvent == 0)
        {
            jumpForce = 8;
            groundDrag = -2;
            moveSpeed = 7;

            if (_redScore == 2 && PhotonNetwork.isMasterClient && plusPoint == true)
            {
                Randomaizer(Random.Range(0, 3));
                photonView.RPC(nameof(Points), PhotonTargets.AllBufferedViaServer, 1, 0);
                plusPoint = false;
            }
            else if (_yellowScore == 2 && PhotonNetwork.isMasterClient && plusPoint == true)
            {
                Randomaizer(Random.Range(0, 3));
                photonView.RPC(nameof(Points), PhotonTargets.AllBufferedViaServer, 0, 1);
                plusPoint = false;
            }
        }
        else
        {
            jumpForce = 7;
            groundDrag = 7;
            moveSpeed = 5;
        }
    }

    private void EventCollection()
    {
        if (gameEvent == 1)
        {
            collectZone[0].SetActive(true);
            collectZone[1].SetActive(true);

            if (bring == true && PhotonNetwork.isMasterClient)
            {
                BaconPosition();
                bacon.SetActive(true);
                bring = false;
            }

            if (_redScore == 5 && PhotonNetwork.isMasterClient && plusPoint == true)
            {
                Randomaizer(Random.Range(0, 3));
                photonView.RPC(nameof(Points), PhotonTargets.AllBufferedViaServer, 1, 0);
                plusPoint = false;
            }
            else if (_yellowScore == 5 && PhotonNetwork.isMasterClient && plusPoint == true)
            {
                Randomaizer(Random.Range(0, 3));
                photonView.RPC(nameof(Points), PhotonTargets.AllBufferedViaServer, 0, 1);
                plusPoint = false;
            }
        }
        else
        {
            collectZone[0].SetActive(false);
            collectZone[1].SetActive(false);
            bacon.SetActive(false);
        }
    }

    private void BaconPosition()
    {
        if (PhotonNetwork.isMasterClient && bring == true)
            bacon.transform.position = baconPos[Random.Range(0, baconPos.Length)];
    }

    private void EventCatchUp()
    {
        int reds = GameObject.FindGameObjectsWithTag("PlayerYellow").Length;
        int yellows = GameObject.FindGameObjectsWithTag("PlayerRed").Length;

        if (gameEvent == 2)
        {
            saveZone.SetActive(true);

            if (_redScore == 5 && PhotonNetwork.isMasterClient && plusPoint == true)
            {
                Randomaizer(Random.Range(0, 3));
                photonView.RPC(nameof(Points), PhotonTargets.AllBufferedViaServer, 1, 0);
                plusPoint = false;
            }
            else if (_yellowScore == 5 && PhotonNetwork.isMasterClient && plusPoint == true)
            {
                Randomaizer(Random.Range(0, 3));
                photonView.RPC(nameof(Points), PhotonTargets.AllBufferedViaServer, 0, 1);
                plusPoint = false;
            }
        }
        else
        {
            saveZone.SetActive(false);
        }
    }

    public void SaveZonePosition()
    {
        saveZone.transform.position = new Vector3(Random.Range(-14.5f, 11f), 3, Random.Range(-14.8f, 9f));
        catchup = false;
    }

    [PunRPC]
    private void Points(int reds, int yellows)
    {
        _redPoint += reds;
        _yellowPoint += yellows;
    }

    [PunRPC]
    private void OtherGetStatistic(int pointsRed, int pointsYellow, int scoresRed, int scoresYellow)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            _redPoint = pointsRed;
            _yellowPoint = pointsYellow;
            _redScore = scoresRed;
            _yellowScore = scoresYellow;
        }
    }
}