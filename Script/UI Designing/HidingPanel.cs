using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPanel : MonoBehaviour
{
    public GameObject[] Temporary_Panel;
    int index_Number;
    public void HidePanel(GameObject Panel)
    {
        Debug.Log(Panel);
        Panel.SetActive(false);
       
        Temporary_Panel[index_Number].SetActive(true);
    }
    public void PanelIndex(int Panel_index)
    {
        index_Number = Panel_index;

    }
    public void OpenPanel(GameObject Panel)
    {
        Panel.SetActive(true);
        Temporary_Panel[index_Number].SetActive(false);
    }
    public void ClosePanel(GameObject Panel)
    {
        Destroy(Panel);
    }
}
