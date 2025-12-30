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

    // [변경점] 매개변수를 GameObject 하나가 아니라 배열([])로 받습니다.
    public void UpdateTooltip(GameObject[] targets)
    {
        // 1. 타겟이 없거나 비어있으면 숨기기
        if (targets == null || targets.Length == 0)
        {
            HideInfo();
            return;
        }

        // 2. 첫 번째 타겟이 유효한지 확인
        if (targets[0] == null)
        {
            HideInfo();
            return;
        }

        // 3. 정보 표시 실행
        ShowInfo(targets);
    }

    private void ShowInfo(GameObject[] targets)
    {
        // 최적화: 이미 켜져 있고 같은 배열 구성을 보고 있다면 UI 갱신 건너뜀 (참조 비교)
        if (uiPanel != null && uiPanel.activeSelf && currentTargets == targets) return;

        currentTargets = targets;

        // --- 데이터 계산 로직 시작 ---
        if (CSV_Database.instance == null || GameManager.instance == null) return;

        int totalWeight = 0;
        int maxReqStr = 0;
        int totalHpCost = 0;

        // 대표 이름용 (첫 번째 물체)
        Trash firstTrash = targets[0].GetComponent<Trash>();

        string firstTrashName = "";

        // 배열을 순회하며 합산
        foreach (GameObject obj in targets)
        {
            if (obj == null) continue;
            Trash trash = obj.GetComponent<Trash>();
            if (trash == null) continue;

            // 키값 생성

            string finalKey = "";

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

            // 힘은 최대값 찾기 (가장 무거운 걸 들 수 있어야 하므로)
            int req = CSV_Database.instance.getrequireStr(finalKey);
            if (req > maxReqStr) maxReqStr = req;
        }

        // --- UI 표시 로직 ---
        int currentStr = GameManager.instance.P_Str;
        bool isEng = GameManager.instance.P_isEnglish;

        // 1. 이름 설정 (예: 유리병 외 2개)
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

        // 2. 상세 정보 설정
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
                // 붉은색 경고
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
