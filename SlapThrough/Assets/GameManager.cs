using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;

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

	public Transform scoreHolder;
	public TextMeshProUGUI[] playerScoresTMPS;
	public Transform[] winnerTexts;

	public int targetScore = 5;
	public int[] scores;

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

	IEnumerator CountDownMatch(float delay = 0.75f)
	{
		yield return new WaitForSeconds(1f);
		countdownTMP.gameObject.SetActive(true);
		countdownTMP.text = "3";
		yield return new WaitForSeconds(delay);
		countdownTMP.text = "2";
		yield return new WaitForSeconds(delay);
		countdownTMP.text = "1";
		yield return new WaitForSeconds(delay);
		countdownTMP.gameObject.SetActive(false);
		EnterMatchState(MatchState.Started);
	}

	private void StartMatch()
	{
		StartRound();
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

	private void ShuffleSpawns()
	{
		spawnPoints.Shuffle();
	}

	private void StartRound()
	{
		DisplayScores();
		ShuffleSpawns();
		SetSpawns();
		//DisplayCountDown();
		EnablePlayers();
	}

	private void DisplayScores()
	{
		scoreHolder.gameObject.SetActive(true);
	}

	private void HideScores()
	{
		scoreHolder.gameObject.SetActive(false);
	}

	private void EnablePlayers()
	{
		foreach (Player p in players)
		{
			p.Enable();
		}
	}

	private void DisplayCountDown()
	{
		throw new NotImplementedException();
	}

	private void SetSpawns()
	{
		foreach (Player p in players)
		{
			p.SetSpawnPosition();
		}
	}

	public void PlayerScored(Player player)
	{
		scores[GetIndexOf(player)]++;

		UpdateScores();

		var winner = CheckWinner();

		if (winner >= 0)
		{
			ShowWinner(winner);
		}

		else
		{
			DisablePlayer(players[(players.Count + 1) % 2]);
			StartCoroutine(CountDownMatch(0.5f));
		}
	}

	private void ShowWinner(int winner)
	{
		DisablePlayers();
		HideScores();
		winnerTexts[winner].gameObject.SetActive(true);

	}

	private int CheckWinner()
	{
		if (scores[0] == targetScore)
		{
			return 0;
		}

		if (scores[1] == targetScore)
		{
			return 1;
		}

		return -1;
	}

	private void DisablePlayers()
	{
		foreach (Player p in players)
		{
			p.Disable();
		}
	}

	private void DisablePlayer(Player player)
	{
		player.Disable();
	}

	private void UpdateScores()
	{
		playerScoresTMPS[0].text = $"Score: {scores[0]}";
		playerScoresTMPS[1].text = $"Score: {scores[1]}";
	}
}