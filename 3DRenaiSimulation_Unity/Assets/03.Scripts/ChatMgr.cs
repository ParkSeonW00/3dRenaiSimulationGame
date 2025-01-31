
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
//using static DialogueData;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class ChatMgr : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject ChatGameObject;
    public GameObject NameGameObject;
    public Image chatBox;
    public Image nameBox;
    public Text chatText;
    public Text nameText;

    [Header("Animator")]
    public GameObject vroidModel;
    public Animator characterAnimator; // 캐릭터 애니메이션을 담당하는 Animator

    [Header("CSV Reader")]
    public string csv_FileName;

    [Header("Fade In-Out")]
    public Animator fadeAnimator;    //Fade 관리 애니메이터
    public Image fadeImage;

    [Header("NextScene")]
    public string nextScene;

    [Header("State Management")]
    public GameMgr.GameState currentState = GameMgr.GameState.Dialogue;

    int ScriptCount = 0;
    List<Dictionary<string, object>> chatData;

    void Start()
    {
        chatData = CSVReader.Read(csv_FileName);
        nameText.text = chatData[ScriptCount].ContainsKey("캐릭터 이름") ? chatData[ScriptCount]["캐릭터 이름"].ToString() : ""; 
        chatText.text = chatData[ScriptCount].ContainsKey("대사 (스크립트)") ? chatData[ScriptCount]["대사 (스크립트)"].ToString() : ""; 

        if (chatData == null || chatData.Count == 0)
        {
            Debug.LogError("CSV 파일을 읽을 수 없습니다.");
            return;
        }

        // 페이드 인 애니메이션 실행
        FadeEffect("FadeInTrigger");
    }

    void Update()
    {
        if (currentState == GameMgr.GameState.Dialogue && Input.GetMouseButtonDown(0))
        {
            DisplayNextSentence();
        }
    }

    void DisplayNextSentence()
    {
        currentState = GameMgr.GameState.Dialogue;

        // 대화 데이터가 더 이상 없는경우 EndDialogue
        if (ScriptCount >= chatData.Count)
        {
            string fadeanimationTrigger = chatData[ScriptCount-1].ContainsKey("페이드") ? chatData[ScriptCount-1]["페이드"].ToString() : "";
            FadeEffect(fadeanimationTrigger);
            EndDialogue();
            return;
        }

        var row = chatData[ScriptCount];

        nameText.text = row["캐릭터 이름"].ToString();
        chatText.text = row["대사 (스크립트)"].ToString();

        //캐릭터이름이 null인 경우
        if (row["캐릭터 이름"] == null)
        {
            nameText.text = "";
        }

        string eventName = row.ContainsKey("이벤트") ? row["이벤트"].ToString() : "";
        // 이벤트 시작 시 대화 멈춤
        if (!string.IsNullOrEmpty(eventName))
        {
            StartEvent(eventName);
            return; 
        }

        string appearName = row.ContainsKey("존재") ? row["존재"].ToString() : "";
        
        if (!string.IsNullOrEmpty(appearName))
        {
            vroidModel.SetActive(false);
        }

        string animationTrigger = row.ContainsKey("애니메이션") ? row["애니메이션"].ToString() : "";
        PlayAnimation(animationTrigger);

        string currentFadeAnimtionTrigger = row.ContainsKey("페이드") ? row["페이드"].ToString() : "";
        FadeEffect(currentFadeAnimtionTrigger);

        ScriptCount++;

    }


    void PlayAnimation(string triggerName)
    {

        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger(triggerName); // 트리거를 활성화하여 애니메이션 실행
        }

        // 빈 문자열 또는 null인 경우 기본 트리거 호출
        if (string.IsNullOrEmpty(triggerName))
        {
            triggerName = "IdleTrigger"; // 기본 트리거 이름
            characterAnimator.SetTrigger(triggerName);
        }
        
    }

    void FadeEffect(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName) || fadeAnimator == null) return;
        fadeImage.gameObject.SetActive(true); // 페이드 이미지 활성화

        if (triggerName == "FadeBlackTrigger")
        {
            // 화면을 즉시 검게 설정
            fadeImage.gameObject.SetActive(true);
            fadeAnimator.SetTrigger(triggerName);
        }

        else
        {
            fadeImage.gameObject.SetActive(true); // 페이드 이미지 활성화
            fadeAnimator.SetTrigger(triggerName);
        }
    }

    void StartEvent(string eventName)
    {
        if (string.IsNullOrEmpty(eventName) || eventName == null) return;

        currentState = GameMgr.GameState.Event;
        Debug.Log("start event");
        EventMgr.Instance.StartEvent(eventName);
        EventMgr.Instance.OnEventCompleted += OnEventCompleted;

    }

    void OnEventCompleted()
    {
        EventMgr.Instance.OnEventCompleted -= OnEventCompleted; // 이벤트 핸들러 해제
        currentState = GameMgr.GameState.Dialogue;
        Debug.Log("이벤트가 완료되었습니다. 대화를 이어갑니다.");
        DisplayNextSentence();
    }

    void EndDialogue()
    {
        currentState = GameMgr.GameState.Idle;
        ChatGameObject.SetActive(false);
        NameGameObject.SetActive(false);
        SceneManager.LoadScene(nextScene);
        Debug.Log("대화 종료");
    }
}
