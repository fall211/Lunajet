using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public Transform rotationCenter;
    public float rotationSpeed = 20.0f;
    public bool isCenter = false;

    private List<Transform> otherPlanets = new();

    private void Start(){
        GameObject[] Planets = GameObject.FindGameObjectsWithTag("Planet");
        foreach (GameObject planet in Planets){
            otherPlanets.Add(planet.transform);
        }
    }
    private void Update(){
        if (isCenter) return;
        transform.RotateAround(rotationCenter.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void EnterAtmosphere(){
        // set all other planets to orbit this planet
        foreach (Transform planet in otherPlanets){
            planet.GetComponent<Planet>().rotationCenter = transform;
        }
        isCenter = true;
        GameObject.Find("ObjectShifter").GetComponent<ObjectShifter>().ShiftObjects(transform.position);
    }
    public void ExitAtmosphere(){
        // set all other planets to orbit the sun
        foreach (Transform planet in otherPlanets){
            planet.GetComponent<Planet>().rotationCenter = GameObject.Find("Sun").transform;
        }
        isCenter = false;
    }
}
