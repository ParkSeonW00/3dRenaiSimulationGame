using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CreditMgr : MonoBehaviour
{
    public Text creditsText;          
    public float scrollSpeed = 500f;    
    public RectTransform CreditViewport;
    public GameObject ScrollView;
    float endPositionY = 1000f;

    void Update()
    {
        CreditViewport.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

        if (CreditViewport.anchoredPosition.y >= endPositionY)
        {
            ScrollView.SetActive(false);
            GameExit();
        }
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
