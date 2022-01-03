using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hex
{
    public int X;
    public int Y;
    public GameObject GameObject;
    public TerrainType Terrain;
    public Image Image;
    public Country Owner;
    public Hex(int _x, int _y, GameObject _gameObject, TerrainType _terrain)
    {
        X = _x;
        Y = _y;
        GameObject = _gameObject;
        Terrain = _terrain;
        Image = GameObject.GetComponent<Image>();
        Image.color = Terrain.Color;
    }
}