using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBaseMgr : MonoBehaviour
{
    public static DataBaseMgr instance;

    [SerializeField] string csv_FileName;

    //�̺�Ʈ ��ȣ - �̺�Ʈ�� �´� ������ ��� 
    Dictionary<int, Dialgoue> dialogueDic = new Dictionary<int, Dialgoue>();
    //�Ľ��� �����͸� ��� �����ߴ���
    public static bool isFinish = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DialogueParser theParser = GetComponent<DialogueParser>();
            Dialgoue[] dialogues = theParser.Parse(csv_FileName);

            for(int i = 0; i < dialogues.Length; i++)
            {
                //Ű�� 1���� ����
                dialogueDic.Add(i + 1, dialogues[i]);
            }
            isFinish = true;
        }
    }

    public Dialgoue[] GetDialogue(int _StartNum, int _EndNum)
    {
        List<Dialgoue> dialogueList = new List<Dialgoue>();

        for (int i = _StartNum; i <= _EndNum; i++)
        {
            dialogueList.Add(dialogueDic[i]);
        }

        return dialogueList.ToArray();
    }

}
