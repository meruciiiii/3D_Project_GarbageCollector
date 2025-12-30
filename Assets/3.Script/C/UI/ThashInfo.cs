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

    private GameObject[] currentTargets = null; // 현재 보고 있는 타겟 목록 저장
    private StringBuilder infoBuilder = new StringBuilder();

    private void Start()
    {
        HideInfo();
    }

    public void UpdateTooltip(GameObject[] targets)
    {
        if (targets == null || targets.Length == 0)
        {
            HideInfo();
            return;
        }

        if (targets[0] == null)
        {
            HideInfo();
            return;
        }

        ShowInfo(targets);
    }

    private void ShowInfo(GameObject[] targets)
    {
        if (uiPanel != null && uiPanel.activeSelf && currentTargets == targets) return;

        currentTargets = targets;

        if (CSV_Database.instance == null || GameManager.instance == null) return;

        int totalWeight = 0;
        int maxReqStr = 0;
        int totalHpCost = 0;

        Trash firstTrash = targets[0].GetComponent<Trash>();
        string firstTrashName = "";

        // [핵심 변경] 키값 생성 로직 수정
        string finalKey = "";

        foreach (GameObject obj in targets)
        {
            if (obj == null) continue;
            Trash trash = obj.GetComponent<Trash>();
            if (trash == null) continue;

            // 키값 생성 (스위치문으로 분기)
            switch (trash.Size)
            {
                case Trash.TrashSize.Small:
                    finalKey = "small_" + trash.TrashNum;
                    break;

                case Trash.TrashSize.Large:
                    finalKey = "large_" + trash.TrashNum;
                    break;

                case Trash.TrashSize.Human:
                    // [여기가 수정됨!] 이름으로 2종류 구분하기

                    // 조건: 오브젝트 이름에 특정 단어(예: "Boss", "Type2", "Gen")가 포함되어 있다면?
                    // (사용자분의 두 번째 NPC 프리팹 이름에 들어가는 단어를 아래 따옴표 안에 넣으세요)
                    if (obj.name.Contains("Mr.Go"))
                    {
                        // 두 번째 NPC -> CSV의 'large_7'(MR.GO) 데이터 사용
                        finalKey = "large_7";
                    }
                    else
                    {
                        // 그 외 일반 NPC -> CSV의 'large_6'(사람) 데이터 사용
                        finalKey = "large_6";
                    }
                    break;
            }

            // 첫 번째 녀석 이름 가져오기
            if (obj == targets[0])
            {
                firstTrashName = CSV_Database.instance.getname(finalKey);
            }

            // 데이터 누적
            totalWeight += CSV_Database.instance.getweight(finalKey);
            totalHpCost += CSV_Database.instance.getHpdecrease(finalKey);

            // 힘은 최대값 찾기
            int req = CSV_Database.instance.getrequireStr(finalKey);
            if (req > maxReqStr) maxReqStr = req;
        }

        if (totalWeight <= 0)
        {
            HideInfo(); // 혹시 켜져있다면 끄기
            return;     // UI 갱신 로직 실행 안 하고 종료
        }

        // --- UI 표시 로직 (기존과 동일) ---
        int currentStr = GameManager.instance.P_Str;
        bool isEng = GameManager.instance.P_isEnglish;

        if (nameText != null)
        {
            int extraCount = targets.Length - 1;
            if (extraCount > 0)
            {
                nameText.text = isEng ? $"{firstTrashName} (+{extraCount})" : $"{firstTrashName} 외 {extraCount}개";
            }
            else
            {
                nameText.text = firstTrashName;
            }
        }

        if (infoText != null)
        {
            infoBuilder.Clear();

            // 총 무게
            infoBuilder.AppendLine(isEng ? $"Total Weight: {totalWeight} g" : $"총 무게: {totalWeight} g");

            // 필요 힘 (Max값)
            if (currentStr >= maxReqStr)
            {
                infoBuilder.AppendLine(isEng ? $"Req Str: {maxReqStr}" : $"필요 힘: {maxReqStr}");
            }
            else
            {
                string warning = isEng ? "Can't hold!" : "힘 부족!";
                infoBuilder.Append($"<color=red>{warning} ({currentStr}/{maxReqStr})</color>\n");
            }

            // 총 청결도 소모
            if (totalHpCost > 0)
            {
                string costText = isEng ? "Total CP Loss" : "총 청결도 소모";
                infoBuilder.Append($"{costText}: <color=orange>-{totalHpCost}</color>");
            }

            infoText.text = infoBuilder.ToString();
        }

        if (uiPanel != null) uiPanel.SetActive(true);
    }

    public void HideInfo()
    {
        if (uiPanel != null && uiPanel.activeSelf)
        {
            uiPanel.SetActive(false);
            currentTargets = null;
        }
    }
}
