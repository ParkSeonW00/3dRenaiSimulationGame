using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshProUGUI, TMP_Text Ŭ���� ���
using UnityEngine.UI;

public class DialogueMgr : MonoBehaviour
{

    [SerializeField] GameObject go_dialogueBar;
    [SerializeField] GameObject go_NameBar;

    [SerializeField] Text txt_dialogue;
    [SerializeField] Text txt_name;


    bool isNext = false;    // Ư�� Ű �Է� ��⸦ ���� ����
    int dialogueCnt = 0;    // ��ȭ ī��Ʈ. �� ĳ���Ͱ� �� ���ϸ� 1 ����
    int contextCnt = 0; 	// ��� ī��Ʈ. �� ĳ���Ͱ� ���� ��縦 �� �� �ִ�.

    bool isDialogue = false;    // ���� ��ȭ������

   
}
