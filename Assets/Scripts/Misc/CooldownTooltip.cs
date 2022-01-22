using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CooldownTooltip : MonoBehaviour
{
    [SerializeField]
    private ActionManager actionManager;

    [SerializeField]
    private Main main;

    public GameObject obj;

    private RectTransform rect;
    private Image img;
    private TMP_Text txt;

    private bool lockToMouse = false;

    void Start()
    {
        rect = obj.GetComponent<RectTransform>();
        img = obj.GetComponent<Image>();
        txt = obj.GetComponentInChildren<TMP_Text>();
        img.enabled = false;
    }

    public void ShowToolTip()
    {
        var CurrentCooldowns = main.cnt_Player.ActionCooldowns;
        var CurrentAction = UIManager.s_hoveredAction;

        if (CurrentCooldowns == null) return;
        if (!CurrentCooldowns.ContainsKey(CurrentAction)) return;
        if (CurrentCooldowns[CurrentAction] <= 0) return;
        lockToMouse = true;
        obj.SetActive(true);
        if (img != null) img.enabled = false; // This is to delay showing the image until after the position is updated

        txt.text = CurrentCooldowns[CurrentAction] + " Turns";
    }

    public void HideToolTip()
    {
        lockToMouse = false;
        obj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (lockToMouse)
        {
            print("Hello");
            Vector3 mousePos = main.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            obj.transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
            rect.localPosition += new Vector3(rect.sizeDelta.x / 2, rect.sizeDelta.y / 2, 0f);
            if (!img.enabled)
            {
                img.enabled = true;
            }
        }
    }
}
