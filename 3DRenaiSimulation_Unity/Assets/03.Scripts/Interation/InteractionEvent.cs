using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] DialgoueEvent dialgoue;

    //���°�ٺ��� ���°�ٱ��� �������� 

    public Dialgoue[] GetDialgoue()
    {
        dialgoue.dialgoues = DataBaseMgr.instance.GetDialogue((int)dialgoue.line.x, (int)dialgoue.line.y);
        return dialgoue.dialgoues;
    }

}
