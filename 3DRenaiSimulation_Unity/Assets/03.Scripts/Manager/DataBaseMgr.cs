using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBaseMgr : MonoBehaviour
{
    public static DataBaseMgr instance;

    [SerializeField] string csv_FileName;

    //이벤트 번호 - 이벤트에 맞는 데이터 대사 
    Dictionary<int, Dialgoue> dialogueDic = new Dictionary<int, Dialgoue>();
    //파싱한 데이터를 모두 저장했는지
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
                //키는 1부터 시작
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
