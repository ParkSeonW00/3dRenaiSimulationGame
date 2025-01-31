
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
    public Animator characterAnimator; // ĳ���� �ִϸ��̼��� ����ϴ� Animator

    [Header("CSV Reader")]
    public string csv_FileName;

    [Header("Fade In-Out")]
    public Animator fadeAnimator;    //Fade ���� �ִϸ�����
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
        nameText.text = chatData[ScriptCount].ContainsKey("ĳ���� �̸�") ? chatData[ScriptCount]["ĳ���� �̸�"].ToString() : ""; 
        chatText.text = chatData[ScriptCount].ContainsKey("��� (��ũ��Ʈ)") ? chatData[ScriptCount]["��� (��ũ��Ʈ)"].ToString() : ""; 

        if (chatData == null || chatData.Count == 0)
        {
            Debug.LogError("CSV ������ ���� �� �����ϴ�.");
            return;
        }

        // ���̵� �� �ִϸ��̼� ����
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

        // ��ȭ �����Ͱ� �� �̻� ���°�� EndDialogue
        if (ScriptCount >= chatData.Count)
        {
            string fadeanimationTrigger = chatData[ScriptCount-1].ContainsKey("���̵�") ? chatData[ScriptCount-1]["���̵�"].ToString() : "";
            FadeEffect(fadeanimationTrigger);
            EndDialogue();
            return;
        }

        var row = chatData[ScriptCount];

        nameText.text = row["ĳ���� �̸�"].ToString();
        chatText.text = row["��� (��ũ��Ʈ)"].ToString();

        //ĳ�����̸��� null�� ���
        if (row["ĳ���� �̸�"] == null)
        {
            nameText.text = "";
        }

        string eventName = row.ContainsKey("�̺�Ʈ") ? row["�̺�Ʈ"].ToString() : "";
        // �̺�Ʈ ���� �� ��ȭ ����
        if (!string.IsNullOrEmpty(eventName))
        {
            StartEvent(eventName);
            return; 
        }

        string appearName = row.ContainsKey("����") ? row["����"].ToString() : "";
        
        if (!string.IsNullOrEmpty(appearName))
        {
            vroidModel.SetActive(false);
        }

        string animationTrigger = row.ContainsKey("�ִϸ��̼�") ? row["�ִϸ��̼�"].ToString() : "";
        PlayAnimation(animationTrigger);

        string currentFadeAnimtionTrigger = row.ContainsKey("���̵�") ? row["���̵�"].ToString() : "";
        FadeEffect(currentFadeAnimtionTrigger);

        ScriptCount++;

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
        EventMgr.Instance.OnEventCompleted -= OnEventCompleted; // �̺�Ʈ �ڵ鷯 ����
        currentState = GameMgr.GameState.Dialogue;
        Debug.Log("�̺�Ʈ�� �Ϸ�Ǿ����ϴ�. ��ȭ�� �̾�ϴ�.");
        DisplayNextSentence();
    }

    void EndDialogue()
    {
        currentState = GameMgr.GameState.Idle;
        ChatGameObject.SetActive(false);
        NameGameObject.SetActive(false);
        SceneManager.LoadScene(nextScene);
        Debug.Log("��ȭ ����");
    }
}
