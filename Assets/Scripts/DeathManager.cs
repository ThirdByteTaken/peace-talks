using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathManager : MonoBehaviour
{
    // Game over stuff
    [SerializeField]
    private TextMeshProUGUI txt_GameOverTitle, txt_GameOverReason;

    [SerializeField]
    private GameObject go_GameOver;

    private static TextMeshProUGUI s_txt_GameOverTitle, s_txt_GameOverReason;
    private static GameObject s_go_GameOver;

    void Start()
    {
        s_txt_GameOverTitle = txt_GameOverTitle;
        s_txt_GameOverReason = txt_GameOverReason;
        s_go_GameOver = go_GameOver;
    }

    public static void GameOver(string Title, string Reason)
    {
        if (Main.s_noDeath) return; // TODOremove later
        s_go_GameOver.SetActive(true);
        s_txt_GameOverTitle.text = Title;
        s_txt_GameOverReason.text = Reason;
        return;
    }

}
