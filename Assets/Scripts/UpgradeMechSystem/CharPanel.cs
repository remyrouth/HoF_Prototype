 using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class CharPanel : MonoBehaviour
{
    public GameObject profileToggleObj;

    public void SetupPopup(Image img, string pilotName, string description)
    {
        Image pilot = transform.Find("Char").GetComponent<Image>();
        if (pilot != null)
        {
            pilot.sprite = img.sprite;
        }
        else
        {
            Debug.LogError("Could not find 'Char' Image component.");
        }

        TextMeshProUGUI name = transform.Find("name").GetComponent<TextMeshProUGUI>();
        if (name != null)
        {
            name.text = pilotName; 
        }
        else
        {
            Debug.LogError("Could not find 'name' TextMeshProUGUI component.");
        }

        TextMeshProUGUI desc = transform.Find("DescBox/Text (TMP)").GetComponent<TextMeshProUGUI>();
        if (desc != null)
        {
            desc.text = description;
        }
        else
        {
            Debug.LogError("Could not find 'DescBox/Text (TMP)' TextMeshProUGUI component.");
        }

        this.profileToggleObj = GameObject.Find("Toggler");
        ProfileToggle profileToggle = profileToggleObj.GetComponent<ProfileToggle>();
        Button returnButton = transform.Find("return").GetComponent<Button>();
        returnButton.onClick.AddListener(() => profileToggle.ReturnFromPopup());

        Button leftButton = transform.Find("prev char").GetComponent<Button>();
        leftButton.onClick.AddListener(() => profileToggle.IteratePopup(true));

        Button rightButton = transform.Find("next char").GetComponent<Button>();
        rightButton.onClick.AddListener(() => profileToggle.IteratePopup(false));
    }
}