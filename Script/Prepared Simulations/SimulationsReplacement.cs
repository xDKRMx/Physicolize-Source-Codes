using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationsReplacement : MonoBehaviour
{
    public int Simulation_Index;
    [Header("Environment Variables")]
    public SystemSettings System_Setting;
    [Header("Simulation one Variables")]
    public GameObject[] Friction_none_Objects_1;
    public GameObject Bounciness_Wall;
    [Header("Simulation two Variables")]
    public GameObject[] Friction_none_Objects_2;
    public GameObject Sphere1_1;
    public GameObject Sphere2_1;

    public void Simulation_Adjustments(int Index)
    {
        switch(Index)
        {
            case 1:

                //Simulation one to Adjust World settings
                System_Setting.IsFriction(false);
                System_Setting.IsDrag(false);
                foreach (var item in Friction_none_Objects_1)
                {
                    //Physic Materail Friction zero for no Friction
                    if (item.GetComponent<Collider>().material)
                    {
                        item.GetComponent<Collider>().material.staticFriction = 0f;
                        item.GetComponent<Collider>().material.dynamicFriction = 0f;
                    }
                    else
                    {
                        PhysicMaterial Col = new PhysicMaterial();
                        Col.staticFriction = 0f;
                        Col.dynamicFriction = 0f;
                        item.GetComponent<Collider>().material = Col;
                    }
                    //Rigidbody Drag zero for no Friction
                    if (item.GetComponent<Rigidbody>()) item.GetComponent<Rigidbody>().drag = 0;
                }

                Bounciness_Wall.GetComponent<Collider>().material.bounciness = 1;
                break;
            case 2:
                //Simulation two to Adjust World settings
                System_Setting.IsFriction(false);
                System_Setting.IsDrag(false);
                foreach (var item in Friction_none_Objects_2)
                {
                    //Physic Materail Friction zero for no Friction

                    if (item.GetComponent<Collider>().material)
                    {
                        item.GetComponent<Collider>().material.staticFriction = 0f;
                        item.GetComponent<Collider>().material.dynamicFriction = 0f;
                    }
                    else
                    {
                        PhysicMaterial Col = new PhysicMaterial();
                        Col.staticFriction = 0f;
                        Col.dynamicFriction = 0f;
                        item.GetComponent<Collider>().material = Col;
                    }
                    //Rigidbody Drag zero for no Friction

                    if (item.GetComponent<Rigidbody>()) item.GetComponent<Rigidbody>().drag = 0;
                }
                Sphere1_1.GetComponent<Rigidbody>().mass = 10;
                Sphere2_1.GetComponent<Rigidbody>().mass = 1;

                break;

        }

    }
}
