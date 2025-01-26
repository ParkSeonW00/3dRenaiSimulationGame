
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
    public Image chatBox;      // 대화창 박스
    public Image nameBox;          // 이름 박스 
    public Text chatText;      // 대화 내용 텍스트
    public Text nameText;          // 대화자 이름 텍스트
    

    [Header("Animator")]
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

    //private Queue<Dialogue> dialogueQueue;

    int ScriptCount = 0;
    List<Dictionary<string, object>> data_Dialog;


    void Start()
    {
        data_Dialog = CSVReader.Read(csv_FileName);
        DisplayNextSentence();

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

        List<Dictionary<string, object>> data_Dialog = CSVReader.Read(csv_FileName);

        // 대화 데이터가 더 이상 없으면 EndDialogue 호출
        if (ScriptCount >= data_Dialog.Count)
        {
            string fadeanimationTrigger = data_Dialog[ScriptCount - 1].ContainsKey("페이드") ? data_Dialog[ScriptCount - 1]["페이드"].ToString() : "";
            FadeEffect(fadeanimationTrigger);
            EndDialogue();
            return;
        }

        nameText.text = data_Dialog[ScriptCount]["캐릭터 이름"].ToString();
        chatText.text = data_Dialog[ScriptCount]["대사 (스크립트)"].ToString();

        //캐릭터이름이 null인 경우
        if (data_Dialog[ScriptCount]["캐릭터 이름"] == null)
        {
            nameText.text = "";
        }

        string eventName = data_Dialog[ScriptCount].ContainsKey("이벤트") ? data_Dialog[ScriptCount]["이벤트"].ToString() : "";


        string animationTrigger = data_Dialog[ScriptCount].ContainsKey("애니메이션") ? data_Dialog[ScriptCount]["애니메이션"].ToString() : "";
        PlayAnimation(animationTrigger);

        string currentFadeAnimtionTrigger = data_Dialog[ScriptCount].ContainsKey("페이드") ? data_Dialog[ScriptCount]["페이드"].ToString() : "";
        FadeEffect(currentFadeAnimtionTrigger);

        if (!string.IsNullOrEmpty(eventName))
        {
            StartEvent(eventName);
            return; // 이벤트 시작 시 대화 멈춤
        }

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
        chatBox.gameObject.SetActive(false);
        chatText.gameObject.SetActive(false);
        nameBox.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
        SceneManager.LoadScene(nextScene);
        Debug.Log("대화 종료");
    }
}
