using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject originalImage;
    public Canvas canvas;
    public int depth;
    private static Vector3[] hexoffsets = new Vector3[]
    {
        new Vector3(-.5f, .75f), // Up and Left
        new Vector3(.5f, .75f), // Up and Right
        new Vector3(1f, 0f), // Right
        new Vector3(.5f, -.75f), // Down and Right
        new Vector3(-.5f, -.75f), // Down and Left
        new Vector3(-1f, 0f)  // Left
    };
    public float ScrollSpeed;
    Image image;
    Vector3 imageSize;
    void Start()
    {
        image = originalImage.GetComponent<Image>();
        imageSize = new Vector3(image.sprite.rect.width, image.sprite.rect.height);
    }
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
    //void Update()
    //{
    //    originalImage.transform.parent.parent.localScale += new Vector3(1, 1) * ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");
    //}

}
