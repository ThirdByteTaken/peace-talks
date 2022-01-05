using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    // Start is called before the first frame update
    /*private static Vector3[] hexoffsets = new Vector3[]
    {
        new Vector3(-.5f, .75f), // Up and Left
        new Vector3(.5f, .75f), // Up and Right
        new Vector3(1f, 0f), // Right
        new Vector3(.5f, -.75f), // Down and Right
        new Vector3(-.5f, -.75f), // Down and Left
        new Vector3(-1f, 0f)  // Left
    };*/


    public GameObject originalImage;
    public List<TerrainType> TerrainTypes;
    public static Dictionary<string, TerrainType> terrainTypes = new Dictionary<string, TerrainType>();

    public int TerritoriesPerCountry;


    private Vector3 imageSize;
    private Vector3 mapFrameSize;
    private RectTransform rect_mapFrame;



    private List<List<Territory>> map = new List<List<Territory>>();

    private Main main;
    void Start()
    {
        main = GetComponent<Main>();
        foreach (TerrainType terrain in TerrainTypes)
            terrainTypes.Add(terrain.Name, terrain);
        Image image = originalImage.GetComponent<Image>();
        imageSize = new Vector3(image.sprite.rect.width, image.sprite.rect.height);
        rect_mapFrame = originalImage.transform.parent.parent.GetComponent<RectTransform>();
    }
    public void GenerateHexGrid()
    {
        map.Clear();
        originalImage.SetActive(true);
        Transform mapObject = originalImage.transform.parent;
        for (int i = mapObject.childCount - 1; i > 0; i--)
        {
            GameObject.Destroy(mapObject.GetChild(i).gameObject);
        }
        int height = Random.Range(5, 20);
        int width = Random.Range(5, 20);
        Vector3 sizeRequired = new Vector3((width + .5f) * imageSize.x, ((height * .75f) + .25f) * imageSize.y);
        float scaleFactor = Mathf.Min(rect_mapFrame.sizeDelta.x / sizeRequired.x, rect_mapFrame.sizeDelta.y / sizeRequired.y); // Finds the scale that allows map to fit both ways
        mapObject.localScale = new Vector3(scaleFactor, scaleFactor);
        mapFrameSize = sizeRequired;
        Vector3 rowStart = new Vector3(-((mapFrameSize.x / 2) - (imageSize.x / 2)), -((mapFrameSize.y / 2) - (imageSize.y / 2)));
        for (int i = 0; i < height; i++)
        {

            map.Add(new List<Territory>());
            Vector3 rowPos = rowStart;
            Transform rowParent = new GameObject().transform;
            rowParent.parent = mapObject;
            rowParent.localScale = Vector3.one;
            for (int j = 0; j < width; j++)
            {
                GameObject newGameobject = GameObject.Instantiate(originalImage, rowParent);
                newGameobject.transform.localPosition = rowPos;
                map[i].Add(new Territory(j, i, newGameobject, (Mathf.Min(i, j) == 0 || height - i == 1 || width - j == 1) ? terrainTypes["Water"] : terrainTypes["Land"]));
                rowPos.x += imageSize.x;
            }
            rowStart = new Vector3(rowStart.x + (((i % 2 == 1) ? -1 : 1) * imageSize.x / 2), rowStart.y += .75f * imageSize.y);
        }
        originalImage.SetActive(false);
        SetCountryOwnerships();
    }
    private void SetCountryOwnerships()
    {
        float perimeter = (2 * map.Count) + (2 * map[0].Count) - 4;
        float tilesBetweenCountryStarts = perimeter / (main.cnt_NonPlayers.Count() + 1);
        List<Country> totalCountries = main.cnt_NonPlayers.ToList();
        totalCountries.Add(main.cnt_Player);
        totalCountries.ForEach(x => x.OwnedTerritories = new List<Territory>());
        List<Territory>[] ownedOutsideTerritories = new List<Territory>[totalCountries.Count];
        int totalCountryCount = totalCountries.Count;
        int spawnedCountries = 0;
        int totalPerimeterDistance = 0;
        (int, int) previousHexPosition = (int.MaxValue, 0);
        (int, int) currentHexPosition = (Random.Range(0, map[0].Count), 0); // TODO choose random position on perimeter
        (int, int) change = (1, 0);
        List<Country> unSpawnedCountries = new List<Country>(totalCountries);
        while (spawnedCountries < totalCountryCount)
        {
            if (totalPerimeterDistance >= tilesBetweenCountryStarts * (spawnedCountries + 1))
            {
                int newOwnerCountryIndex = Random.Range(0, unSpawnedCountries.Count - 1);
                Country newOwnerCountry = unSpawnedCountries[newOwnerCountryIndex];
                map[currentHexPosition.Item2][currentHexPosition.Item1].Owner = newOwnerCountry;
                map[currentHexPosition.Item2][currentHexPosition.Item1].Image.color = Color.white;
                ownedOutsideTerritories[totalCountries.IndexOf(newOwnerCountry)] = new List<Territory>();
                ownedOutsideTerritories[totalCountries.IndexOf(newOwnerCountry)].Add(map[currentHexPosition.Item2][currentHexPosition.Item1]);
                newOwnerCountry.OwnedTerritories.Add(map[currentHexPosition.Item2][currentHexPosition.Item1]);
                unSpawnedCountries.Remove(newOwnerCountry);
                spawnedCountries++;
                previousHexPosition = currentHexPosition;
            }
            if (change.Item1 != 0)
            {
                int newXPos = currentHexPosition.Item1 + change.Item1;
                if (newXPos < map[0].Count && newXPos > -1)
                {
                    currentHexPosition.Item1 = newXPos;
                }
                else
                {
                    change = (0, change.Item1);
                    currentHexPosition.Item2 += change.Item2;
                }
            }
            else
            {
                int newYPos = currentHexPosition.Item2 + change.Item2;
                if (newYPos < map.Count && newYPos > -1)
                {
                    currentHexPosition.Item2 = newYPos;
                }
                else
                {
                    change = (-change.Item2, 0);
                    currentHexPosition.Item1 += change.Item1;
                }
            }
            totalPerimeterDistance++;
        }
        TerritoriesPerCountry = (map.Count * map[0].Count) / (2 * totalCountryCount);
        while (totalCountries.Min(x => x.OwnedTerritories.Count) < TerritoriesPerCountry)
        {
            for (int i = 0; i < totalCountries.Count; i++)
            {
                if (totalCountries[i].OwnedTerritories.Count > TerritoriesPerCountry) continue;
                List<Territory> borderedHexes = new List<Territory>();
                foreach (Territory hex in ownedOutsideTerritories[i])
                {
                    if (hex.X + 1 < map[0].Count)
                    {
                        borderedHexes.Add(map[hex.Y][hex.X + 1]);
                        if (hex.Y % 2 == 1)
                        {
                            if (hex.Y + 1 < map.Count)
                                borderedHexes.Add(map[hex.Y + 1][hex.X + 1]);
                            if (hex.Y - 1 > -1)
                                borderedHexes.Add(map[hex.Y - 1][hex.X + 1]);
                        }
                    }
                    if (hex.X - 1 > -1)
                    {
                        borderedHexes.Add(map[hex.Y][hex.X - 1]);
                        if (hex.Y % 2 == 0)
                        {
                            if (hex.Y + 1 < map.Count)
                                borderedHexes.Add(map[hex.Y + 1][hex.X - 1]);
                            if (hex.Y - 1 > -1)
                                borderedHexes.Add(map[hex.Y - 1][hex.X - 1]);
                        }
                    }
                    if (hex.Y - 1 > -1)
                        borderedHexes.Add(map[hex.Y - 1][hex.X]);
                    if (hex.Y + 1 < map.Count)
                        borderedHexes.Add(map[hex.Y + 1][hex.X]);
                }
                ownedOutsideTerritories[i].Clear();
                while (borderedHexes.Count > 0)
                {
                    if (totalCountries[i].OwnedTerritories.Count > TerritoriesPerCountry) break;
                    Territory borderHex = borderedHexes[Random.Range(0, borderedHexes.Count)];
                    if (borderHex.Owner == null)
                    {
                        borderHex.Owner = totalCountries[i];
                        borderHex.Image.color = borderHex.Owner.textColor;
                        ownedOutsideTerritories[i].Add(borderHex);
                        totalCountries[i].OwnedTerritories.Add(borderHex);
                    }
                    borderedHexes.Remove(borderHex);
                }





            }
        }
        foreach (Country country in totalCountries)
            country.UpdateTerritoryBenefits();


    }
}

// even left odd right






/*
public void Generate()
{
    for (int i = originalImage.transform.parent.childCount - 1; i > 0; i--)
        GameObject.Destroy(originalImage.transform.parent.GetChild(i).gameObject);

    GameObject currentImage = originalImage;
    List<Vector3> occupiedPositions = new List<Vector3>();
    occupiedPositions.Add(originalImage.transform.localPosition);
    for (int i = 0; i < depth; i++)
    {
        GameObject newImage = GameObject.Instantiate(currentImage, currentImage.transform.position, new Quaternion(), currentImage.transform.parent);

        List<Vector3> possibleDirections = new List<Vector3>();
        foreach (Vector3 offset in hexoffsets)
        {
            Vector3 offsetPosition = DevTools.RoundVector3(newImage.transform.localPosition + new Vector3(imageSize.x * offset.x, imageSize.y * offset.y), 1);
            if (!occupiedPositions.Contains(offsetPosition)) // If the position in this direction is unoccupied                
                possibleDirections.Add(offset);
        }
        int newDirection = Random.Range(0, possibleDirections.Count);
        Vector3 newOffset = possibleDirections[newDirection];

        newImage.transform.localPosition += new Vector3(imageSize.x * newOffset.x, imageSize.y * newOffset.y);
        occupiedPositions.Add(DevTools.RoundVector3(newImage.transform.localPosition, 1));
        currentImage = newImage;
    }
    Vector3 v3_mostLeftHex = occupiedPositions.OrderBy(x => x.x).First();
    Vector3 v3_mostRightHex = occupiedPositions.OrderBy(x => x.x).Last();
    Vector3 v3_mostDownHex = occupiedPositions.OrderBy(x => x.y).First();
    Vector3 v3_mostUpHex = occupiedPositions.OrderBy(x => x.y).Last();
    Vector3 center = new Vector3(occupiedPositions.Average(x => x.x), occupiedPositions.Average(x => x.y));
    originalImage.transform.parent.localPosition = -center;

}    
*/



