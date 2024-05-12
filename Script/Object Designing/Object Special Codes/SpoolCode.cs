using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpoolCode : MonoBehaviour
{
    public Building Build;
    public GameObject[] Spool_Anchor_Objects;
    public GameObject Connected_Object;
    public RaycastHit hit;
    public Transform Lowest_Point;
    public Transform Toppest_Point;
    private FixedJoint Object_Fixed_Joint;
    private bool Spool_Connection;
    public void Spool_Anchorage()
    {
       
            foreach (var Anchor_Object in Spool_Anchor_Objects)
            {
               Spool_Connection = Spool_Connection_Control(Lowest_Point.gameObject);
               if (Spool_Connection)
               {
                Anchor_Object.SetActive(true);
                Object_Fixed_Joint = Anchor_Object.AddComponent<FixedJoint>();
                Object_Fixed_Joint.connectedBody = Connected_Object.GetComponent<Rigidbody>();
                Object_Fixed_Joint.breakForce = 1000f;
                Object_Fixed_Joint.breakTorque = 1000f;
               }
               else
               {
                if (Anchor_Object.GetComponent<FixedJoint>()) Destroy(Anchor_Object.GetComponent<FixedJoint>());
               }
            }
       
       
    }
    private bool Spool_Connection_Control(GameObject The_Lowest_Point)
    {
        Vector3 rayDirection = The_Lowest_Point.transform.InverseTransformDirection(Vector3.up);
        RaycastHit _hit;
        Debug.DrawRay(The_Lowest_Point.transform.position, rayDirection, Color.red, 30f);

        if (Physics.Raycast(The_Lowest_Point.transform.position, rayDirection, out _hit, float.MaxValue))
        {
            float Distance = Mathf.Abs(Vector3.Distance(The_Lowest_Point.transform.position, _hit.point));
            float Limit_Distance = Mathf.Abs(Vector3.Distance(The_Lowest_Point.transform.position, Toppest_Point.transform.position));
            if (Distance > Limit_Distance) return false;
            else return true;
        }
        else return false;
    }
}
