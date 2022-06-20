using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public class AimIndicator : MonoBehaviour
{
    public float Length = 10;
    public int BounceCount = 1;
    private LineRenderer lr;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        Aim();
    }

    private void Aim()
    {
        var rotation = transform.rotation;
        Vector2 origin = transform.position;
        Vector2 direction = rotation * Vector2.up;
        List<Vector3> positions = new List<Vector3> {origin};
        RaycastHit2D hit;
        bool foundPlayer = false;
        for (int i = 0; i < BounceCount; i++)
        {
            hit = Physics2D.Raycast(origin, direction, float.MaxValue,~LayerMask.GetMask("Bullet"));
            if (hit.collider!=null)
            {
                if (hit.collider.gameObject.layer==LayerMask.NameToLayer("Player"))
                {
                    positions.Add(hit.point);
                    foundPlayer = true;
                    break;
                }
                Vector2 reflection = Vector2.Reflect(hit.point - origin, hit.normal).normalized;
                Debug.DrawLine(origin, hit.point, Color.green);
                //Debug.DrawRay(hit.point, reflection * 3, Color.red);
                positions.Add(hit.point);
                origin = hit.point;
                direction = reflection;
            }
        }

        if (!foundPlayer)
        {
            hit = Physics2D.Raycast(origin, direction, float.MaxValue,~LayerMask.GetMask("Bullet"));
            Debug.DrawRay(origin, direction * 3, Color.red);

            if (hit.collider!=null)
            {
                positions.Add(hit.point);
            }
            else
            {
                positions.Add(origin+direction*Length);
            }
        }
        lr.positionCount = positions.Count;
        lr.SetPositions(positions.ToArray());
    }
}