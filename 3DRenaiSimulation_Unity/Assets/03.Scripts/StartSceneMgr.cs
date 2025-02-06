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

    bool isStart = true;        //ó������ �� ����
    bool hasStarted = false;  // ������ ���۵ƴ��� ����

    // Start is called before the first frame update
    void Start()
    {

        Application.targetFrameRate = 300;
        MenuBtn.onClick.AddListener(MenuBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space)))
        {
            SceneManager.LoadScene("02. Scene1");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }

    void MenuBtnClick()
    {
        MenuPanel.gameObject.SetActive(!MenuPanel.activeSelf);
        if (MenuPanel.activeSelf)
        {
            CreditBtn.onClick.AddListener(CreditBtnClick);
            ExitBtn.onClick.AddListener(ExitBtnClick);
        }
        else
        {

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
