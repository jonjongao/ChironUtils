using System;
using UnityEngine;

[Serializable]
public struct TransformLite
{
    /// <summary>
    /// LocalPosition
    /// </summary>
    public Vector3 a;
    /// <summary>
    /// WorldRotation
    /// </summary>
    public Quaternion b;

    public TransformLite(Vector3 localPosition, Quaternion rotation)
    {
        this.a = localPosition;
        this.b = rotation;
    }
}