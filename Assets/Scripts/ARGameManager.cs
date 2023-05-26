using HTW.AiRHockey.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARGameManager : MonoBehaviour
{
    [SerializeField]
    Player[] _players;
    [SerializeField]
    Puck _puck;
    [SerializeField]
    TableBehaviour _table;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPositions()
    {
        foreach (Player player in _players)
            player.transform.position = player.InitialPosition;

        Rigidbody puckRB = _puck.gameObject.GetComponent<Rigidbody>();
        puckRB.velocity = new();
        puckRB.angularVelocity = new();
        _puck.transform.position = _puck.InitialPosition;
    }

    public void EndGame()
    {
        foreach (Player player in _players)
        {
            player.gameObject.SetActive(false);
        }
        _puck.gameObject.SetActive(false);
        _table.LiftTableHandler();
    }

    public void StartGame()
    {
        _table.PlaceTableHandler();
        foreach(Player player in _players)
        {
            player.transform.position = player.InitialPosition + _table.transform.position;
            player.gameObject.SetActive(true);
        }
        _puck.transform.position = _puck.InitialPosition + _table.transform.position;
        _puck.gameObject.SetActive(true);

    }
}
