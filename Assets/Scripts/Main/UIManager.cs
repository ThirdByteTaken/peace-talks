using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    Main main;

    [HideInInspector]
    public Action hoveredAction;

    [SerializeField]
    private GameObject go_CategoryButtons;

    [SerializeField]
    private GameObject[] go_Categories;

    public GameObject go_Actions;


    public List<ColorBlock> cb_CountrySlotColors;

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
        CountrySlot oldSelected = (SelectedCountry == null) ? null : main.cs_Players[SelectedCountry];
        bool diffAsLast = (oldSelected != CountrySlot); // if button clicked was NOT the same as previously selected country       

        if (oldSelected != null) oldSelected.SetButtonSelected(false);
        CountrySlot.SetButtonSelected(diffAsLast);
        SelectedCountry = (diffAsLast) ? CountrySlot.Country : null; // sets receiver to corresponsing country of clicked country slot
        ActionManager.Instance.CurrentEvent.receiver = SelectedCountry;
        SetActionButtonsEnabled(diffAsLast);
    }
    public void ActionHover(Action action)
    {
        hoveredAction = action;
    }

    public void ReturnToCategories()
    {
        go_CategoryButtons.SetActive(true);
        foreach (GameObject obj in go_Categories) obj.SetActive(false);
    }

    public void DeselectCurrentCountrySlot()
    {
        if (SelectedCountry != null) SelectCountrySlot(main.cs_Players[SelectedCountry]);
    }

    public void SetCountrySlotButtonsInteractable()
    {
        foreach (CountrySlot cs in main.cs_Players.Values) cs.SetButtonInteractable(true);
    }
    public void SetCountrySlotButtonsUninteractable()
    {
        foreach (CountrySlot cs in main.cs_Players.Values) cs.SetButtonInteractable(false);
    }

    public void SetActionButtonsEnabled(bool enabled)
    {
        go_Actions.GetComponentsInChildren<Button>().ToList().ForEach(x => x.interactable = enabled);
    }
}
