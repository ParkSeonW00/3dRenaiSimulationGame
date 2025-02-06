using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.SceneManagement;

public class EndingMgr : MonoBehaviour
{
    private void Start()
    {
        SelectMgr.Instance.OnEventCompleted += LikePointMgr;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }

    public void LikePointMgr()
    {
        if (GlobalData.g_LikePoint >= 50)
        {
            Debug.Log("해피엔딩시작");
            SceneManager.LoadScene("09-1. EndingHappyScene");
        }
        else
        {
            Debug.Log("새드엔딩시작");
            SceneManager.LoadScene("09-2. EndingSadScene");
        }
    }
}

    

