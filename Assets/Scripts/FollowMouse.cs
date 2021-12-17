using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowMouse : MonoBehaviour
{
    public GameObject obj;

    private RectTransform rect;
    private Image img;

    private bool lockToMouse = false;

    void Start()
    {
        rect = obj.GetComponent<RectTransform>();
        img = obj.GetComponent<Image>();
        img.enabled = false;
    }

    public void ShowToolTip()
    {
        lockToMouse = true;
        obj.SetActive(true);
        if (img != null) img.enabled = false; // This is to delay showing the image until after the position is updated
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
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rect.transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
            rect.localPosition += new Vector3(rect.sizeDelta.x / 2, rect.sizeDelta.y / 2, 0f);
            if (!img.enabled)
            {
                img.enabled = true;
            }
        }
    }
}
