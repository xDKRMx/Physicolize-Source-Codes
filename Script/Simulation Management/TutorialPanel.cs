using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [Header("Tutorial Menu ")]
    public GameObject Main_Tutorial_Panel;
    public GameObject Tutorial_Menu;
    [Header("Mini Tutorial")]
    public GameObject Mini_Tutorial_Panel;
    public List<GameObject> tutorial_Parts = new List<GameObject>();
    public int tutorial_part_index;
    

    public void Opening_Tutorial()
    {
        Main_Tutorial_Panel.SetActive(true);
        Tutorial_Menu.SetActive(true);
        Mini_Tutorial_Panel.SetActive(false);
        //Menu_Animaton.Play();
    }

    public void OpenMiniTutorial()
    {
        Mini_Tutorial_Panel.SetActive(true);
        Tutorial_Menu.SetActive(false);
        tutorial_part_index = 0;
        tutorial_Parts[tutorial_part_index].SetActive(true);
    }
    public void Next_Panel()
    {
        if (tutorial_part_index == tutorial_Parts.Count - 1)
        {
            Mini_Tutorial_Panel.SetActive(false);
            tutorial_part_index = 0;
        }
        else
        {
            tutorial_part_index++;
            tutorial_Parts[tutorial_part_index -1].SetActive(false);
            tutorial_Parts[tutorial_part_index].SetActive(true);
        }
        
    }
    public void Close_Tutorial_Panel()
    {
        Main_Tutorial_Panel.SetActive(false);
        tutorial_part_index = 0;
    }
      

}
