using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    Main main;

    public static Action s_hoveredAction;

    [SerializeField]
    private GameObject go_CategoryButtons;

    [SerializeField]
    private GameObject[] go_Categories;

    [SerializeField]
    public Color SelectedCountrySlotHighlightColor;

    [SerializeField]
    public Color DeselectedCountrySlotHighlightColor;

    private Country SelectedCountry;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {

        main = Main.Instance;
        Main.s_TurnActions += DeselectCurrentCountrySlot;
        Main.s_TurnActions += SetCountrySlotButtonsInteractable;
    }

    public void SelectCountrySlot(CountrySlot CountrySlot)
    {
        CountrySlot oldSelected = (SelectedCountry == null) ? null : main.cs_NonPlayers[SelectedCountry.ID];
        bool diffAsLast = (oldSelected != CountrySlot); // if button clicked was NOT the same as previously selected country       

        if (oldSelected != null) oldSelected.SetButtonSelected(false);
        CountrySlot.SetButtonSelected(diffAsLast);
        SelectedCountry = (diffAsLast) ? main.cnt_Players[System.Array.IndexOf(main.cs_NonPlayers, CountrySlot)] : null; // sets receiver to corresponsing country of clicked country slot
        ActionManager.Instance.CurrentEvent.receiver = SelectedCountry;
        main.SetActionButtonsEnabled(diffAsLast);
    }
    public void ActionHover(Action action)
    {
        s_hoveredAction = action;
    }

    public void ReturnToCategories()
    {
        go_CategoryButtons.SetActive(true);
        foreach (GameObject obj in go_Categories) obj.SetActive(false);
    }

    public void DeselectCurrentCountrySlot()
    {
        if (SelectedCountry != null) SelectCountrySlot(main.cs_NonPlayers[SelectedCountry.ID]);
    }

    public void SetCountrySlotButtonsInteractable()
    {
        foreach (CountrySlot cs in main.cs_NonPlayers) cs.SetButtonInteractable(true);
    }
    public void SetCountrySlotButtonsUninteractable()
    {
        foreach (CountrySlot cs in main.cs_NonPlayers) cs.SetButtonInteractable(false);
    }

}
