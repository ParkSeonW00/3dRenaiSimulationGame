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
    public Image selectImage;        // 선택지 UI 패널
    public Text selectText;         // 선택지 글자 
    public Button optionButton1;    // 선택지 버튼 1
    public Button optionButton2;    // 선택지 버튼 2
    public Text optionBtnText1;
    public Text optionBtnText2;

    [Header("Notice Elements")]
    public GameObject noticePanel;
    public Image noticeImage;        
    public Text noticeText;         
    public Button noticeButton;    
    public Text noticeBtnText;     

    [Header("Animator")]
    public Animator characterAnimator; // 캐릭터 애니메이션을 담당하는 Animator

    [Header("Fade In-Out")]
    public Animator fadeAnimator;    //Fade 관리 애니메이터
    public Image fadeImage;

    [Header("Select Data")]
    public string selectCSVFile_1;  // 선택 결과 CSV 파일명
    public string selectCSVFile_2;  // 선택 결과 CSV 파일명

    public List<Dictionary<string, object>> selectData;
    private Dictionary<string, List<Dictionary<string, object>>> cachedCSVData = new Dictionary<string, List<Dictionary<string, object>>>();

    [Header("Select Status")]
    public bool isSelectActive = false; // 이벤트 진행 여부

    public delegate void EventCompletedHandler();
    public event EventCompletedHandler OnEventCompleted; // 이벤트 완료 시 호출되는 델리게이트

    public GameMgr.GameState currentState;

    public string nextScene;

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
        // 초기에 CSV 데이터를 캐싱
        CacheCSVData(selectCSVFile_1);
        CacheCSVData(selectCSVFile_2);

        selectPanel.SetActive(false);
        noticePanel.SetActive(false);
        isSelectActive = false;

        // 버튼 클릭 리스너 설정
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

    //선택지 버튼 클릭함수
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
            Debug.LogError("Select CSV 데이터가 비어있습니다!");
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

        selectText.text = selectData[currentSelectIndex]["선택지제목"].ToString();
        optionBtnText1.text = selectData[currentSelectIndex]["선택지 스크립트"].ToString();
        optionBtnText2.text = selectData[currentSelectIndex + 1]["선택지 스크립트"].ToString();

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
        nameText.text = row.ContainsKey("캐릭터 이름") ? row["캐릭터 이름"].ToString() : "";
        chatText.text = row.ContainsKey("대사 (스크립트)") ? row["대사 (스크립트)"].ToString() : "";

        PlayAnimation(row.ContainsKey("애니메이션") ? row["애니메이션"].ToString() : "");
        FadeEffect(row.ContainsKey("페이드") ? row["페이드"].ToString() : "");
        ProcessLikePoints(row.ContainsKey("호감도") ? row["호감도"].ToString() : "");

        currentSelectIndex++;

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
            noticeText.text = "호감도가" + likePoint.ToString() + "이 되었습니다. \n 전체 호감도: "+ (GlobalData.g_LikePoint).ToString();
            
        }
        
    }

    public void EndSelect()
    {
        isSelectActive = false;
        selectPanel.SetActive(false);
        Debug.Log("선택문 종료");
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
