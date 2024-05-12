using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyPad : MonoBehaviour
{
    public GameObject KeyPad_Panel;
    
    public GameObject[] KeyPad_buttons;
   private  GameObject Last_Selected_UI_Element;
    public TextMeshProUGUI Result_value;
    private TMP_InputField Focussed_Input_Field;
    private bool Keypad_openned;
    void Start()
    {
        KeyPad_Panel.SetActive(false);
        InvokeRepeating("RepeatedMethod", 1.0f, 0.1f); 
    }
     // In order to optimizate to the app.
    private void RepeatedMethod()
    {
        if(!KeyPad_buttons.Contains(EventSystem.current.currentSelectedGameObject) && Keypad_openned)
        {
            Last_Selected_UI_Element = EventSystem.current.currentSelectedGameObject;
            if (Last_Selected_UI_Element != null && Last_Selected_UI_Element.GetComponent<TMP_InputField>())
            {
                string Allowed_chars = "1234567890,-";
                Result_value.text = new string(Last_Selected_UI_Element.GetComponent<TMP_InputField>().text.Where(c => Allowed_chars.Contains(c)).ToArray());

            }
                
        }
    }
    public void open_Keypad_Panel()
    {
        KeyPad_Panel.SetActive(true);
        Keypad_openned = true;
        if ( Last_Selected_UI_Element != null && Last_Selected_UI_Element.GetComponent<TMP_InputField>() )
            Result_value.text = Last_Selected_UI_Element.GetComponent<TMP_InputField>().text;
       
    }
    public void Close_Keypad_Panel()
    {
        KeyPad_Panel.SetActive(false);
        Keypad_openned = false;
    }
    public void Input_Number(int Number)
    {
        Type_The_Value(Number.ToString(),false);

    }
    public void Input_Decimal(string Keycode)
    {
        if (Keycode == ",") Type_The_Value(",",true);
        else if(Keycode == "-") Type_The_Value("-", true);
        else RemovelastChar();
    }
    TMP_InputField Focused_Input_Field_Control()
    {

        if (Last_Selected_UI_Element != null && Last_Selected_UI_Element.GetComponent<TMP_InputField>())
        {
            return Last_Selected_UI_Element.GetComponent<TMP_InputField>();

        }
        else return null;

    }
    void Type_The_Value(string value,bool IsItSembol)
    {
        
        if(!IsItSembol)
        Result_value.text += value;
        else
        {
            if (value == "-" && !Result_value.text.Contains("-"))
            {
                Result_value.text = "-" + Result_value.text;
                KeyPad_buttons[12].GetComponent<Button>().interactable = false;
            }
            
            if (value == "," && !Result_value.text.Contains(","))
            {
                Result_value.text += value;
                KeyPad_buttons[9].GetComponent<Button>().interactable = false;
            }
            if (Result_value.text[0] == ',') Result_value.text = "0" + Result_value.text;

            if (Result_value.text.Contains("-,"))
            {
                Result_value.text= Result_value.text.Replace("-,", "");
                Result_value.text = "-0," + Result_value.text;
            }
        }
        //if (value == "-")
        //{

        //    Current_String = "-" + Result_value.text;
        //    Result_value.text = Current_String;
        //}
        //else Result_value.text += value;
        TypeInput();
    }
    void RemovelastChar()
    {
        if (Result_value.text.Length >= 1) Result_value.text = Result_value.text.Remove(Result_value.text.Length - 1);
        if(!Result_value.text.Contains("-")) KeyPad_buttons[12].GetComponent<Button>().interactable = true;
        if (!Result_value.text.Contains(",")) KeyPad_buttons[9].GetComponent<Button>().interactable = true;
        TypeInput();
    }
    void TypeInput()
    {
        Focussed_Input_Field = Focused_Input_Field_Control();
        EventSystem.current.SetSelectedGameObject(Last_Selected_UI_Element);

        if (Focussed_Input_Field != null) Focussed_Input_Field.text = Result_value.text;
 
    }
}
