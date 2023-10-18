using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class InternationalText : MonoBehaviour
{

    [SerializeField]
    private string _en;
    [SerializeField]
    private string _ru;

    private void Start()
    {
        if (Language.Instance.CurrentLanguage == "ru")
        {
            GetComponent<TextMeshProUGUI>().text = _ru;
        }
        else
        {
            GetComponent<TextMeshProUGUI>().text = _en;
        }
    }
}
