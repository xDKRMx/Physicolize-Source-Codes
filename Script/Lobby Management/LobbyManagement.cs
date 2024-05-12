using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManagement : MonoBehaviour
{
    public Animator Geart_Animator;
    public Button Simulation_Button;
    private void Start()
    {
        Geart_Animator.enabled = false;
    }

    //In this function if the mouse cursor is on the Gear button. The gear animation will begin.
    public void OnPointerEnter()
    {
        Geart_Animator.enabled = true;
        Geart_Animator.Play("Turninggear"); 
    }

    public void OnPointerExit()
    {
        Geart_Animator.StopPlayback();
        Geart_Animator.enabled = false;
    }

    public void Load_Simulation()
    {
        SceneManager.LoadScene(1);
    }
    public void Exit_App()
    {
        Application.Quit();
    }
}
