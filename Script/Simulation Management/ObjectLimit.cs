using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectLimit : MonoBehaviour
{
    public GameObject Current_Main_Object;
    public int Parent_object_Limit;
    public float Current_Object_Count;
    public Image Limit_Indicator;
    public bool Limit_Exceeded = false;
    public Color Default_Limit_color;
    // Update is called once per frame
    void Update()
    {
        Current_Object_Count = Current_Main_Object.transform.childCount;

        if(Parent_object_Limit >= Current_Object_Count)
        {
            Limit_Exceeded = false;
            float Fill_Amount = Current_Object_Count / 100;
            Limit_Indicator.fillAmount = Fill_Amount;
            
            Limit_Indicator.color = Default_Limit_color;
        }
        else
        {
            Limit_Indicator.fillAmount = 1;
            Limit_Exceeded = true;
            Limit_Indicator.color = Color.red;
        }
    }
}
