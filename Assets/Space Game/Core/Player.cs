using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private string playerName;
    public string Name { get => playerName; set => playerName = value; }

    [SerializeField] private int score = 0;
    public int Score { get => score; set => score = value; }
    [SerializeField] private int deaths = 0;
    public int Deaths { get => deaths; set => deaths = value; }

    [SerializeField] private Spaceship ship;
    public Spaceship Ship { get => ship; set => ship = value; }
}
