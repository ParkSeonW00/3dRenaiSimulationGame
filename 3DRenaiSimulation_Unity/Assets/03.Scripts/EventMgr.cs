using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EventMgr : MonoBehaviour
{
    public static EventMgr Instance;
    private int currentEventIndex = 0;

    [Header("UI Elements")]
    public GameObject ChatGameObject;
    public GameObject NameGameObject;
    public Image chatBox;
    public Image nameBox;
    public Text chatText;
    public Text nameText;

    [Header("Animator")]
    public Animator characterAnimator; // 캐릭터 애니메이션을 담당하는 Animator

    [Header("Fade In-Out")]
    public Animator fadeAnimator;    //Fade 관리 애니메이터
    public Image fadeImage;

    [Header("Event Data")]
    public string csv_FileName;  // 이벤트 CSV 파일명
    private List<Dictionary<string, object>> eventData;
    private int eventIndex = 0;

    [Header("Event Status")]
    public bool isEventActive = false; // 이벤트 진행 여부

    public delegate void EventCompletedHandler();
    public event EventCompletedHandler OnEventCompleted; // 이벤트 완료 시 호출되는 델리게이트

    public GameMgr.GameState currentState;

    void Awake()
    {
        // 싱글턴 패턴을 사용해 인스턴스 관리
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (currentState == GameMgr.GameState.Event && Input.GetMouseButtonDown(0))
        {
            ShowEventDialogue();  // 클릭 시 대사를 넘기도록 추가
        }
    }

    void Start()
    {

    }

    // 이벤트 이름에 맞는 CSV 데이터를 로드하고 이벤트 시작
    public void StartEvent(string eventName)
    {
        currentState = GameMgr.GameState.Event;
        Debug.Log($"Loading CSV file: {eventName}");
        eventData = CSVReader.Read(eventName);

        if (eventData == null || eventData.Count == 0)
        {
            Debug.LogError("Event CSV 데이터가 비어있습니다!");
            EndEventDialogue();
            return;
        }

        isEventActive = true; // 이벤트 활성화
        currentEventIndex = 0; // 이벤트 인덱스 초기화

        Debug.Log($"이벤트 {eventName} 시작");

        ChatGameObject.SetActive(true);
        NameGameObject.SetActive(true);

        ShowEventDialogue();
    }

    void ShowEventDialogue()
    {

        if (currentEventIndex >= eventData.Count)
        {
            EndEventDialogue();
            return;
        }
        var row = eventData[currentEventIndex];
        nameText.text = row["캐릭터 이름"].ToString();
        chatText.text = row["대사 (스크립트)"].ToString();

        //캐릭터이름이 null인 경우
        if (eventData[currentEventIndex]["캐릭터 이름"] == null)
        {
            nameText.text = "";
        }

        string selectOption = row.ContainsKey("선택지ID") ? row["선택지ID"].ToString() : "";
        if (!string.IsNullOrEmpty(selectOption))
        {
            ShowSelectOption(selectOption);
            return; // 이벤트 시작 시 대화 멈춤
        }

        PlayAnimation(row.ContainsKey("애니메이션") ? row["애니메이션"].ToString() : "");

        FadeEffect(row.ContainsKey("페이드") ? row["페이드"].ToString() : "");

        currentEventIndex++;

    }

    void ShowSelectOption(string selectOption)
    {
        currentState = GameMgr.GameState.Select;
        Debug.Log("start select");
        Debug.Log("선택한 옵션: " + selectOption);

        SelectMgr.Instance.StartSelect(selectOption);


        EndEventDialogue();
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

    void EndEventDialogue()
    {
        ChatGameObject.SetActive(false);
        NameGameObject.SetActive(false);
        Debug.Log("이벤트  종료");

        isEventActive = false; // 이벤트 비활성화

    }
}
