using HTW.AiRHockey.Game;
using HTW.AiRHockey.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTW.AiRHockey.Game
{

public class ARGameManager : GameManager
{
    [SerializeField]
    private TableBehaviour _table;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Player player in _players)
        {
            player.InitialPosition = player.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ResetPositions()
    {
        foreach (Player player in _players)
        {
            player.transform.rotation = new();
            player.transform.position = player.InitialPosition;
        }

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
            player.transform.position = _table.transform.position;
            player.transform.Translate(player.InitialPosition, _table.transform);
            player.gameObject.SetActive(true);
        }
        _puck.transform.position = _puck.InitialPosition + _table.transform.position;
        _puck.gameObject.SetActive(true);

    }


    #region private methods

    private void GameWon(Player winningPlayer)
    {
        Debug.Log($"Game Won by Player {winningPlayer}");
    }

    #endregion
}

}
