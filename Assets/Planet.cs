using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public Transform rotationCenter;
    public float rotationSpeed = 20.0f;
    public bool isCenter = false;

    private void Update(){
        if (isCenter) return;
        transform.RotateAround(rotationCenter.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void EnterAtmosphere(){
        GameObject.FindGameObjectWithTag("SolarSystem").GetComponent<SolarSystem>().SetCenter(transform);
        isCenter = true;
    }
    public void ExitAtmosphere(){
        GameObject.FindGameObjectWithTag("SolarSystem").GetComponent<SolarSystem>().RemoveCenter();
        isCenter = false;
    }
}
