using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CountrySlot : MonoBehaviour
{
    #region Variables

    private Image img_Flag;

    private TextMeshProUGUI txt_CountryName, txt_LeaderName, txt_Personality, txt_Money, txt_WarPower, txt_Relation;

    private Button btn_select; // the entry in the country list that is clicked to select this country

    public Country Country
    {
        get
        {
            return (Main.Instance.cs_Players.FirstOrDefault(x => x.Value == this).Key); // Returns the key of the key value pair that has this as the value
        }
    }
    #endregion

    public void Init()
    {
        img_Flag = transform.Find("Flag").GetComponent<Image>();

        txt_CountryName = transform.Find("Country Name").GetComponent<TextMeshProUGUI>();
        txt_LeaderName = transform.Find("Leader Name").GetComponent<TextMeshProUGUI>();
        txt_Personality = transform.Find("Personality").Find("Text").GetComponent<TextMeshProUGUI>();
        txt_Money = transform.Find("Money").Find("Text").GetComponent<TextMeshProUGUI>();
        txt_WarPower = transform.Find("War Power").Find("Text").GetComponent<TextMeshProUGUI>();
        txt_Relation = transform.Find("Relation").GetComponent<TextMeshProUGUI>();

        btn_select = GetComponent<Button>();
    }

    #region  Setters

    public void UpdateSlot()
    {
        SetCountryName(Country.CountryName);
        SetLeaderName(Country.Leader.Name);
        SetPersonality(Country.Leader.Personality);
        SetMoney(Country.Money);
        SetWarPower(Country.WarPower);
        if (!Country.IsPlayer) SetRelation(Country.Relations[Main.Instance.cnt_Player].Value);
    }

    // Sprites
    public void SetFlag(Sprite Flag)
    { img_Flag.sprite = Flag; }

    // Strings
    public void SetCountryName(string Name)
    { txt_CountryName.text = Name; }
    public void SetLeaderName(string Name)
    { txt_LeaderName.text = Name; }

    // Integers 
    public void SetMoney(int Money)
    { txt_Money.text = Money.ToString(); }
    public void SetWarPower(int WarPower)
    { txt_WarPower.text = WarPower.ToString(); }
    public void SetRelation(int Relation)
    { txt_Relation.text = Relation + "/" + 100; }

    // Color Block
    public void SetColorBlock(ColorBlock colorBlock)
    {
        btn_select.colors = colorBlock;
    }

    // Other
    public void SetPersonality(PersonalityType Personality)
    { txt_Personality.text = Personality.Name; }


    public void SetButtonSelected(bool selected)
    {
        ColorBlock newColorBlock = btn_select.colors;
        Color newColor = (selected) ? btn_select.colors.disabledColor : new Color((1 / 3f), (1 / 3f), (1 / 3f));
        newColorBlock.normalColor = newColor;
        newColorBlock.highlightedColor = (selected) ? UIManager.Instance.SelectedCountrySlotHighlightColor : UIManager.Instance.DeselectedCountrySlotHighlightColor;
        btn_select.colors = newColorBlock;
    }

    public void SetButtonInteractable(bool interactable)
    {
        if (Country.IsPlayer || interactable == btn_select.IsInteractable()) return;
        ColorBlock newColorBlock = btn_select.colors;
        newColorBlock.normalColor = btn_select.colors.disabledColor;
        newColorBlock.disabledColor = btn_select.colors.normalColor;
        btn_select.colors = newColorBlock;
        btn_select.interactable = !btn_select.IsInteractable();
    }
    #endregion
}
