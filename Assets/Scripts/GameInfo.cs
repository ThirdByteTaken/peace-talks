using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameInfo : MonoBehaviour
{
    #region Variables    

    private static TextMeshProUGUI txt_TurnCount;

    private static GameObject go_NextTurn;
    private static int turnCount;
    public static int s_TurnCount
    {
        get
        { return turnCount; }
        set
        {
            txt_TurnCount.text = "Turn " + value;
            turnCount = value;
        }
    }

    #endregion

    public void OnEnable()
    {
        go_NextTurn = transform.Find("Next Turn").gameObject;
        txt_TurnCount = transform.Find("Turn Count").GetComponent<TextMeshProUGUI>();
    }

    #region  Setters

    // Integers 
    public static void ShowNextTurnButton(bool active)
    { go_NextTurn.SetActive(active); }

    #endregion
}
