using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class EventSlot : MonoBehaviour
{
    #region variables

    private TextMeshProUGUI txt_CountryName, txt_Action, txt_description;
    private TextMeshProUGUI[] txt_Responses = new TextMeshProUGUI[Main.Max_Action_Reactions]; // size should be greatest number of responses to a single event
    private Image img_Flag;

    #endregion 

    public void Init()
    {
        img_Flag = transform.Find("Country Info").Find("Flag").GetComponent<Image>();

        txt_CountryName = transform.Find("Country Info").Find("Country Name").GetComponent<TextMeshProUGUI>();
        txt_Action = transform.Find("Action Info").Find("Action").GetComponent<TextMeshProUGUI>();
        txt_description = transform.Find("Action Info").Find("Description").GetComponent<TextMeshProUGUI>();
        txt_Responses[0] = transform.Find("Action Info").Find("Option1").Find("Text").GetComponent<TextMeshProUGUI>();
        txt_Responses[1] = transform.Find("Action Info").Find("Option2").Find("Text").GetComponent<TextMeshProUGUI>();
        txt_Responses[2] = transform.Find("Action Info").Find("Option3").Find("Text").GetComponent<TextMeshProUGUI>();
    }

    #region  Setters

    public void SetResponseActive(int index, bool Active)
    { txt_Responses[index].transform.parent.gameObject.SetActive(Active); }

    // Strings 
    public void SetCountryName(string CountryName)
    { txt_CountryName.text = CountryName; }
    public void SetAction(string Action)
    { txt_Action.text = Action; }
    public void SetDescription(string Description)
    { txt_description.text = Description; }
    public void SetResponse(int index, string Response)
    { txt_Responses[index].text = Response; }

    //Images
    public void SetFlag(Sprite Flag)
    { img_Flag.sprite = Flag; }


    #endregion
}
