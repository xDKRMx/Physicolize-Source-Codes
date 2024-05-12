using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentCreateControl : MonoBehaviour
{
    //ANY INTERACTION CONTROL
    public List<bool>  Create_control_bools = new List<bool>();
    public bool Object_Create_Okay;
    CreateControl[] Current_game_objects;
    //PLANE INTERACTION CONTROL
    public List<bool> Plane_Control_Bools = new List<bool>();
    public bool Plane_Intereaction;
    void Start()
    {
        Current_game_objects = transform.gameObject.GetComponentsInChildren<CreateControl>();
        foreach (var item in Current_game_objects)
        {
            Create_control_bools.Add(item.No_Intreaction);
            Plane_Control_Bools.Add(item.Intereaction_Plane);
        }
      
    }
    void Update()
    {
        for (int i = 0; i < Current_game_objects.Length; i++)
        {
            if (Current_game_objects[i].No_Intreaction == false)
            {
                Create_control_bools[i] = false;
            }
            else
            {
                Create_control_bools[i] = true;
            }

            if (Current_game_objects[i].Intereaction_Plane)
            {
                Plane_Control_Bools[i] = true;
            }
            else
            {
                Plane_Control_Bools[i] = false;
            }
        }
        //ANY INTERACTION CONTROL
        if (Create_control_bools.Contains(false))
        {
            Object_Create_Okay = false;
        }
        else
        {
            Object_Create_Okay = true;
        }

        //PLANE INTERACTION CONTROL

        if (Plane_Control_Bools.Contains(true))
        {
            Plane_Intereaction = true;
        }
        else
        {
            Plane_Intereaction = false;
        }
    }
}
