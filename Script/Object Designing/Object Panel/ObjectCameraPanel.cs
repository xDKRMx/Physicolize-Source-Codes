using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCameraPanel : MonoBehaviour
{
    [Header("Camera viewing")]
    public bool Is_Object_Active = false;
    public GameObject Body_Diagram;
    public GameObject Camera_Parent_Object;
    public GameObject[] Indicator_Objects;
    public GameObject[] Point_GameObjects;
    public Camera Camera;
    public GameObject Camera_Point;
    public Vector3 Distance_Offset;
    [SerializeField]
    Vector3 Offset;
    public GameObject Selected_object;
    [Header("UI Panel viewing")]
    public GameObject Camera_view;

    void Update()
     {
        if(Is_Object_Active && Selected_object != null)
        {
            //If the game object selected show it on the Object Settings panel
            Camera.enabled = true;
            Body_Diagram.SetActive(true);
            Camera_view.SetActive(true);
            //Adjust the rotations of force and torque indicator objects
           
            for (int i = 0; i < Indicator_Objects.Length; i++)
            {
                Indicator_Objects[i].transform.position = Point_GameObjects[i].transform.position;
            }
            //Locate the camera as much appropriate as the camera view can be

             Offset = new Vector3(-Distance_Offset.x, -Distance_Offset.y, -Distance_Offset.z);
            Camera_Parent_Object.transform.position = Selected_object.transform.position + Offset;
            if (Camera_Parent_Object.transform.parent != Body_Diagram) Camera_Parent_Object.transform.SetParent(Body_Diagram.transform);

        }
        else
        {
            Camera.enabled = false;
            Body_Diagram.SetActive(false);
            Camera_view.SetActive(false);
            Selected_object = null;
        }
        
     }
}
