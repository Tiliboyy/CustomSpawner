using System;
using UnityEngine;

namespace SerializedVector3;

[Serializable]
public class SerializedVector3
{
    public SerializedVector3(Vector3 vector)
    {
        X = vector.x;
        Y = vector.y;
        Z = vector.z;
    }

    public SerializedVector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public SerializedVector3()
    {
    }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Vector3 Parse()
    {
        return new(X, Y, Z);
    }

    public static implicit operator Vector3(SerializedVector3 vector)
    {
        return vector?.Parse() ?? Vector3.zero;
    }

    public static implicit operator SerializedVector3(Vector3 vector)
    {
        return new(vector);
    }

    public static implicit operator SerializedVector3(Quaternion rotation)
    {
        return new(rotation.eulerAngles);
    }

    public static implicit operator Quaternion(SerializedVector3 vector)
    {
        return Quaternion.Euler(vector);
    }
}