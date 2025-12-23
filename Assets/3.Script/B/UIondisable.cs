using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIondisable : MonoBehaviour
{
    [SerializeField] private GameObject UIvalue;
    [SerializeField] private GameObject minimap;

    // ÀÌ ¿ÀºêÁ§Æ®°¡ ÄÑÁú ¶§ ½ÇÇàµÊ
    private void OnEnable()
    {
        if (UIvalue != null&&minimap!=null)
        {
            UIvalue.SetActive(false); // UI¸¦ ²û
            minimap.SetActive(false); // UI¸¦ ²û
        }
    }

    // ÀÌ ¿ÀºêÁ§Æ®°¡ ²¨Áú ¶§ ½ÇÇàµÊ
    private void OnDisable()
    {
        if (UIvalue != null)
        {
            UIvalue.SetActive(true); // UI¸¦ Å´
            minimap.SetActive(true); // UI¸¦ Å´
        }
    }
}
