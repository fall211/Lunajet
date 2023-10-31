using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    public Transform centeredPlanet;
    private float rotationSpeed;

    private void Update(){
        if (centeredPlanet == null) return;
        
        transform.RotateAround(centeredPlanet.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void SetCenter(Transform planet){
        centeredPlanet = planet;
        rotationSpeed = planet.GetComponent<Planet>().rotationSpeed;
        planet.parent = null;
    }
    public void RemoveCenter(){
        if (centeredPlanet == null) return;
        centeredPlanet.transform.parent = transform;
        centeredPlanet = null;
    }
}
