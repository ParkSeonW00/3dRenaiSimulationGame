using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectMgr : MonoBehaviour
{
    public static SelectMgr Instance;
    private int currentSelectIndex = 0;

    [Header("Select Elements")]
    public GameObject selectPanel;
    public Image selectImage;        // ������ UI �г�
    public Text selectText;         // ������ ���� 
    public Button optionButton1;    // ������ ��ư 1
    public Button optionButton2;    // ������ ��ư 2
    public Text optionBtnText1;
    public Text optionBtnText2;

    [Header("UI Elements")]
    public Image chatBox;      // ��ȭâ �ڽ�
    public Image nameBox;          // �̸� �ڽ� 
    public Text chatText;      // ��ȭ ���� �ؽ�Ʈ
    public Text nameText;      // ��ȭ ���� �ؽ�Ʈ

    [Header("Animator")]
    public Animator characterAnimator; // ĳ���� �ִϸ��̼��� ����ϴ� Animator

    [Header("Fade In-Out")]
    public Animator fadeAnimator;    //Fade ���� �ִϸ�����
    public Image fadeImage;

    [Header("Select Data")]
    public string selectCSVFile_1;  // ���� ��� CSV ���ϸ�
    public string selectCSVFile_2;  // ���� ��� CSV ���ϸ�

    private List<Dictionary<string, object>> selectData;

    [Header("Select Status")]
    public bool isSelectActive = false; // �̺�Ʈ ���� ����

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
        if (isSelectActive==true && Input.GetMouseButtonDown(0))
        {
            DisplayNextText();
        }
    }

    void Start()
    {
    

    }

    // �̺�Ʈ �̸��� �´� CSV �����͸� �ε��ϰ� �̺�Ʈ ����
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
        optionBtnText1.text = selectData[currentSelectIndex]["������ ��ũ��Ʈ"].ToString();
        optionBtnText2.text = selectData[currentSelectIndex + 1]["������ ��ũ��Ʈ"].ToString();
        isSelectActive = false;
        ShowSelectOptions();

    }


    void ShowSelectOptions()
    { 
        optionButton1.onClick.AddListener(optionBtn1);
        optionButton2.onClick.AddListener(optionBtn2);

    }

    void OnUI()
    {
        this.selectPanel.gameObject.SetActive(false);
        chatBox.gameObject.SetActive(true);
        chatText.gameObject.SetActive(true);
        nameBox.gameObject.SetActive(true);
        nameText.gameObject.SetActive(true);
    }

    void optionBtn1()
    {
        OnUI();
        isSelectActive = true;
        List<Dictionary<string, object>> selectData1 = CSVReader.Read(selectCSVFile_1);
        selectData = selectData1;

        DisplayNextText();

    }

    void optionBtn2()
    {
        OnUI();
        isSelectActive = true;
        List<Dictionary<string, object>> selectData2 = CSVReader.Read(selectCSVFile_2);
        selectData = selectData2;

        DisplayNextText();
   
    }

    void DisplayNextText()
    {
        if (currentSelectIndex >= selectData.Count)
        {
            EndSelect();
            return;
        }

        nameText.text = selectData[currentSelectIndex]["ĳ���� �̸�"].ToString();
        chatText.text = selectData[currentSelectIndex]["��� (��ũ��Ʈ)"].ToString();

        string animationSelect = selectData[currentSelectIndex].ContainsKey("�ִϸ��̼�") ? selectData[currentSelectIndex]["�ִϸ��̼�"].ToString() : "";
        PlayAnimation(animationSelect);

        string fadeSelect = selectData[currentSelectIndex].ContainsKey("���̵�") ? selectData[currentSelectIndex]["���̵�"].ToString() : "";
        FadeEffect(fadeSelect);

        string likePoint = selectData[currentSelectIndex].ContainsKey("ȣ����") ? selectData[currentSelectIndex]["ȣ����"].ToString() : "";
        LikeSelect(likePoint);

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


    void LikeSelect(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName)) return;
        fadeImage.gameObject.SetActive(true); // ���̵� �̹��� Ȱ��ȭ

        if (triggerName == null)
        {
            return;
        }

        else
        {
            Debug.Log(int.Parse(triggerName));
        }

    }

    void EndSelect()
    {
        selectPanel.gameObject.SetActive(false);
        isSelectActive = false; // ���� ��Ȱ��ȭ
        Debug.Log("���ù� ����");
    }
}
