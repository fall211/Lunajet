using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulatePlanets : MonoBehaviour
{
    public GameObject[] prefabsToPlace;
    private GameObject meshObject;
    private Mesh planetMesh;
    public int numberOfObjects;


    private void Start(){
        meshObject = GetComponentInChildren<MeshFilter>().gameObject;
        planetMesh = meshObject.GetComponent<MeshFilter>().mesh;
        Populate();
    }

    private void Populate(){
        // place objects randomly on the surface of the planet
        for (int i = 0; i < numberOfObjects; i++){
            Vector3 randomPoint = Random.onUnitSphere;
            GameObject prefab = prefabsToPlace[Random.Range(0, prefabsToPlace.Length)];
            Vector3 pointOnSurface = meshObject.transform.localScale.x * planetMesh.bounds.extents.x * randomPoint + transform.position - (randomPoint * 0.4f);
            GameObject obj = Instantiate(prefab, pointOnSurface, Quaternion.FromToRotation(Vector3.up, randomPoint));
            obj.transform.localScale = new Vector3(2f, 2f, 2f);
            obj.transform.parent = transform;
        }
    }
}
