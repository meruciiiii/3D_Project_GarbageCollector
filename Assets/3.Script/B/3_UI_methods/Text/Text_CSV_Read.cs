using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Text_CSV_Read : MonoBehaviour
{
    [SerializeField] private Text text_component;
    [SerializeField] private string Key1; //첫번째 키 Key를 외부에서 넣어주세요

    private string Key2 = "value";

    private Coroutine loadCoroutine; // 실행 중인 Coroutine

    private IEnumerator WaitForDataAndApplyText()
    {
        while (GameManager.instance == null || !GameManager.instance.LoadComplete) yield return null;
        while (CSV_Database.instance==null || !CSV_Database.instance.IsLoaded) yield return null;

        if (string.IsNullOrEmpty(Key1))
        {
            Debug.LogWarning($"Text_CSV_Read: {gameObject.name}의 inputkey 설정이 없습니다.");
            yield break;
        }
        if (CSV_Database.instance == null || CSV_Database.instance.DataMap == null)
        {
            Debug.LogError($"CSV_Database.instance, DatamAmp이 null입니다. {gameObject.name} Text 업데이트 불가능합니다..");
            yield break;
        }
        if (CSV_Database.instance.DataMap.TryGetValue(Key1, out Dictionary<string, object> Data))
        {
            if (Data.ContainsKey(Key2))
            {
                if (text_component != null)
                {
                    string rawText = Data[Key2].ToString();
                    text_component.text = rawText.Replace("|", "\n");
                }
            }
        }
        else
        {
            // inputkey (첫 번째 키)를 찾을 수 없을 때
            if (text_component != null) text_component.text = $"오류: {Key1}";
            Debug.LogError($"Text_CSV_Read: '{Key1}' Key를 가진 데이터를 DataMap에서 찾을 수 없습니다.");
        }
    }
    void OnEnable()
    {
        if (loadCoroutine != null) StopCoroutine(loadCoroutine);
       
        loadCoroutine = StartCoroutine(WaitForDataAndApplyText());
    }

    void OnDisable()
    {
        // 오브젝트가 비활성화될 때 코루틴도 중지하여 불필요한 실행 방지
        if (loadCoroutine != null) StopCoroutine(loadCoroutine);
    }
}
