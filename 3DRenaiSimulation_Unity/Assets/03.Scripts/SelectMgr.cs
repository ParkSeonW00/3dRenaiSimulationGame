using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMgr : MonoBehaviour
{
    public static SelectMgr Instance;
    public int currentSelectIndex = 0;
    bool isFinished = false;

    [Header("UI Elements")]
    public GameObject ChatGameObject;
    public GameObject NameGameObject;
    public Image chatBox;
    public Image nameBox;
    public Text chatText;
    public Text nameText;

    [Header("Select Elements")]
    public GameObject selectPanel;
    public Image selectImage;        // ������ UI �г�
    public Text selectText;         // ������ ���� 
    public Button optionButton1;    // ������ ��ư 1
    public Button optionButton2;    // ������ ��ư 2
    public Text optionBtnText1;
    public Text optionBtnText2;

    [Header("Notice Elements")]
    public GameObject noticePanel;
    public Image noticeImage;        
    public Text noticeText;         
    public Button noticeButton;    
    public Text noticeBtnText;     

    [Header("Animator")]
    public Animator characterAnimator; // ĳ���� �ִϸ��̼��� ����ϴ� Animator

    [Header("Fade In-Out")]
    public Animator fadeAnimator;    //Fade ���� �ִϸ�����
    public Image fadeImage;

    [Header("Select Data")]
    public string selectCSVFile_1;  // ���� ��� CSV ���ϸ�
    public string selectCSVFile_2;  // ���� ��� CSV ���ϸ�

    public List<Dictionary<string, object>> selectData;
    private Dictionary<string, List<Dictionary<string, object>>> cachedCSVData = new Dictionary<string, List<Dictionary<string, object>>>();

    [Header("Select Status")]
    public bool isSelectActive = false; // �̺�Ʈ ���� ����

    public delegate void EventCompletedHandler();
    public event EventCompletedHandler OnEventCompleted; // �̺�Ʈ �Ϸ� �� ȣ��Ǵ� ��������Ʈ

    public GameMgr.GameState currentState;

    public string nextScene;

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
        if (isSelectActive==true && Input.GetMouseButtonDown(0))
        {
            DisplayNextText();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void Start()
    {
        isFinished = false;
        // �ʱ⿡ CSV �����͸� ĳ��
        CacheCSVData(selectCSVFile_1);
        CacheCSVData(selectCSVFile_2);

        selectPanel.SetActive(false);
        noticePanel.SetActive(false);
        isSelectActive = false;

        // ��ư Ŭ�� ������ ����
        optionButton1.onClick.AddListener(() => SelectClick(selectCSVFile_1));
        optionButton2.onClick.AddListener(() => SelectClick(selectCSVFile_2));

    }

    void CacheCSVData(string csvFileName)
    {
        if (!cachedCSVData.ContainsKey(csvFileName))
        {
            var data = CSVReader.Read(csvFileName);
            cachedCSVData[csvFileName] = data ?? new List<Dictionary<string, object>>();
        }
    }

    //������ ��ư Ŭ���Լ�
    void SelectClick(string csvFileName)
    {
        selectData = cachedCSVData[csvFileName];
        currentSelectIndex = 0;
        isSelectActive = true;
        this.selectPanel.SetActive(false);

        ChatGameObject.SetActive(true);
        NameGameObject.SetActive(true);

        DisplayNextText();
    }

    public void StartSelect(string selectName)
    {
        isSelectActive = true;
        selectData = CSVReader.Read(selectName);

        if (selectData == null || selectData.Count == 0)
        {
            Debug.LogError("Select CSV �����Ͱ� ����ֽ��ϴ�!");
            return;
        }
        currentSelectIndex = 0;

        this.selectPanel.SetActive(true);

        ShowUIPanel();
    }

    void ShowUIPanel()
    {
        isSelectActive = false;
        if (currentSelectIndex + 1 >= selectData.Count) return;

        selectText.text = selectData[currentSelectIndex]["����������"].ToString();
        optionBtnText1.text = selectData[currentSelectIndex]["������ ��ũ��Ʈ"].ToString();
        optionBtnText2.text = selectData[currentSelectIndex + 1]["������ ��ũ��Ʈ"].ToString();

    }

    void DisplayNextText()
    {
        isSelectActive = true;
        if (currentSelectIndex >= selectData.Count)
        {
            EndSelect();
            return;
        }

        var row = selectData[currentSelectIndex];
        nameText.text = row.ContainsKey("ĳ���� �̸�") ? row["ĳ���� �̸�"].ToString() : "";
        chatText.text = row.ContainsKey("��� (��ũ��Ʈ)") ? row["��� (��ũ��Ʈ)"].ToString() : "";

        PlayAnimation(row.ContainsKey("�ִϸ��̼�") ? row["�ִϸ��̼�"].ToString() : "");
        FadeEffect(row.ContainsKey("���̵�") ? row["���̵�"].ToString() : "");
        ProcessLikePoints(row.ContainsKey("ȣ����") ? row["ȣ����"].ToString() : "");

        currentSelectIndex++;

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
        if (fadeAnimator == null || string.IsNullOrEmpty(triggerName)) return;

        fadeImage.gameObject.SetActive(true);
        fadeAnimator.SetTrigger(triggerName);
    }


    void ProcessLikePoints(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName)) return;

        else
        {
            int likePoint = int.Parse(triggerName);
            GlobalData.g_LikePoint += likePoint;
            noticeText.text = "ȣ������" + likePoint.ToString() + "�� �Ǿ����ϴ�. \n ��ü ȣ����: "+ (GlobalData.g_LikePoint).ToString();
            
        }
        
    }

    public void EndSelect()
    {
        isSelectActive = false;
        selectPanel.SetActive(false);
        Debug.Log("���ù� ����");
        noticePanel.SetActive(true);
        noticeButton.onClick.AddListener(noticeBtnClick);
        isFinished = true;
        OnEventCompleted?.Invoke();

    }

    void noticeBtnClick()
    {
        isSelectActive = false;
        noticePanel.gameObject.SetActive(true);
        Debug.Log("noticeBtnClick");
        SceneManager.LoadScene(nextScene);
    }
}
