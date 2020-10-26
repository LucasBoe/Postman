using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    
    public static Vector3 WithYZero (this Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }
}
