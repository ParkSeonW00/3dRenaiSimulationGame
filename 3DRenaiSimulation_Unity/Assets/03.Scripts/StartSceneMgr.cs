using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneMgr : MonoBehaviour
{
    public Button MenuBtn;
    public GameObject MenuPanel;
    public Button CreditBtn;
    public Button ExitBtn;

    bool isStart = true;        //처음에는 탭 가능
    bool hasStarted = false;  // 게임이 시작됐는지 여부

    // Start is called before the first frame update
    void Start()
    {
        isStart = true;
        hasStarted = true;
        Application.targetFrameRate = 300;
        MenuBtn.onClick.AddListener(MenuBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStarted == true && Input.GetMouseButtonDown(0))
        {
            hasStarted = true;
            SceneManager.LoadScene("02. Scene1");
        }
    }

    void MenuBtnClick()
    {
        isStart = false;
 
        MenuPanel.gameObject.SetActive(!MenuPanel.activeSelf);
        if (MenuPanel.activeSelf)
        {
            isStart = false; 
            CreditBtn.onClick.AddListener(CreditBtnClick);
            ExitBtn.onClick.AddListener(ExitBtnClick);
        }
        else
        {
            isStart = false;
            hasStarted = true;
        }
    }

    void CreditBtnClick()
    {
        SceneManager.LoadScene("10. CreditScene");
    }

    void ExitBtnClick()
    {
        GameExit();
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
