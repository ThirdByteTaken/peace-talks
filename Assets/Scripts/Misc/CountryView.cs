using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountryView : MonoBehaviour
{
    #region Variables

    private Image img_Flag;

    private TextMeshProUGUI txt_CountryName, txt_LeaderName, txt_Personality, txt_Money, txt_WarPower, txt_Relation;

    private Button btn_select; // the entry in the country list that is clicked to select this country

    public bool currentlyHovered; // if the mouse is on the country slot currently
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

}
