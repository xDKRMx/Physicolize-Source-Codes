using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectObject : MonoBehaviour
{
    public GameObject[] Transparency_objects;
    public GameObject[] Objects;
    public Building Build;
    public ObjectLimit TheLimit;
    
    public void Object_Panel_Click(int index)
    {
        if(!TheLimit.Limit_Exceeded)
        {
            if (Build.Current_Object != null)
            {
                Destroy(Build.Current_Object.gameObject);

            }
            Build.Selected_Object = Objects[index];
            Build.Transparent_Object = Transparency_objects[index];
            Build.Object_selected = true;
            Build.IsCreate = true;
        }
       
    }
}
