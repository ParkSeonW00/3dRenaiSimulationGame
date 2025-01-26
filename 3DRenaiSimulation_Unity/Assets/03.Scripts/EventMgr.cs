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
    public Image chatBox;      // ��ȭâ �ڽ�
    public Image nameBox;          // �̸� �ڽ� 
    public Text chatText;      // ��ȭ ���� �ؽ�Ʈ
    public Text nameText;          // ��ȭ�� �̸� �ؽ�Ʈ

    [Header("Animator")]
    public Animator characterAnimator; // ĳ���� �ִϸ��̼��� ����ϴ� Animator

    [Header("Fade In-Out")]
    public Animator fadeAnimator;    //Fade ���� �ִϸ�����
    public Image fadeImage;

    [Header("Event Data")]
    public string csv_FileName;  // �̺�Ʈ CSV ���ϸ�
    private List<Dictionary<string, object>> eventData;
    private int eventIndex = 0;

    [Header("Event Status")]
    public bool isEventActive = false; // �̺�Ʈ ���� ����

    public delegate void EventCompletedHandler();
    public event EventCompletedHandler OnEventCompleted; // �̺�Ʈ �Ϸ� �� ȣ��Ǵ� ��������Ʈ

    public GameMgr.GameState currentState;

    void Awake()
    {
        // �̱��� ������ ����� �ν��Ͻ� ����
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (currentState == GameMgr.GameState.Event && Input.GetMouseButtonDown(0))
        {
            ShowEventDialogue();  // Ŭ�� �� ��縦 �ѱ⵵�� �߰�
        }
    }

    void Start()
    {

    }

    // �̺�Ʈ �̸��� �´� CSV �����͸� �ε��ϰ� �̺�Ʈ ����
    public void StartEvent(string eventName)
    {
        currentState = GameMgr.GameState.Event;
        Debug.Log($"Loading CSV file: {eventName}");
        eventData = CSVReader.Read(eventName);

        if (eventData == null || eventData.Count == 0)
        {
            Debug.LogError("Event CSV �����Ͱ� ����ֽ��ϴ�!");
            EndEventDialogue();
            return;
        }

        isEventActive = true; // �̺�Ʈ Ȱ��ȭ
        currentEventIndex = 0; // �̺�Ʈ �ε��� �ʱ�ȭ

        Debug.Log($"�̺�Ʈ {eventName} ����");

        chatBox.gameObject.SetActive(true); 
        nameBox.gameObject.SetActive(true);
        chatText.gameObject.SetActive(true); 
        nameText.gameObject.SetActive(true); 

        ShowEventDialogue();
    }

    void ShowEventDialogue()
    {

        if (currentEventIndex >= eventData.Count)
        {
            EndEventDialogue();
            return;
        }

        nameText.text = eventData[currentEventIndex]["ĳ���� �̸�"].ToString();
        chatText.text = eventData[currentEventIndex]["��� (��ũ��Ʈ)"].ToString();

        string selectOption = eventData[currentEventIndex].ContainsKey("������ID") ? eventData[currentEventIndex]["������ID"].ToString() : "";
        if (!string.IsNullOrEmpty(selectOption))
        {
            ShowSelectOption(selectOption);
            return; // �̺�Ʈ ���� �� ��ȭ ����
        }

        string animationOption = eventData[currentEventIndex].ContainsKey("�ִϸ��̼�") ? eventData[currentEventIndex]["�ִϸ��̼�"].ToString() : "";
        PlayAnimation(animationOption);

        string fadeOption = eventData[currentEventIndex].ContainsKey("���̵�") ? eventData[currentEventIndex]["���̵�"].ToString() : "";
        FadeEffect(fadeOption);

        currentEventIndex++;

    }

    void ShowSelectOption(string selectOption)
    {
        currentState = GameMgr.GameState.Select;
        Debug.Log("start select");
        Debug.Log("������ �ɼ�: " + selectOption);

        SelectMgr.Instance.StartSelect(selectOption);


        EndEventDialogue();
    }


    void PlayAnimation(string triggerName)
    {

        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger(triggerName); // Ʈ���Ÿ� Ȱ��ȭ�Ͽ� �ִϸ��̼� ����
        }

        // �� ���ڿ� �Ǵ� null�� ��� �⺻ Ʈ���� ȣ��
        if (string.IsNullOrEmpty(triggerName))
        {
            triggerName = "IdleTrigger"; // �⺻ Ʈ���� �̸�
            characterAnimator.SetTrigger(triggerName);
        }

    }


    void FadeEffect(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName) || fadeAnimator == null) return;
        fadeImage.gameObject.SetActive(true); // ���̵� �̹��� Ȱ��ȭ

        if (triggerName == "FadeBlackTrigger")
        {
            // ȭ���� ��� �˰� ����
            fadeImage.gameObject.SetActive(true);
            fadeAnimator.SetTrigger(triggerName);
        }

        else
        {
            fadeImage.gameObject.SetActive(true); // ���̵� �̹��� Ȱ��ȭ
            fadeAnimator.SetTrigger(triggerName);
        }
    }

    void EndEventDialogue()
    {
        chatBox.gameObject.SetActive(false);
        chatText.gameObject.SetActive(false);
        nameBox.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
        Debug.Log("�̺�Ʈ  ����");

        isEventActive = false; // �̺�Ʈ ��Ȱ��ȭ

    }
}
