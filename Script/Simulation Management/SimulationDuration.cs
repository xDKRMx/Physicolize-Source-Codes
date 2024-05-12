using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class SimulationDuration : MonoBehaviour
{
    public GameObject Main_Simulation_Object;
    public Transform[] Main_Simulation_Child_Objects;
    public  bool Simulation_Play = false;
    bool Simulation_Time_Control;
    public bool Simulation_Clear;
    public Sprite Resume_Icon;
    public Sprite Pause_Icon;
    public GameObject TimeControl_Button;
    //Object Panel Varýables
    public ObjectAdjustment Obj_Manager;

    void Update()
     {
         Main_Simulation_Child_Objects = Main_Simulation_Object.GetComponentsInChildren<Transform>();
        if (Simulation_Play == false)
        {

            for (int i = 0; i < Main_Simulation_Child_Objects.Length; i++)
            {
                if (Main_Simulation_Child_Objects[i].GetComponent<Rigidbody>())
                {
                    Main_Simulation_Child_Objects[i].GetComponent<Rigidbody>().isKinematic = true;

                }
                
            }
            if (Simulation_Time_Control)
            {
                Simulation_Time_Control = false;
                //Time.timeScale = 0f;
            }
        }
        else
        {
            for (int i = 0; i < Main_Simulation_Child_Objects.Length; i++)
            {

                if (Main_Simulation_Child_Objects[i].GetComponent<Rigidbody>())
                {
                    if (!Main_Simulation_Child_Objects[i].GetComponent<StaticObject>() || !Main_Simulation_Child_Objects[i].GetComponentInParent<StaticObject>())
                    {
                        Main_Simulation_Child_Objects[i].GetComponent<Rigidbody>().isKinematic = false;
                    }
                    else
                    {
                        Main_Simulation_Child_Objects[i].GetComponent<Rigidbody>().isKinematic = true;
                    }

                }
                if (Main_Simulation_Child_Objects[i].GetComponent<ALLforces>() && !Main_Simulation_Child_Objects[i].GetComponent<ALLforces>().Is_Force_Applied) Main_Simulation_Child_Objects[i].GetComponent<ALLforces>().Apply_All_Memory();
            }
            if(Simulation_Time_Control)
            {
                Simulation_Time_Control = false;
                //Time.timeScale = 1f;
            }
            
        }
        
    }
    public void SimulationControl()
    {
        if(Simulation_Play)
        {
            Simulation_Play = false;
            Simulation_Time_Control = true;
            TimeControl_Button.GetComponent<Image>().sprite = Pause_Icon;
           
        }
        else
        {
            Simulation_Play = true;
            Simulation_Time_Control = true;
            TimeControl_Button.GetComponent<Image>().sprite = Resume_Icon;
        }
    }
    public void SimulationClear()
    {
        Simulation_Play = false;
        for (int i = 0; i < Main_Simulation_Child_Objects.Length; i++)
        {
            if(Main_Simulation_Child_Objects[i].gameObject != Main_Simulation_Object.gameObject) Destroy(Main_Simulation_Child_Objects[i].gameObject);
        }
        TimeControl_Button.GetComponent<Image>().sprite = Pause_Icon;
       
    }
}
