using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Color[] playerColors;
    public List<Player> players;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {

    }

    public int WhatPlayerAmI(Player player)
    {
        if (players.Contains(player))
        {
            return players.IndexOf(player);
        }

        else
        {
            players.Add(player);
            return WhatPlayerAmI(player);
        }

        return 0;
    }

    public Color WhatColorAmI(int playerIndex)
    {
        return playerColors[playerIndex];
    }

    public Color WhatColorAmI(Player player)
    {
        return playerColors[players.IndexOf(player)];
    }
}
