using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Text_CSV_Read : MonoBehaviour
{
    [SerializeField] private Text text_component;
    [SerializeField] private string inputkey; //첫번째 키 Key를 외부에서 넣어주세요

    //딸깍으로 사용하기 위해 편의상 넣어둔 bool값, string, Enum 값으로해도 됩니다. 두번째 키
    [SerializeField] private bool isKeyname;
    [SerializeField] private bool isValue_Content;
    [SerializeField] private bool isValue_Price;
    private string Key2;

    private Coroutine loadCoroutine; // 실행 중인 Coroutine

    private void Awake()
    {
        if (isKeyname) Key2 = "Keyname";
        else if (isValue_Content) Key2 = "Value_Content";
        else if (isValue_Price) Key2 = "Value_Price";
    }

    private IEnumerator WaitForDataAndApplyText()
    {
        while (GameManager.instance == null || !GameManager.instance.IsInitialLoadComplete) yield return null;
        while (CSV_Database.instance==null || !CSV_Database.instance.IsLoaded) yield return null;

        if (string.IsNullOrEmpty(inputkey) || string.IsNullOrEmpty(Key2))
        {
            Debug.LogWarning($"Text_CSV_Read: {gameObject.name}의 inputkey 또는 Key2 설정이 없습니다.");
            yield break;
        }
        if (CSV_Database.instance == null || CSV_Database.instance.DataMap == null)
        {
            //CSV_Database.instance 싱글톤으로 뺄거고 메인 화면에서 설정하고 인게임에선 null이 아닐거기 때문에 인게임에서 UI불러오는데 문제 없을겁니다.
            //언어 선택 창에서는 언어 변경돼는 Text UI가 있으면 안돼겠네요.
            //-> 이거 코루틴으로 null이면 계속 대기 시켜서 처리 했습니다. 문제 없습니다.
            Debug.LogError($"CSV_Database.instance, DatamAmp이 null입니다. {gameObject.name} Text 업데이트 불가능합니다..");
            yield break;
        }
        if (CSV_Database.instance.DataMap.TryGetValue(inputkey, out Dictionary<string, object> Data))
        {
            if (Data.ContainsKey(Key2))
            {
                if (text_component != null)
                {
                    // 데이터 안전하게 ToString() 처리
                    text_component.text = Data[Key2].ToString();
                }
            }
            else
            {
                // 컬럼 키를 찾을 수 없을 때
                if (text_component != null) text_component.text = $"컬럼 오류: {Key2}";
                Debug.LogError($"Text_CSV_Read: {inputkey} 데이터에 컬럼 '{Key2}'가 없습니다. CSV 파일 확인 필요.");
            }
        }
        else
        {
            // inputkey (첫 번째 키)를 찾을 수 없을 때
            if (text_component != null) text_component.text = $"키 오류: {inputkey}";
            Debug.LogError($"Text_CSV_Read: '{inputkey}' Key를 가진 데이터를 DataMap에서 찾을 수 없습니다.");
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
