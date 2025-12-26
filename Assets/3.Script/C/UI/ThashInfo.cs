using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ThashInfo : MonoBehaviour
{
    [Header("UI 설정")]
    // [변경] 판넬 변수는 삭제하고, 텍스트만 남깁니다.
    [SerializeField] private Text tooltipText;

    [Header("감지 설정")]
    [SerializeField] private float checkDistance = 5.0f;
    [SerializeField] private LayerMask trashLayerMask;

    private Camera mainCamera;
    private Trash lastHitTrash = null;

    private void Start()
    {
        mainCamera = Camera.main;
        HideInfo(); // 게임 시작 시 텍스트 숨기기
    }

    private void Update()
    {
        CheckObjectInFront();
    }

    private void CheckObjectInFront()
    {
        if (mainCamera == null) return;

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, checkDistance, trashLayerMask))
        {
            Trash trash = hit.collider.GetComponentInParent<Trash>();

            if (trash != null)
            {
                ShowInfo(trash);
                return;
            }
        }

        HideInfo();
    }

    private void ShowInfo(Trash trash)
    {
        // 텍스트 오브젝트가 이미 켜져 있고, 같은 쓰레기면 패스
        if (tooltipText != null && tooltipText.gameObject.activeSelf && lastHitTrash == trash) return;

        lastHitTrash = trash;

        // 키값 생성
        string keyPrefix = "";
        if (trash.Size == Trash.TrashSize.Small) keyPrefix = "small_";
        else if (trash.Size == Trash.TrashSize.Large) keyPrefix = "large_";
        else if (trash.Size == Trash.TrashSize.Human) keyPrefix = "human_";

        string finalKey = keyPrefix + trash.TrashNum;

        // CSV 데이터 가져오기
        if (CSV_Database.instance != null && CSV_Database.instance.GarbageMap != null)
        {
            if (CSV_Database.instance.GarbageMap.TryGetValue(finalKey, out Dictionary<string, object> data))
            {
                string tName = data.ContainsKey("name") ? data["name"].ToString() : "이름 없음";
                int tWeight = data.ContainsKey("weight") ? int.Parse(data["weight"].ToString()) : 0;

                if (tooltipText != null)
                {
                    tooltipText.text = $"{tName}\n<color=yellow>{tWeight} g</color>";

                    // [핵심 변경] 판넬 대신 텍스트 오브젝트 자체를 켭니다!
                    tooltipText.gameObject.SetActive(true);
                }
            }
        }
    }

    private void HideInfo()
    {
        // 텍스트 끄기
        if (tooltipText != null && tooltipText.gameObject.activeSelf)
        {
            tooltipText.gameObject.SetActive(false);
            lastHitTrash = null;
        }
    }
}
