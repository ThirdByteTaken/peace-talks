using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Territory
{
    public int X;
    public int Y;
    public GameObject GameObject;
    public TerrainType Terrain;
    public Image Image;
    public Button Button;
    public Country Owner;
    public Territory(int _x, int _y, GameObject _gameObject, TerrainType _terrain)
    {
        X = _x;
        Y = _y;
        GameObject = _gameObject;
        Terrain = _terrain;
        Image = GameObject.GetComponent<Image>();
        Image.color = Terrain.Color;
        Button = GameObject.GetComponent<Button>();
        Button.onClick.AddListener(OpenStatBox);
    }

    public void OpenStatBox()
    {
        MapManager.MoveStatBox(this);
    }

    public void OpenLeaderView()
    {
        Debug.Log("open " + Owner.CountryName);
    }
}