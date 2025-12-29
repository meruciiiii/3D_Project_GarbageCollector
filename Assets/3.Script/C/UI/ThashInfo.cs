using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class ThashInfo : MonoBehaviour
{
    [Header("UI 연결 설정")]
    [Tooltip("텍스트와 패널을 포함하는 부모 오브젝트 (배경 패널)")]
    [SerializeField] private GameObject uiPanel;

    [Tooltip("쓰레기 이름이 표시될 텍스트")]
    [SerializeField] private Text nameText;

    [Tooltip("무게, 힘, 청결도 등 상세 정보가 표시될 텍스트")]
    [SerializeField] private Text infoText;

    private Trash lastHitTrash = null;
    private StringBuilder infoBuilder = new StringBuilder();

    private void Start()
    {
        HideInfo();
    }

    public void UpdateTooltip(GameObject targetObj)
    {
        // 1. 타겟이 없으면 숨기기
        if (targetObj == null)
        {
            HideInfo();
            return;
        }

        // 2. Trash 컴포넌트 확인 (최적화를 위해 GetComponent 캐싱 고려 가능하지만, UI 갱신 빈도상 유지)
        Trash trash = targetObj.GetComponent<Trash>();
        if (trash == null)
        {
            HideInfo();
            return;
        }

        // 3. 정보 표시 실행
        ShowInfo(trash);
    }

    private void ShowInfo(Trash trash)
    {
        // 최적화: 이미 켜져 있고 같은 대상을 보고 있다면 UI 갱신 건너뜀
        if (uiPanel != null && uiPanel.activeSelf && lastHitTrash == trash) return;

        lastHitTrash = trash;

        // 키값 생성
        string keyPrefix = "";
        switch (trash.Size)
        {
            case Trash.TrashSize.Small: keyPrefix = "small_"; break;
            case Trash.TrashSize.Large: keyPrefix = "large_"; break;
            case Trash.TrashSize.Human: keyPrefix = "human_"; break;
        }
        string finalKey = keyPrefix + trash.TrashNum;

        if (CSV_Database.instance == null || GameManager.instance == null) return;

        // 데이터 가져오기
        string tName = CSV_Database.instance.getname(finalKey);
        int tWeight = CSV_Database.instance.getweight(finalKey);
        int reqStr = CSV_Database.instance.getrequireStr(finalKey);
        int hpCost = CSV_Database.instance.getHpdecrease(finalKey);
        int currentStr = GameManager.instance.P_Str;

        // 이름 설정
        if (nameText != null) nameText.text = tName;

        // 상세 정보 설정
        if (infoText != null)
        {
            infoBuilder.Clear();
            bool isEng = GameManager.instance.P_isEnglish;

            // 무게
            infoBuilder.AppendLine(isEng ? $"weight: {tWeight} g" : $"무게: {tWeight} g");

            // 힘 요구량
            if (currentStr >= reqStr)
            {
                infoBuilder.AppendLine(isEng ? $"require str: {reqStr}" : $"필요 힘: {reqStr}");
            }
            else
            {
                // 붉은색 경고
                string warning = isEng ? "can't hold!" : "힘 부족!";
                infoBuilder.Append($"<color=red>{warning} ({currentStr}/{reqStr})</color>\n");
            }

            // 청결도 소모
            if (hpCost > 0)
            {
                string costText = isEng ? "CP" : "청결도";
                infoBuilder.Append($"{costText}: <color=orange>-{hpCost}</color>");
            }

            infoText.text = infoBuilder.ToString();
        }

        if (uiPanel != null) uiPanel.SetActive(true);
    }

    public void HideInfo()
    {
        // 패널 전체를 끄면 자식 텍스트들도 같이 꺼집니다.
        if (uiPanel != null && uiPanel.activeSelf)
        {
            uiPanel.SetActive(false);
            lastHitTrash = null;
        }
    }
}
