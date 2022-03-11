using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class CountryView : MonoBehaviour
{
    #region Variables

    private Image img_Flag;

    private TextMeshProUGUI txt_CountryName, txt_WarPower, txt_Money;

    private Transform trfm_Relation, trfm_Focus;

    private Button btn_Close, btn_BackgroundClose;
    #endregion

    public void Init()
    {
        img_Flag = transform.Find("Flag").GetComponent<Image>();

        txt_CountryName = transform.Find("Country").GetComponent<TextMeshProUGUI>();
        txt_WarPower = transform.Find("Resources").Find("War Power").Find("Text").GetComponent<TextMeshProUGUI>();
        txt_Money = transform.Find("Resources").Find("Money").Find("Text").GetComponent<TextMeshProUGUI>();


        trfm_Relation = transform.Find("Relations").Find("Relation");
        trfm_Focus = transform.Find("Focuses").Find("Focus");

        btn_Close = transform.Find("Close").GetComponent<Button>();
        btn_Close.onClick.AddListener(Close);

        btn_BackgroundClose = transform.Find("Background Close").GetComponent<Button>();
        btn_BackgroundClose.onClick.AddListener(Close);
    }

    public void OpenCountry(Country country)
    {
        img_Flag.sprite = country.Flag;

        txt_CountryName.text = country.CountryName;
        #region Relations
        Transform relationsParent = transform.Find("Relations");
        for (int i = relationsParent.childCount - 1; i > 1; i--) // Destroys all extra relations (not original object or title)
            GameObject.Destroy(relationsParent.GetChild(i).gameObject);
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
        var relationCountries = new List<Country>(Main.Instance.cnt_Players);
        relationCountries.Remove(country);
        for (int i = 0; i < relationCountries.Count; i++)
        {
            txt_relationCountries[i].text = $"{relationCountries[i].CountryName}:";
            txt_relationValues[i].text = $"{country.Relations[relationCountries[i]].Value}/100";
        }

        #endregion

        #region Focuses

        trfm_Focus.gameObject.SetActive(true);

        Transform focusesParent = transform.Find("Focuses");
        for (int i = focusesParent.childCount - 1; i > 1; i--) // Destroys all extra focuses (not original object or title)
            GameObject.Destroy(focusesParent.GetChild(i).gameObject);
        var originalFocusRect = trfm_Focus.GetComponent<RectTransform>();
        float totalAnchoredFocusSize = originalFocusRect.anchorMax.y - originalFocusRect.anchorMin.y;
        var focusAnchoredPosition = originalFocusRect.anchorMin.y; // Stores highest filled position (anchored)
        var focusCopy = new List<Focus>(ActionManager.Instance.Focuses);


        for (int i = 0; i < focusCopy.Count; i++)
        {
            var newFocus = GameObject.Instantiate(trfm_Focus.gameObject, focusesParent);
            var rect_newFocus = newFocus.GetComponent<RectTransform>();
            rect_newFocus.anchorMin = new Vector2(originalFocusRect.anchorMin.x, focusAnchoredPosition);
            rect_newFocus.anchorMax = new Vector2(originalFocusRect.anchorMax.x, focusAnchoredPosition);
            rect_newFocus.offsetMin = new Vector2(0, 0);
            rect_newFocus.offsetMax = new Vector2(0, 0);

            var anchorBoundsSize = ((2 / rect_newFocus.rect.width), (1 / rect_newFocus.rect.height));

            var rect_background = newFocus.transform.GetChild(0).GetComponent<RectTransform>();
            rect_background.anchorMin = new Vector2(anchorBoundsSize.Item1, anchorBoundsSize.Item2);
            rect_background.anchorMax = new Vector2(1 - anchorBoundsSize.Item1, 1 - anchorBoundsSize.Item2);
            rect_background.GetComponent<Image>().color = (country.Focus == focusCopy[i]) ? Color.yellow : focusesParent.GetComponent<Image>().color;


            var trfm_focusName = newFocus.transform.GetChild(1);
            trfm_focusName.GetComponent<TextMeshProUGUI>().text = focusCopy[i].Name;
            var rect_focusName = trfm_focusName.GetComponent<RectTransform>();
            rect_focusName.anchorMin = new Vector2(anchorBoundsSize.Item1, anchorBoundsSize.Item2);
            rect_focusName.anchorMax = new Vector2(rect_focusName.anchorMax.x, 1 - anchorBoundsSize.Item2);

            var trfm_focusPercent = newFocus.transform.GetChild(2);
            var rect_focusPercent = trfm_focusPercent.GetComponent<RectTransform>();
            rect_focusPercent.anchorMin = new Vector2(rect_focusPercent.anchorMin.x, anchorBoundsSize.Item2);
            rect_focusPercent.anchorMax = new Vector2(1 - anchorBoundsSize.Item1, 1 - anchorBoundsSize.Item2);
            newFocus.transform.GetChild(3).GetComponent<Image>().sprite = focusCopy[i].Sprite; // Focus Sprite            

        }
        trfm_Focus.gameObject.SetActive(false);
        #endregion                
        txt_WarPower.text = (country.WarPower + $" ({((country.WarPowerGain > 0) ? "+" : "")}{country.WarPowerGain})");
        txt_Money.text = (country.MoneyGain + $" ({((country.MoneyGain > 0) ? "+" : "")}{country.MoneyGain})");
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}