using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TheMovingSphere : MonoBehaviour
{

    [SerializeField] private Color sphereColor;
    private MeshRenderer thisRenderer;

    Material newSphereMaterial;

    // Start is called before the first frame update
    void Start()
    {
         thisRenderer = this.gameObject.GetComponent<MeshRenderer>();
         newSphereMaterial = new Material(thisRenderer.material);

        newSphereMaterial.SetColor("_Color", GetRandomColor());
        thisRenderer.material = newSphereMaterial;
    }

    Color GetRandomColor()
    {
        return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("You pressed the left mouse button");


            newSphereMaterial.SetColor("_Color", GetRandomColor());
            thisRenderer.material = newSphereMaterial;

        }
        if (Input.GetKeyDown("a"))
        {
            Debug.Log("You pressed the 'a' key");
        }


    }
}