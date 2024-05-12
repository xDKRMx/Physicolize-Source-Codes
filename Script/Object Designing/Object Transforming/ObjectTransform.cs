using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum TransformType
{

    Rotation,
    Scale,
    Position
}
public class ObjectTransform : MonoBehaviour
{
    [Header("Scripts")]
    public Building Build;
    [Header("Object Rotating")]
    Quaternion rotation;
    public GameObject SpecifiedObject;
    public TMP_InputField X_Text_Rotation;
    public TMP_InputField Y_Text_Rotation;
    public TMP_InputField Z_Text_Rotation;
    public bool Object_activity;
    public bool isRotating;
    public string _Axis;
    [Header("Object Scaling")]
    public Vector3 Default_Scale;
    public bool isScaling;
    public TMP_InputField X_Scale_Panel;
    public TMP_InputField Y_Scale_Panel;
    public TMP_InputField Z_Scale_Panel;
    public float X_scale;
    public float Y_scale;
    public float Z_scale;
    [Header("Object Position")]
    public bool isPositioning;
    public TMP_InputField X_Position_Panel;
    public TMP_InputField Y_Position_Panel;
    public TMP_InputField Z_Position_Panel;
    public float X_position;
    public float Y_position;
    public float Z_position;
   public Vector3 Added_Position;

    public void ShowTransform()
    {
        Object_activity = true;
        //Match the  defualt Rotation value to the variables

        X_Text_Rotation.text = SpecifiedObject.transform.rotation.eulerAngles.x.ToString();
        Y_Text_Rotation.text = SpecifiedObject.transform.rotation.eulerAngles.y.ToString();
        Z_Text_Rotation.text = SpecifiedObject.transform.rotation.eulerAngles.z.ToString();
        //Match the  defualt Scale value to the variables
        Default_Scale = SpecifiedObject.transform.localScale;
        X_scale = Default_Scale.x;
        Y_scale = Default_Scale.y;
        Z_scale = Default_Scale.z;
      
        X_Scale_Panel.text = Default_Scale.x.ToString();
        Y_Scale_Panel.text = Default_Scale.y.ToString();
        Z_Scale_Panel.text = Default_Scale.z.ToString();
        // Match the default Position value to the variables
        X_position = 0f;
        Y_position = 0f;
        Z_position = 0f;
        Added_Position = Vector3.zero;
        X_Position_Panel.text = SpecifiedObject.transform.position.x.ToString();
        Y_Position_Panel.text = SpecifiedObject.transform.position.y.ToString();
        Z_Position_Panel.text = SpecifiedObject.transform.position.z.ToString();
    }


    void Update()
    {
        //Control for The Selected_Object is null or not
        if (SpecifiedObject != null)
        {
            Object_activity = true;
        }
        else
        {
            Object_activity = false;
            X_position = 0f;
            Y_position = 0f;
            Z_position = 0f;
            Added_Position = Vector3.zero;
        }
        if (Object_activity)
        {
            //Make The process by its type
            if (isRotating)
            {
                ElementAxis(_Axis);
                ElementProcess("Rotation");
            }
            if (isScaling)
            {
                ElementAxis(_Axis);
                ElementProcess("Scale");
            }
            if (isPositioning)
            {
                ElementAxis(_Axis);
                ElementProcess("Position");
            }
            
            //ROTATE THE SPECIFIED GAMEOBJECT BY USING KEYBOARD KEYS
            if ((Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.RightArrow)) || (Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.LeftArrow)))
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    Axis_Transform("-X", "Rotation");
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Axis_Transform("+X", "Rotation");
                }

            }
            if ((Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.RightArrow)) || (Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.LeftArrow)))
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    Axis_Transform("-Y", "Rotation");
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Axis_Transform("+Y", "Rotation");
                }
            }
            if ((Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.RightArrow)) || (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.LeftArrow)))
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    Axis_Transform("-Z", "Rotation");
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Axis_Transform("+Z", "Rotation");
                }
            }
            //LOCATE THE SPECIFIED GAMEOBJECT BY USING KEYBOARD KEYS
            if ((Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.UpArrow)) || (Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.DownArrow)))
            {
                
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    Debug.Log("Deneme");
                    Axis_Transform("-X", "Position");
                }
                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    Debug.Log("Karama");
                    Axis_Transform("+X", "Position");
                }

            }
            if ((Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.UpArrow)) || (Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.DownArrow)))
            {
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    Axis_Transform("-Y", "Position");
                }
                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    Axis_Transform("+Y", "Position");
                }
            }
            if ((Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.UpArrow)) || (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.DownArrow)))
            {
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    Axis_Transform("-Z", "Position");
                }
                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    Axis_Transform("+Z", "Position");
                }
            }
        }

    }
    public void ElementAxis(string Axis)
    {
        _Axis = Axis;

    }
    public void ElementProcess(string Transform_Process)
    {
        if (Transform_Process == "Rotation")
        {
            isRotating = true;
        }
        else if (Transform_Process == "Scale")
        {
            isScaling = true;
        }
        else if (Transform_Process == "Position")
        {

            isPositioning = true;
        }
        Axis_Transform(_Axis, Transform_Process);
    }

    public void Axis_Transform(string Axis, string Transform_Process)
    {

        //ALL THE TRANSFORM PROCESSES 
        if (Transform_Process == "Rotation")
        {
            //ROTATE THE SPECIFIED GAMEOBJECT BY USING PANEL KEYS
            rotation = SpecifiedObject.transform.rotation;

            switch (Axis)
            {
                case "+X":
                    rotation *= Quaternion.Euler(3, 0, 0);
                    break;
                case "-X":
                    rotation *= Quaternion.Euler(-3, 0, 0);
                    break;
                case "+Y":
                    rotation *= Quaternion.Euler(0, 3, 0);
                    break;
                case "-Y":
                    rotation *= Quaternion.Euler(0, -3, 0);
                    break;
                case "+Z":
                    rotation *= Quaternion.Euler(0, 0, 3);
                    break;
                case "-Z":
                    rotation *= Quaternion.Euler(0, 0, -3);
                    break;
            }
            SpecifiedObject.transform.rotation = rotation;
            Build.Alterred_Rotation = rotation;
            X_Text_Rotation.text = SpecifiedObject.transform.rotation.eulerAngles.x.ToString();
            Y_Text_Rotation.text = SpecifiedObject.transform.rotation.eulerAngles.y.ToString();
            Z_Text_Rotation.text = SpecifiedObject.transform.rotation.eulerAngles.z.ToString();
        }
        else if (Transform_Process == "Scale")
        {
            //SCALE THE SPECIFIED GAMEOBJECT BY USING PANEL KEYS
            switch (Axis)
            {
                case "+X":
                    X_scale += 0.1f;
                    X_scale = ScaleControl(X_scale, Default_Scale.x, X_Scale_Panel);
                    SpecifiedObject.transform.localScale = new Vector3(X_scale, SpecifiedObject.transform.localScale.y, SpecifiedObject.transform.localScale.z);
                    break;
                case "-X":
                    X_scale -= 0.1f;
                    X_scale = ScaleControl(X_scale, Default_Scale.x, X_Scale_Panel);
                    SpecifiedObject.transform.localScale = new Vector3(X_scale, SpecifiedObject.transform.localScale.y, SpecifiedObject.transform.localScale.z);
                    break;
                case "+Y":
                    Y_scale += 0.1f;
                    Y_scale = ScaleControl(Y_scale, Default_Scale.y, Y_Scale_Panel);
                    SpecifiedObject.transform.localScale = new Vector3(SpecifiedObject.transform.localScale.x, Y_scale, SpecifiedObject.transform.localScale.z);
                    break;
                case "-Y":
                    Y_scale -= 0.1f;
                    Y_scale = ScaleControl(Y_scale, Default_Scale.y, Y_Scale_Panel);
                    SpecifiedObject.transform.localScale = new Vector3(SpecifiedObject.transform.localScale.x, Y_scale, SpecifiedObject.transform.localScale.z);
                    break;
                case "+Z":
                    Z_scale += 0.1f;
                    Z_scale = ScaleControl(Z_scale, Default_Scale.z, Z_Scale_Panel);
                    SpecifiedObject.transform.localScale = new Vector3(SpecifiedObject.transform.localScale.x,SpecifiedObject.transform.localScale.y, Z_scale);
                    break;
                case "-Z":
                    Z_scale -= 0.1f;
                    Z_scale = ScaleControl(Z_scale, Default_Scale.z, Z_Scale_Panel);
                    SpecifiedObject.transform.localScale = new Vector3(SpecifiedObject.transform.localScale.x,SpecifiedObject.transform.localScale.y, Z_scale);
                    break;

            }

            X_Scale_Panel.text = SpecifiedObject.transform.localScale.x.ToString();
            Y_Scale_Panel.text = SpecifiedObject.transform.localScale.y.ToString();
            Z_Scale_Panel.text = SpecifiedObject.transform.localScale.z.ToString();
            //Save the new value of the bottomest and center point of the Selected Object
            Build.Calculate_Abs_Points();
        }
        else if (Transform_Process == "Position")
        {
            switch (Axis)
            {
                case "+X":
                    X_position += 0.1f;
                    Added_Position.x = X_position;
                    StartCoroutine(Limit_Naturalize("+X"));
                    break;
                case "-X":

                    X_position -= 0.1f;
                    Added_Position.x = X_position;
                  StartCoroutine(Limit_Naturalize("-X"));
                    break;
                case "+Y":
                    Y_position += 0.1f;
                    if (Y_position > 10f)
                    {
                        Y_position = 10f;
                    }
                    Added_Position.y = Y_position;
                    break;
                case "-Y":
                    RaycastHit Collide_Hit;
                    if (Physics.Raycast(SpecifiedObject.transform.position, Vector3.down, out Collide_Hit, 0.1f))
                    {
                        if (Collide_Hit.collider.transform.tag.Contains("Plane"))
                        {
                            Debug.Log("DENEME");
                            Y_position += 0f;
                        }
                        else
                        {
                            Y_position -= 0.1f;
                        }
                    }
                    else
                    {
                        Y_position -= 0.1f;
                    }
                    Added_Position.y = Y_position;
                    break;
                case "+Z":
                    Z_position += 0.1f;
                    Added_Position.z = Z_position;
                    StartCoroutine(Limit_Naturalize("+Z"));
                    break;
                case "-Z":
                    Z_position -= 0.1f;
                    Added_Position.z = Z_position;
                    StartCoroutine(Limit_Naturalize("-Z"));
                    break;
            }
            Build.Alterred_Position = Added_Position;
            X_Position_Panel.text = SpecifiedObject.transform.position.x.ToString();
            Y_Position_Panel.text = SpecifiedObject.transform.position.y.ToString();
            Z_Position_Panel.text = SpecifiedObject.transform.position.z.ToString();
        }
       
    }
    //Limit Control Coroutine
    IEnumerator Limit_Naturalize(string Axis)
    {
        yield return new WaitForSeconds(0.1f);

        if (Build.Position_Limit_Control)
        {
            if(Axis == "+X")
            {
                X_position -= 0.1f;
                Added_Position.x = X_position;
            }
            else if(Axis == "-X")
            {
                X_position += 0.1f;
                Added_Position.x = X_position;
            }
            else if (Axis == "+Z")
            {
                Z_position -= 0.1f;
                Added_Position.z = Z_position;
            }
            else if (Axis == "-Z")
            {
                Z_position += 0.1f;
                Added_Position.z = Z_position;
            }

        }
    }
    //Scale Processes
    public float ScaleControl(float Axis_Scale, float Default_Axis_Scale, TMP_InputField Axis_Panel)
    {
        if (Mathf.Abs(Axis_Scale) >= Mathf.Abs(Default_Axis_Scale * 3))
        {
            if (Axis_Scale > 0)
            {
                Axis_Scale = Default_Axis_Scale * 3;
            }
            else
            {
                Axis_Scale = Default_Axis_Scale * -3;
            }
            Axis_Panel.text = Z_scale.ToString();
        }
        return Axis_Scale;

    }
    public void TransformFinish()
    {

        isRotating = false;
        isScaling = false;
        isPositioning = false;
    }
    public void RotateObject(string Axis)
    {
        if (Object_activity)
        {
            //ROTATE THE GAMEOBJECT BY USÝNG INPUT FIELD VALUE
            switch (Axis)
            {
                case "X":
                    float X_value = SystemSettings.ParseValue(X_Text_Rotation.text);
                    rotation = Quaternion.Euler(X_value, 0, 0);
                    SpecifiedObject.transform.rotation = rotation;
                    Build.Alterred_Rotation = rotation;
                    break;
                case "Y":
                    float Y_value = SystemSettings.ParseValue(Y_Text_Rotation.text);
                    rotation = Quaternion.Euler(0, Y_value, 0);
                    SpecifiedObject.transform.rotation = rotation;
                    Build.Alterred_Rotation = rotation;
                    break;
                case "Z":
                    float Z_value = SystemSettings.ParseValue(Z_Text_Rotation.text);
                    rotation = Quaternion.Euler(0, 0, Z_value);
                    SpecifiedObject.transform.rotation = rotation;
                    Build.Alterred_Rotation = rotation;
                    break;
            }
            //Save the new value of the bottomest and center point of the Selected Object
        }
    }
    public void ScaleObject(string Axis)
    {
        if (Object_activity)
        {
            //SCALE THE GAMEOBJECT BY USÝNG INPUT FIELD VALUE
            switch (Axis)
            {
                case "X":
                    if (string.IsNullOrEmpty(X_Scale_Panel.text)) X_Scale_Panel.text = "0";
                    X_scale = float.Parse(X_Scale_Panel.text);
                    X_scale = ScaleControl(X_scale, Default_Scale.x, X_Scale_Panel);
                    if (X_scale != 0)
                    {
                        SpecifiedObject.transform.localScale = new Vector3(X_scale, SpecifiedObject.transform.localScale.y, SpecifiedObject.transform.localScale.z);
                    }
                    else
                    {
                        SpecifiedObject.transform.localScale = new Vector3(SpecifiedObject.transform.localScale.x, SpecifiedObject.transform.localScale.y, SpecifiedObject.transform.localScale.z);
                    }
                    X_Scale_Panel.text = X_scale.ToString();
                    break;
                case "Y":
                    if (string.IsNullOrEmpty(Y_Scale_Panel.text)) X_Scale_Panel.text = "0";
                    Y_scale = float.Parse(Y_Scale_Panel.text);
                    Y_scale = ScaleControl(Y_scale, Default_Scale.y, Y_Scale_Panel);
                    if (Y_scale != 0)
                    {
                        SpecifiedObject.transform.localScale = new Vector3(SpecifiedObject.transform.localScale.x, Y_scale, SpecifiedObject.transform.localScale.z);
                    }
                    else
                    {
                        SpecifiedObject.transform.localScale = new Vector3(SpecifiedObject.transform.localScale.x, SpecifiedObject.transform.localScale.y, SpecifiedObject.transform.localScale.z);
                    }
                    Y_Scale_Panel.text = Y_scale.ToString();
                    break;
                case "Z":
                    if (string.IsNullOrEmpty(Z_Scale_Panel.text)) X_Scale_Panel.text = "0";
                    Z_scale = float.Parse(Z_Scale_Panel.text);
                    Z_scale = ScaleControl(Z_scale, Default_Scale.z, Z_Scale_Panel);
                    if (Z_scale != 0)
                    {
                        SpecifiedObject.transform.localScale = new Vector3(SpecifiedObject.transform.localScale.x, SpecifiedObject.transform.localScale.y, Z_scale);
                    }
                    else
                    {
                        SpecifiedObject.transform.localScale = new Vector3(SpecifiedObject.transform.localScale.x, SpecifiedObject.transform.localScale.y, SpecifiedObject.transform.localScale.z);
                    }
                    Z_Scale_Panel.text = Z_scale.ToString();
                    break;
            }
            //Save the new value of the bottomest and center point of the Selected Object
            Build.Calculate_Abs_Points();
        }


    }
 
}
