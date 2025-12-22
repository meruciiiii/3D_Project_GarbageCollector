using UnityEngine;

public class ChangeLanguage : MonoBehaviour
{

    [SerializeField]private GameObject KROB;
    [SerializeField]private GameObject ENOB;

    private void Awake()
    {
        switchlanguage();
    }
    public void changeLanguage()
    {
        GameManager.instance.P_isEnglish = !GameManager.instance.P_isEnglish;
        CSV_Database.instance.LoadData();
        switchlanguage();
    }
    
    private void switchlanguage()
    {
        if (!GameManager.instance.P_isEnglish)
        {
            KROB.SetActive(true);
            ENOB.SetActive(false);
        }
        else
        {
            KROB.SetActive(false);
            ENOB.SetActive(true);
        }
    }
}
