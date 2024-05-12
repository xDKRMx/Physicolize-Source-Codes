using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ALLforces : MonoBehaviour
{
    public List<Vector3> Force_Direction = new List<Vector3>();
    public List<Vector3> Torque_Direction = new List<Vector3>();
    public float Impacting_time;
    public bool Impacting_process;
    public GameObject Impacting_Force_Direction_Indicator;
    [SerializeField]
    List<GameObject> Over_view_Indicator_Objects = new List<GameObject>();
    private GameObject Active_indicator;
    public GameObject Indicator_Parent;
    public GameObject Object_itself;
    bool Show_Indicator_Object;
    public SimulationDuration Simulation_Duration;
    public ObjectAdjustment Obj_adj;
    //List For Force Memory in order to impact this object when simulation begins
    List<Force_Memory> F_M = new List<Force_Memory>();
    public bool Is_Force_Applied;
    public void Impacting_Object(bool Is_It_Force)
    {
        Impacting_process = true;
        Show_Indicator_Object = true;
        if (Is_It_Force)
        {
            if(Simulation_Duration.Simulation_Play)
            {
                StartCoroutine(Impact_Time(true, Impacting_time));
                Indicate_direction_Onpanel(Force_Direction[Force_Direction.Count - 1]);
            }
            else
            {
                Force_Memory F_c = new Force_Memory(Impacting_time, Object_itself, Force_Direction[Force_Direction.Count - 1], true);
                F_M.Add(F_c);
                Force_Direction.Remove(Force_Direction[Force_Direction.Count - 1]);
                Is_Force_Applied = false;
            }
        } 
        else
        {
            if (Simulation_Duration.Simulation_Play)
            {
                StartCoroutine(Impact_Time(false, Impacting_time));
            }
            else
            {
                Force_Memory F_c = new Force_Memory(Impacting_time, Object_itself, Torque_Direction[Torque_Direction.Count - 1], false);
                F_M.Add(F_c);
                Torque_Direction.Remove(Torque_Direction[Torque_Direction.Count - 1]);
                Is_Force_Applied = false;
            }
           
        }
    }
    public IEnumerator Impact_Time(bool Is_It_Force,float Time_Process)
    {
        if(Is_It_Force)
        {
            float elapsedTime = 0f;
             while (elapsedTime < Time_Process)
             {
                //It stands for if the Time_process is Infinity conceal the object when the selected object is different from this object which is  applied force on itself
                if (Object_itself != Obj_adj.Selected_Object && Show_Indicator_Object && Active_indicator != null)
                {
                    Show_Indicator_Object = false;
                    Over_view_Indicator_Objects.Remove(Over_view_Indicator_Objects[Over_view_Indicator_Objects.Count - 1]);
                    Destroy(Active_indicator.gameObject);
                }
                //Force to the object as long time as the time_process has
                if (Impacting_process)
                        Object_itself.GetComponent<Rigidbody>().AddForce(Force_Direction[Force_Direction.Count - 1], ForceMode.Force);
                    elapsedTime += Time.deltaTime;
                    if (Active_indicator != null) Active_indicator.transform.position = Indicator_Parent.transform.position;
                    yield return null;
             }
            Impacting_process = false;
            Show_Indicator_Object = false;
            Over_view_Indicator_Objects.Remove(Over_view_Indicator_Objects[Over_view_Indicator_Objects.Count - 1]);
            Destroy(Active_indicator.gameObject);
        }
        else
        {
            float elapsedTime = 0f;
            while (elapsedTime < Time_Process)
            {
                //Torque to the object as long time as the time_process has
                if (Impacting_process)
                    Object_itself.GetComponent<Rigidbody>().AddTorque(-Torque_Direction[Torque_Direction.Count - 1], ForceMode.Impulse);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
          
            Impacting_process = false;
            //Destroy(Over_view_Indicator_Objects[Over_view_Indicator_Objects.Count - 1]);
            //Over_view_Indicator_Objects.Remove(Over_view_Indicator_Objects[Over_view_Indicator_Objects.Count - 1]);
        }

    }

    void Indicate_direction_Onpanel(Vector3 Direction)
    {
      Vector3 Direction_rotation = Direction.normalized;
      GameObject Indicator_Object = Instantiate(Impacting_Force_Direction_Indicator, Indicator_Parent.transform.position, Quaternion.identity);
      Active_indicator = Indicator_Object;
      Active_indicator.transform.SetParent(Indicator_Parent.transform);
      Active_indicator.transform.localRotation = Quaternion.LookRotation(Direction_rotation);
      Over_view_Indicator_Objects.Add(Indicator_Object);
       
    }
    public void Apply_All_Memory()
    {
        Is_Force_Applied = true;
        if (F_M.Count >0)
        {
            foreach (Force_Memory item in F_M)
            {
                Impacting_time = item.Impact_duration;
                Object_itself = item.Selected_game_Object;
                if (item.Is_It_force) Force_Direction.Add(item.Impact_Magnitude);
                else Torque_Direction.Add(item.Impact_Magnitude);
                Impacting_Object(item.Is_It_force);
            }
        }
    }
}
public class Force_Memory
{
    public float Impact_duration;
    public GameObject Selected_game_Object;
    public Vector3 Impact_Magnitude;
    public bool Is_It_force;
    public Force_Memory(float Impact_duration, GameObject Selected_game_Object , Vector3 Impact_Magnitude , bool Is_It_force)
    {
        this.Impact_duration = Impact_duration;
        this.Selected_game_Object = Selected_game_Object;
        this.Impact_Magnitude = Impact_Magnitude;
        this.Is_It_force = Is_It_force;
    }
}
