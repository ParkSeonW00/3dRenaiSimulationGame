
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
    public Image chatBox;      // ��ȭâ �ڽ�
    public Image nameBox;          // �̸� �ڽ� 
    public Text chatText;      // ��ȭ ���� �ؽ�Ʈ
    public Text nameText;          // ��ȭ�� �̸� �ؽ�Ʈ
    

    [Header("Animator")]
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

    //private Queue<Dialogue> dialogueQueue;

    int ScriptCount = 0;
    List<Dictionary<string, object>> data_Dialog;


    void Start()
    {
        data_Dialog = CSVReader.Read(csv_FileName);
        DisplayNextSentence();

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

        List<Dictionary<string, object>> data_Dialog = CSVReader.Read(csv_FileName);

        // ��ȭ �����Ͱ� �� �̻� ������ EndDialogue ȣ��
        if (ScriptCount >= data_Dialog.Count)
        {
            string fadeanimationTrigger = data_Dialog[ScriptCount - 1].ContainsKey("���̵�") ? data_Dialog[ScriptCount - 1]["���̵�"].ToString() : "";
            FadeEffect(fadeanimationTrigger);
            EndDialogue();
            return;
        }

        nameText.text = data_Dialog[ScriptCount]["ĳ���� �̸�"].ToString();
        chatText.text = data_Dialog[ScriptCount]["��� (��ũ��Ʈ)"].ToString();

        //ĳ�����̸��� null�� ���
        if (data_Dialog[ScriptCount]["ĳ���� �̸�"] == null)
        {
            nameText.text = "";
        }

        string eventName = data_Dialog[ScriptCount].ContainsKey("�̺�Ʈ") ? data_Dialog[ScriptCount]["�̺�Ʈ"].ToString() : "";


        string animationTrigger = data_Dialog[ScriptCount].ContainsKey("�ִϸ��̼�") ? data_Dialog[ScriptCount]["�ִϸ��̼�"].ToString() : "";
        PlayAnimation(animationTrigger);

        string currentFadeAnimtionTrigger = data_Dialog[ScriptCount].ContainsKey("���̵�") ? data_Dialog[ScriptCount]["���̵�"].ToString() : "";
        FadeEffect(currentFadeAnimtionTrigger);

        if (!string.IsNullOrEmpty(eventName))
        {
            StartEvent(eventName);
            return; // �̺�Ʈ ���� �� ��ȭ ����
        }

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
        chatBox.gameObject.SetActive(false);
        chatText.gameObject.SetActive(false);
        nameBox.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
        SceneManager.LoadScene(nextScene);
        Debug.Log("��ȭ ����");
    }
}
