﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalItem : Item
{
    public override void SetEdited(bool edited)
    {

    }

    public override Vector2 GetDragOffset(Vector3 position)
    {
        return new Vector2(Size.x / 2.0f, position.y);
        // return new Vector2(Vector3.Dot(Dir.Vector, position), position.y);
    }

    public override Plane GetOffsetPlane(Vector3 position)
    {
        Vector3 dir = Vector3.Cross(Dir.Vector, Vector3.up);
        float distance = Vector3.Dot(dir.normalized, position);
        Plane plane = new Plane(dir, -distance);
        // Debug.Log("offset plane: " + plane);
        return plane;
    }

}
