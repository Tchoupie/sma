using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{
    public bool affected; //indique si un paquet a un agent assigné à son transport

    void Start()
    {
        affected = false;
    }

}
