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
    public Image selectImage;        // 선택지 UI 패널
    public Text selectText;         // 선택지 글자 
    public Button optionButton1;    // 선택지 버튼 1
    public Button optionButton2;    // 선택지 버튼 2
    public Text optionBtnText1;
    public Text optionBtnText2;

    [Header("UI Elements")]
    public Image chatBox;      // 대화창 박스
    public Image nameBox;          // 이름 박스 
    public Text chatText;      // 대화 내용 텍스트
    public Text nameText;      // 대화 내용 텍스트

    [Header("Animator")]
    public Animator characterAnimator; // 캐릭터 애니메이션을 담당하는 Animator

    [Header("Fade In-Out")]
    public Animator fadeAnimator;    //Fade 관리 애니메이터
    public Image fadeImage;

    [Header("Select Data")]
    public string selectCSVFile_1;  // 선택 결과 CSV 파일명
    public string selectCSVFile_2;  // 선택 결과 CSV 파일명

    private List<Dictionary<string, object>> selectData;

    [Header("Select Status")]
    public bool isSelectActive = false; // 이벤트 진행 여부

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
        if (isSelectActive==true && Input.GetMouseButtonDown(0))
        {
            DisplayNextText();
        }
    }

    void Start()
    {
    

    }

    // 이벤트 이름에 맞는 CSV 데이터를 로드하고 이벤트 시작
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
        optionBtnText1.text = selectData[currentSelectIndex]["선택지 스크립트"].ToString();
        optionBtnText2.text = selectData[currentSelectIndex + 1]["선택지 스크립트"].ToString();
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

        nameText.text = selectData[currentSelectIndex]["캐릭터 이름"].ToString();
        chatText.text = selectData[currentSelectIndex]["대사 (스크립트)"].ToString();

        string animationSelect = selectData[currentSelectIndex].ContainsKey("애니메이션") ? selectData[currentSelectIndex]["애니메이션"].ToString() : "";
        PlayAnimation(animationSelect);

        string fadeSelect = selectData[currentSelectIndex].ContainsKey("페이드") ? selectData[currentSelectIndex]["페이드"].ToString() : "";
        FadeEffect(fadeSelect);

        string likePoint = selectData[currentSelectIndex].ContainsKey("호감도") ? selectData[currentSelectIndex]["호감도"].ToString() : "";
        LikeSelect(likePoint);

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


    void LikeSelect(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName)) return;
        fadeImage.gameObject.SetActive(true); // 페이드 이미지 활성화

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
        isSelectActive = false; // 선택 비활성화
        Debug.Log("선택문 종료");
    }
}
