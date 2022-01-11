using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class CountryView : MonoBehaviour
{
    #region Variables

    private Image img_Flag, img_LeaderPersonality;

    private TextMeshProUGUI txt_CountryName, txt_LeaderName, txt_LeaderPersonality, txt_WarPower, txt_Money;

    private Transform trfm_Relation;
    #endregion

    public void Init()
    {
        img_Flag = transform.Find("Flag").GetComponent<Image>();
        img_LeaderPersonality = transform.Find("Leader").Find("Personality").Find("Image").GetComponent<Image>();

        txt_CountryName = transform.Find("Country").GetComponent<TextMeshProUGUI>();
        trfm_Relation = transform.Find("Relations").Find("Relation");
        txt_LeaderName = transform.Find("Leader").Find("Name").GetComponent<TextMeshProUGUI>();
        txt_LeaderPersonality = transform.Find("Leader").Find("Personality").Find("Text").GetComponent<TextMeshProUGUI>();
        txt_WarPower = transform.Find("Resources").Find("War Power").Find("Text").GetComponent<TextMeshProUGUI>();
        txt_Money = transform.Find("Resources").Find("Money").Find("Text").GetComponent<TextMeshProUGUI>();

    }

    public void OpenCountry(Country country)
    {


        img_Flag.sprite = country.Flag;
        img_LeaderPersonality.sprite = country.Leader.Personality.Sprite;

        txt_CountryName.text = country.CountryName;

        Transform relationsParent = transform.Find("Relations");
        for (int i = relationsParent.childCount - 1; i > 1; i--) // Destroys all extra relations (not original object or title)
            GameObject.Destroy(relationsParent.GetChild(i));
        var txt_relationValues = new List<TextMeshProUGUI>();
        var txt_relationCountries = new List<TextMeshProUGUI>();
        var relationCount = country.Relations.Count;
        var anchoredSpacePerRelation = relationsParent.GetChild(0).GetComponent<RectTransform>().anchorMin.y / relationCount;
        var minimumAnchorPosition = 0f;
        var maximumAnchorPosition = anchoredSpacePerRelation;
        while (txt_relationValues.Count < relationCount)
        {
            var newRelation = (txt_relationValues.Count == 0) ? trfm_Relation.gameObject : GameObject.Instantiate(trfm_Relation.gameObject, relationsParent);
            var rect_newRelation = newRelation.GetComponent<RectTransform>();
            rect_newRelation.anchorMin = new Vector2(0, minimumAnchorPosition);
            rect_newRelation.anchorMax = new Vector2(1, maximumAnchorPosition);
            rect_newRelation.offsetMin = new Vector2(0, 0);
            rect_newRelation.offsetMax = new Vector2(0, 0);
            txt_relationValues.Add(newRelation.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
            txt_relationCountries.Add(newRelation.transform.GetChild(1).GetComponent<TextMeshProUGUI>());
            minimumAnchorPosition = maximumAnchorPosition;
            maximumAnchorPosition += anchoredSpacePerRelation;

        }
        var relationCountries = new List<Country>(Main.s_cnt_Players);
        relationCountries.Remove(country);
        for (int i = 0; i < relationCountries.Count; i++)
        {
            txt_relationCountries[i].text = $"{relationCountries[i].CountryName}:";
            txt_relationValues[i].text = $"{country.Relations[i].Value}/100";
        }
        txt_LeaderName.text = country.Leader.Name;
        txt_LeaderPersonality.text = country.Leader.Personality.Name;
        txt_WarPower.text = (country.WarPower + $" ({((country.WarPowerGain > 0) ? "+" : "")}{country.WarPowerGain})");
        txt_Money.text = (country.MoneyGain + $" ({((country.MoneyGain > 0) ? "+" : "")}{country.MoneyGain})");
    }

}
