using UnityEngine;

public class ChangeLanguage : MonoBehaviour
{
    public void changeLanguage()
    {
        GameManager.instance.P_isEnglish = !GameManager.instance.P_isEnglish;
        CSV_Database.instance.LoadData();
    }                             
}
