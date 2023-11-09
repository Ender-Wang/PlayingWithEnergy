using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
public class MyVector3Comparer : IEqualityComparer<Vector3>
{
    public static MyVector3Comparer Instance = new MyVector3Comparer();
    public bool Equals(Vector3 a, Vector3 b)
    {
        return DistanceSquared(a, b) < 1f;
    }

    public double DistanceSquared(Vector3 a, Vector3 b)
    {
        var c = a - b;
        return c.x * c.x + c.y * c.y + c.z * c.z;
    }
    public int GetHashCode(Vector3 obj)
    {

        unchecked
        {
            int hash = 17;
            hash = hash * 23 + obj.x.GetHashCode();
            hash = hash * 23 + obj.y.GetHashCode();
            hash = hash * 23 + obj.z.GetHashCode();
            return hash;
        }
    }
}

