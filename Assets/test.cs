using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    SpriteRenderer spriteRenderer;
    Vector3 rendererSize;
    void Start()
    {
        spriteRenderer = originalImage.GetComponent<SpriteRenderer>();
        rendererSize = spriteRenderer.bounds.size / canvas.GetComponent<RectTransform>().localScale.x;
        print(new Vector3(0.00f, 0.00f, 0.00f) == new Vector3(0.00f, 0.00f, 0.00f));
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
            //print("**************occupied poses******************");
            //            occupiedPositions.ForEach(x => print(x));
            foreach (Vector3 offset in hexoffsets)
            {
                //print("potential pos: " + ());
                //print("open?: " + !occupiedPositions.Any(x => x == newImage.transform.localPosition + new Vector3(Mathf.Round(rendererSize.x * offset.x), Mathf.Round(rendererSize.y * offset.y))));
                Vector3 offsetPosition = DevTools.RoundVector3(newImage.transform.localPosition + new Vector3(rendererSize.x * offset.x, rendererSize.y * offset.y), 1);
                if (!occupiedPositions.Contains(offsetPosition)) // If the position in this direction is unoccupied                
                    possibleDirections.Add(offset);

            }

            int newDirection = Random.Range(0, possibleDirections.Count);
            Vector3 newOffset = possibleDirections[newDirection];

            newImage.transform.localPosition += new Vector3(rendererSize.x * newOffset.x, rendererSize.y * newOffset.y);
            occupiedPositions.Add(DevTools.RoundVector3(newImage.transform.localPosition, 1));
            currentImage = newImage;
        }
        //print("**************");
        //occupiedPositions.ForEach(x => print(x));

    }
}
