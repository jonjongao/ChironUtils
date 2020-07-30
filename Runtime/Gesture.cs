using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Gesture
{
    public string gestureName;
    public List<Pose> relativePosePerJoint;
    public List<Pose> worldPosePerJoint;

    public bool hasGesture => string.IsNullOrEmpty(gestureName) == false;

    public Gesture(string name, List<Pose> relativePose, List<Pose> worldPose)
    {
        this.gestureName = name;
        this.relativePosePerJoint = relativePose;
        this.worldPosePerJoint = worldPose;
    }
}