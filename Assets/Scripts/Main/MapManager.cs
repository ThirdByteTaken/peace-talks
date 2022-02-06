using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

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
    private static Transform mapObject;
    public List<TerrainType> TerrainTypes;
    public static Dictionary<string, TerrainType> terrainTypes = new Dictionary<string, TerrainType>();

    public int TerritoriesPerCountry;


    private static Vector3 imageSize;
    private static Vector3 mapFrameSize;
    private static RectTransform rect_mapFrame;
    private static Image TerritoryStatBox;
    private static Dictionary<string, TMP_Text> StatBoxTexts = new Dictionary<string, TMP_Text>();
    private static Button StatBoxOwnerButton;

    private static Transform OriginalOwnerBanner;

    private static List<List<Territory>> map = new List<List<Territory>>();

    private static List<RectTransform> rect_OwnerBanners = new List<RectTransform>();
    private Main main;
    void Start()
    {
        main = Main.Instance;
        foreach (TerrainType terrain in TerrainTypes)
            terrainTypes.Add(terrain.Name, terrain);
        Image image = originalImage.GetComponent<Image>();
        imageSize = new Vector3(image.sprite.rect.width, image.sprite.rect.height);
        rect_mapFrame = originalImage.transform.parent.parent.GetComponent<RectTransform>();
        TerritoryStatBox = originalImage.transform.parent.parent.GetChild(1).GetComponent<Image>();
        OriginalOwnerBanner = originalImage.transform.parent.GetChild(1);
        for (int i = 0; i < TerritoryStatBox.transform.childCount; i++)
        {
            var child = TerritoryStatBox.transform.GetChild(i);
            TMP_Text textComponent;
            if (child.TryGetComponent<TMP_Text>(out textComponent)) StatBoxTexts.Add(child.name, textComponent);
        }
        StatBoxOwnerButton = StatBoxTexts["Owner"].GetComponent<Button>();

    }
    public void GenerateHexGrid()
    {
        map.Clear();
        rect_OwnerBanners.Clear();
        mapObject = originalImage.transform.parent;
        for (int i = mapObject.childCount - 1; i > -1; i--)
        {
            if (mapObject.GetChild(i).Equals(originalImage.transform) || mapObject.GetChild(i).Equals(OriginalOwnerBanner)) continue; // Skip over original hexagon and original owner banner
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
                newGameobject.SetActive(true);
                map[i].Add(new Territory(j, i, newGameobject, (Mathf.Min(i, j) == 0 || height - i == 1 || width - j == 1) ? terrainTypes["Water"] : terrainTypes["Land"]));
                rowPos.x += imageSize.x;
            }
            rowStart = new Vector3(rowStart.x + (((i % 2 == 1) ? -1 : 1) * imageSize.x / 2), rowStart.y += .75f * imageSize.y);
        }
        SetCountryOwnerships();
    }
    private void SetCountryOwnerships()
    {
        float perimeter = (2 * map.Count) + (2 * map[0].Count) - 4;

        var totalCountries = new List<Country>(main.cnt_Players);
        float tilesBetweenCountryStarts = perimeter / (totalCountries.Count());
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
                map[currentHexPosition.Item2][currentHexPosition.Item1].Image.color = newOwnerCountry.textColor;
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
        while (totalCountries.Min(x => x.OwnedTerritories.Count) < TerritoriesPerCountry) // Runs until all countries have enough territories
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
        totalCountries.ForEach(x => x.UpdateTerritoryBenefits());
        foreach (Country country in totalCountries)
        {
            var averageCountryTerritoryPosition = new Vector2(country.OwnedTerritories.Average(x => x.GameObject.transform.localPosition.x), country.OwnedTerritories.Average(x => x.GameObject.transform.localPosition.y));
            var go_countryOwnerBanner = GameObject.Instantiate(OriginalOwnerBanner, OriginalOwnerBanner.parent);
            go_countryOwnerBanner.gameObject.SetActive(true);
            go_countryOwnerBanner.localPosition = averageCountryTerritoryPosition;
            var img_countryOwnerBanner = go_countryOwnerBanner.GetComponent<Image>();
            var txt_countryOwnerBanner = go_countryOwnerBanner.transform.GetChild(0).GetComponent<TMP_Text>();
            var countryColor = country.textColor;
            var countryLuminance = DevTools.ColorLuminance(countryColor);
            if (countryLuminance < 0.5)
            {
                img_countryOwnerBanner.color = Color.white;
                txt_countryOwnerBanner.color = Color.black;
            }
            var rect_countryOwnerBanner = img_countryOwnerBanner.rectTransform;

            txt_countryOwnerBanner.text = country.name;
            txt_countryOwnerBanner.ForceMeshUpdate(); // needed to get extents            
            var terrritoryLength = (country.OwnedTerritories[0].Image.rectTransform.sizeDelta.x);
            rect_countryOwnerBanner.sizeDelta = new Vector3(txt_countryOwnerBanner.textBounds.extents.x * 2, txt_countryOwnerBanner.textBounds.extents.y * 2);
            rect_OwnerBanners.Add(rect_countryOwnerBanner);
        }


        rect_mapFrame.gameObject.SetActive(true);

        TerritoryStatBox.rectTransform.sizeDelta = new Vector3(imageSize.x * 1.5f, imageSize.y * .75f) * mapObject.localScale.x;
    }

    public static void MoveStatBox(Territory territory)
    {

        Vector3 statBoxSize = TerritoryStatBox.rectTransform.sizeDelta;
        Vector3 territoryPosition = territory.GameObject.transform.localPosition * mapObject.localScale.x;
        bool boxExtendsOverTop = territoryPosition.y + statBoxSize.y > rect_mapFrame.sizeDelta.y / 2;
        bool boxExtendsOverSide = territoryPosition.x + statBoxSize.x > rect_mapFrame.sizeDelta.x / 2;
        TerritoryStatBox.transform.localPosition = territoryPosition + new Vector3((boxExtendsOverSide ? -1 : 1) * statBoxSize.x / 2, (boxExtendsOverTop ? -1 : 1) * statBoxSize.y / 2);
        var rect_StatBox = DevTools.GetPositionedRect(TerritoryStatBox.rectTransform);
        rect_OwnerBanners.ForEach(x => x.gameObject.SetActive(!rect_StatBox.Overlaps(DevTools.GetPositionedRect(x))));

        var territoryColor = territory.Image.color;
        var territoryLuminance = DevTools.ColorLuminance(territoryColor);
        TerritoryStatBox.color = (territoryLuminance > .5f) ? Color.black : Color.white;
        TerritoryStatBox.GetComponentsInChildren<TextMeshProUGUI>().ToList().ForEach(x => x.color = (TerritoryStatBox.color == Color.black) ? Color.white : Color.black);


        PopulateStatBoxInfo(territory);

    }
    private static void PopulateStatBoxInfo(Territory territory)
    {
        StatBoxTexts["Money"].text = "+" + territory.Terrain.MoneyProduction.ToString();
        StatBoxTexts["War Power"].text = "+" + territory.Terrain.WarPowerProduction.ToString();
        if (territory.Owner == null)
        {
            StatBoxTexts["Owner"].text = "Currently Unowned";
            StatBoxTexts["Owner"].raycastTarget = false;
            StatBoxOwnerButton.onClick.RemoveAllListeners();
        }
        else
        {
            StatBoxTexts["Owner"].text = "Owned by: " + territory.Owner.CountryName;
            StatBoxTexts["Owner"].raycastTarget = true;
            StatBoxOwnerButton.onClick.AddListener(territory.OpenLeaderView);
        }

    }
}








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



