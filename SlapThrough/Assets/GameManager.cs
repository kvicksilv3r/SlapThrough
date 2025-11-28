using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public enum MatchState
{
    Idle,
    Initialized,
    Started,
    Concluded
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Color[] playerColors;
    public List<Player> players;
    public Transform[] spawnPoints;
    public PlayerInputManager pim;
    private int connectedPlayers;
    private MatchState matchState = MatchState.Idle;

    public TextMeshProUGUI joinTMP;
    public TextMeshProUGUI countdownTMP;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        MatchIdle();
    }

    private void MatchIdle()
    {
        if (matchState == MatchState.Idle)
        {
            connectedPlayers = pim.playerCount;

            if (connectedPlayers == 2)
            {
                EnterMatchState(MatchState.Initialized);
            }
        }

        joinTMP.text = $"{connectedPlayers}/2 players joined";
    }

    private void EnterMatchState(MatchState state)
    {
        matchState = state;

        switch (matchState)
        {
            case MatchState.Idle:
                break;
            case MatchState.Initialized:
                InitiateMatch();
                break;
            case MatchState.Started:
                StartMatch();
                break;
            case MatchState.Concluded:
                break;
        }
    }

    private void InitiateMatch()
    {
        joinTMP.gameObject.SetActive(false);
        StartCoroutine(CountDownMatch());
    }

    IEnumerator CountDownMatch()
    {
        yield return new WaitForSeconds(0.5f);
        countdownTMP.gameObject.SetActive(true);
        countdownTMP.text = "3";
        yield return new WaitForSeconds(1);
        countdownTMP.text = "2";
        yield return new WaitForSeconds(1);
        countdownTMP.text = "1";
        yield return new WaitForSeconds(1);
        countdownTMP.gameObject.SetActive(false);
        EnterMatchState(MatchState.Started);
    }

    private void StartMatch()
    {
        foreach (Player p in players)
        {
            p.Enable();
        }
    }

    public Color WhatColorAmI(Player player)
    {
        return playerColors[GetIndexOf(player)];
    }

    public Vector3 GetSpawnPosition(Player player)
    {
        return spawnPoints[GetIndexOf(player)].position;
    }

    public int GetIndexOf(Player player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }

        return players.IndexOf(player);
    }
}