using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public static GameMgr Instance;

    [Header("Chat UI")]
    public Text nameText;
    public Image nameBox;
    public Text chatText;
    public Image chatBox;

    [Header("FadeEffect")]
    public Image fadeImage;
    public Animator fadeAnimator;

    [Header("Animation")]
    public Animator characterAnimator;

    [Header("SelectOption")]
    public GameObject selectPanel;
    public Image selectImage;        // 선택지 UI 패널
    public Text selectText;         // 선택지 글자 
    public Button optionButton1;    // 선택지 버튼 1
    public Button optionButton2;    // 선택지 버튼 2
    public Text optionBtnText1;
    public Text optionBtnText2;

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

