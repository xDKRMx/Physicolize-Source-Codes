using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateControl : MonoBehaviour
{
    public GameObject Enterred_object;
    public bool No_Intreaction = true;
    public bool Intereaction_Plane;
    //AUTO CONSTRUCT
    public bool Auto_Construct_Active;
    public GameObject Auto_Construct_Object;
    public string _Auto_Construction_Type;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Contains("CenterPivot") || other.gameObject.tag.Contains("BottomPivot") || other.gameObject.tag.Contains("ChildObject"))
        {

            Enterred_object = other.gameObject;

            No_Intreaction = false;
        }
        if(other.gameObject.tag.Contains("Plane"))
        {
            Intereaction_Plane = true;
        }
        else
        {
            Intereaction_Plane = false;
        }
       
       
    }
    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag.Contains("CenterPivot") || other.gameObject.tag.Contains("BottomPivot") || other.gameObject.tag.Contains("ChildObject"))
        {

            No_Intreaction = true;
        }
    }
}
