using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldObjectPreviewUI : MonoBehaviour
{
    public static WorldObjectPreviewUI i;

    [SerializeField] GameObject graphicsParent;
    [SerializeField] Image portraitRenderer;
    [SerializeField] TMP_Text headerText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] Image hpBarFill; //the red fill of the info bar HP fill
    [SerializeField] TMP_Text healthText;

    void Awake()
    {
        if(i==null)
            i = this;
        else
            Destroy(this.gameObject);
    }

    public void displayObject(MapObject mapObject)
    {
        Sprite portrait;
        string description;
        float healthPercentage;
        int maxHealth;
        headerText.text = mapObject.exposeObjectInfo(out portrait, out description, out healthPercentage, out maxHealth); //assigns all datatypes without constructing a new class or multiple getters

        portraitRenderer.sprite = portrait;
        descriptionText.text = description;

        hpBarFill.rectTransform.localScale = new Vector3(healthPercentage, 1, 1); //set the fill to match the current HP value
        healthText.text = $"{(int)(healthPercentage * maxHealth)}/{maxHealth} HP"; //set the text value of the health text

        graphicsParent.gameObject.SetActive(true);
    }

    public void hideMenu()
    {
        graphicsParent.gameObject.SetActive(false);
    }
}