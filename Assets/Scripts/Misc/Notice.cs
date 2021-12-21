using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Notice : MonoBehaviour
{
    #region variables

    private TextMeshProUGUI txt_description;
    private Image img_Symbol;

    #endregion 

    public void Init()
    {
        img_Symbol = transform.Find("Symbol").GetComponent<Image>();

        txt_description = transform.Find("Tooltip").Find("Description").GetComponent<TextMeshProUGUI>();
    }

    #region  Setters

    // Strings
    public void SetDescription(string Description)
    { txt_description.text = Description; }

    //Images
    public void SetSymbol(Sprite Symbol)
    { img_Symbol.sprite = Symbol; }


    #endregion
}
