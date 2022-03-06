using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum UserType
{
    Owner,
    AuthorizedBuyer,
    UnauthorizedBuyer,
} 

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private Transform initScene;
    [SerializeField] private Transform houseScene;
    [SerializeField] private Transform userChoiceCanvas;
    [SerializeField] private Transform houseSelectionCanvas;
    [SerializeField] private Transform errorPopup;
    private int userType;

    // Start is called before the first frame update
    void Start()
    {
        initScene.gameObject.SetActive(true);
        houseScene.gameObject.SetActive(false);
        userChoiceCanvas.gameObject.SetActive(true);
        houseSelectionCanvas.gameObject.SetActive(false);
        errorPopup.gameObject.SetActive(false);
        //DontDestroyOnLoad(gameObject);
    }

    public void UserChoice(int userType)
    {
        this.userType = userType;
        userChoiceCanvas.gameObject.SetActive(false);
        houseSelectionCanvas.gameObject.SetActive(true);
    }

    public void OnHouseSelected(int houseName)
    {
        if (userType == 0 || userType == 1)
        {
            //SceneManager.LoadScene("Complete");
            houseScene.gameObject.SetActive(true);
            initScene.gameObject.SetActive(false);
        }
        else
        {
            errorPopup.gameObject.SetActive(true);
        }
    }

    public void Back()
    {
        userChoiceCanvas.gameObject.SetActive(true);
        houseSelectionCanvas.gameObject.SetActive(false);
        errorPopup.gameObject.SetActive(false);
    }
}
