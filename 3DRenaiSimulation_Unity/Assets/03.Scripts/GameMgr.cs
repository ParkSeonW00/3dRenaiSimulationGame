using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMgr : MonoBehaviour
{
    public static GameMgr Instance;
    private SelectMgr selectMgr;

    [Header("Global Data")]
    public List<Dictionary<string, object>> dialogueData;
    public List<Dictionary<string, object>> eventData;
    public List<Dictionary<string, object>> selectData;

    public enum GameState
    {
        Dialogue,
        Event,
        Idle,
        Select
    }

    public GameState currentState; // 현재 상태

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}

