using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShifter : MonoBehaviour
{
    public GameObject[] objects;

    public void ShiftObjects(Vector3 shift){
        foreach (GameObject obj in objects){
            obj.transform.position -= shift;
        }

    }


}
