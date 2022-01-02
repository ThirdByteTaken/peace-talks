using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class test : MonoBehaviour
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

    private Vector3 imageSize;
    private Vector3 mapFrameSize;
    private RectTransform rect_mapFrame;

    private List<List<Hex>> map = new List<List<Hex>>();
    void Start()
    {
        Image image = originalImage.GetComponent<Image>();
        imageSize = new Vector3(image.sprite.rect.width, image.sprite.rect.height);
        rect_mapFrame = originalImage.transform.parent.parent.GetComponent<RectTransform>();
    }
    public void GenerateHexGrid()//int height, int width)
    {
        originalImage.SetActive(true);
        Transform mapObject = originalImage.transform.parent;
        for (int i = mapObject.childCount - 1; i > 0; i--)
        {
            GameObject.Destroy(mapObject.GetChild(i).gameObject);
        }
        int height = Random.Range(1, 20);
        int width = Random.Range(1, 20);
        Vector3 sizeRequired = new Vector3((width + .5f) * imageSize.x, ((height * .75f) + .25f) * imageSize.y);
        float scaleFactor = Mathf.Min(rect_mapFrame.sizeDelta.x / sizeRequired.x, rect_mapFrame.sizeDelta.y / sizeRequired.y);
        mapObject.localScale = new Vector3(scaleFactor, scaleFactor);
        //if (sizeRequired.x > mapObject.localScale.x) mapObject.localScale = new Vector3(rect_mapFrame.sizeDelta.x / sizeRequired.x, rect_mapFrame.sizeDelta.x / sizeRequired.x);
        //if (sizeRequired.y > mapObject.localScale.y) mapObject.localScale = new Vector3(rect_mapFrame.sizeDelta.y / sizeRequired.y, rect_mapFrame.sizeDelta.y / sizeRequired.y);

        print(rect_mapFrame.sizeDelta + " / " + sizeRequired + " = " + rect_mapFrame.localScale);
        mapFrameSize = sizeRequired;
        Vector3 rowStart = new Vector3(-((mapFrameSize.x / 2) - (imageSize.x / 2)), -((mapFrameSize.y / 2) - (imageSize.y / 2)));
        for (int i = 0; i < height; i++)
        {

            map.Add(new List<Hex>());
            Vector3 rowPos = rowStart;
            Transform rowParent = new GameObject().transform;
            rowParent.parent = mapObject;
            rowParent.localScale = Vector3.one;
            for (int j = 0; j < width; j++)
            {
                GameObject newGameobject = GameObject.Instantiate(originalImage, rowParent);
                newGameobject.transform.localPosition = rowPos;
                map[i].Add(new Hex() { x = i, y = j, gameObject = newGameobject });
                rowPos.x += imageSize.x;
            }
            rowStart = new Vector3(rowStart.x + (((i % 2 == 1) ? -1 : 1) * imageSize.x / 2), rowStart.y += .75f * imageSize.y);
        }
        originalImage.SetActive(false);
    }


}

public class Hex
{
    public int x;
    public int y;
    public GameObject gameObject;
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

