using UnityEngine;

using UnityEngine.EventSystems;

using UnityEngine.UI;



public class MenuController : MonoBehaviour

{

    [Header("UI Focus")]

    public GameObject firstSelectedButton;



    void OnEnable()

    {


        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(firstSelectedButton);

    }

}