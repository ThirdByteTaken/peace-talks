using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject originalImage;
    public Canvas canvas;
    public int depth;
    void Start()
    {
        SpriteRenderer spriteRenderer = originalImage.GetComponent<SpriteRenderer>();
        Vector3 rendererSize = spriteRenderer.bounds.size / canvas.GetComponent<RectTransform>().localScale.x;
        GameObject newImage;
        for (int i = 0; i < depth; i++)
        {
            newImage = GameObject.Instantiate(originalImage, originalImage.transform.parent);
            //print(spriteRenderer.sprite.rect.width + " " + spriteRenderer.sprite.rect.height);
            //print(spriteRenderer.bounds.size.x + " " + spriteRenderer.bounds.size.y);
            //print(canvas.GetComponent<RectTransform>().localScale.x + " " + canvas.GetComponent<RectTransform>().localScale.y);
            newImage.transform.localPosition = new Vector3(rendererSize.x * -.5f * (i + 1), rendererSize.y * .75f * (i + 1));
        }


    }
}
