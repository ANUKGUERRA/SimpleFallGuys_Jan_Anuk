using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public TextMeshProUGUI winText;
    [SerializeField] public TextMeshProUGUI loseText;
    void Awake()
    {
        Instance = this;
    }
}
