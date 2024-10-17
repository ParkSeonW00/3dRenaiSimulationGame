using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialogueParser : MonoBehaviour
{
    private List<Dialgoue> dialgoueList; // 대사 리스트

    private void Start()
    {
        Parse("Test_csv");
    }

    public Dialgoue[] Parse(string _CSVFileName)
    {
        List<Dialgoue> dialgoueList = new List<Dialgoue>(); //대사 리스트 생성 
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);

        string[] data = csvData.text.Split(new char[] { '\n' });

        for(int i = 1; i < data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' });

            Dialgoue dialgoue = new Dialgoue(); //캐릭터 한명의 대사 생성

            dialgoue.name = row[1];
            Debug.Log(row[1]);

            List<string> contextList = new List<string>();
            do 
            {
                contextList.Add(row[2]);
                Debug.Log(row[2]);
                if (++i < data.Length)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }
                
            } while (row[0].ToString() == "");

            dialgoue.contexts = contextList.ToArray();  //대사를 대사 배열로 바꿈 
            dialgoueList.Add(dialgoue);                 //이름, 대사 < 세트로 반환 

        }

        return dialgoueList.ToArray();
    }

    
}
