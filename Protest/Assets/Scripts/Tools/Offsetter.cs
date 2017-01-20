using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offsetter : Base
{

    void Start()
    {

    }

    void Update()
    {
        transform.SetAsLastSibling();
    }
}
