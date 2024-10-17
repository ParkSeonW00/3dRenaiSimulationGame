using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshProUGUI, TMP_Text 클래스 사용
using UnityEngine.UI;

public class DialogueMgr : MonoBehaviour
{

    [SerializeField] GameObject go_dialogueBar;
    [SerializeField] GameObject go_NameBar;

    [SerializeField] Text txt_dialogue;
    [SerializeField] Text txt_name;


    bool isNext = false;    // 특정 키 입력 대기를 위한 변수
    int dialogueCnt = 0;    // 대화 카운트. 한 캐릭터가 다 말하면 1 증가
    int contextCnt = 0; 	// 대사 카운트. 한 캐릭터가 여러 대사를 할 수 있다.

    bool isDialogue = false;    // 현재 대화중인지

   
}
