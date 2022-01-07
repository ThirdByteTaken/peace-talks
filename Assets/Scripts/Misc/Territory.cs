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
    private Country owner;
    public Country Owner
    {
        get
        {
            return owner;
        }
        set
        {
            if (owner == null) Button.onClick.AddListener(OpenOwnerView);
            owner = value;
        }
    }
    public Territory(int _x, int _y, GameObject _gameObject, TerrainType _terrain)
    {
        X = _x;
        Y = _y;
        GameObject = _gameObject;
        Terrain = _terrain;
        Image = GameObject.GetComponent<Image>();
        Image.color = Terrain.Color;
        Button = GameObject.GetComponent<Button>();

    }

    public void OpenOwnerView()
    {
        Debug.Log("opened owner view of " + Owner.name);
        MapManager.MoveStatIndicator(this);
    }
}