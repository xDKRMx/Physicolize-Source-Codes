using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    // Start is called before the first frame update
    public float Time_value;
    public float Time_Frame;
    public bool Time_Counting = false;
    public TextMeshProUGUI Time_Panel;
    public Sprite Resume_Icon;
    public Sprite Pause_Icon;
    public GameObject TimeControl_Button;
    void Update()
    {
        if (Time_Counting)
        {
            Time_value += Time.deltaTime; 
            Time_Panel.text = Time_value.ToString("F2") + " s";
        }
    }
    public void TimeControl()
    {
        if(Time_Counting)
        {
            Time_Counting = false;
            TimeControl_Button.GetComponent<Image>().sprite = Pause_Icon;
        }
        else
        {
            Time_Counting = true;
            TimeControl_Button.GetComponent<Image>().sprite = Resume_Icon;
        }
    }
    public void RefreshTime()
    {
        Time_Counting = false;
        Time_Frame = 0;
        Time_value = 0;
        Time_Panel.text = Time_value.ToString("F2") + " s";
        TimeControl_Button.GetComponent<Image>().sprite = Pause_Icon;
    }
}
