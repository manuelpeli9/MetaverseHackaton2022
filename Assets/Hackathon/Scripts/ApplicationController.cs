using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private Transform homeMenuCanvas;
    [SerializeField] private Transform houseSelectionCanvas;
    [SerializeField] private Transform errorPopup;
    [SerializeField] private Image documentImage;
    [SerializeField] private Button showDocument;
    [Header("Buyer")]
    [SerializeField] private Transform buyerCanvas;
    [SerializeField] private Button startProposal;
    [Header("Owner")]
    [SerializeField] private Transform ownerCanvas;
    [SerializeField] private Button acceptProposal;
    [SerializeField] private Button denyProposal;
    [Header("Request")]
    [SerializeField] private Transform waitCanvas;
    [SerializeField] private Transform resultCanvas;
    [SerializeField] private TMP_Text confirmText;
    private int userType;


    private string showDocumentUri = "https://metaverse-rest-api.herokuapp.com/getDocuments/0";
    private string isAuthorizedUri = "https://metaverse-rest-api.herokuapp.com/isAuthorized/{type}";
    private string startProposalUri = "https://metaverse-rest-api.herokuapp.com/startProposal";
    private string showProposalUri = "https://metaverse-rest-api.herokuapp.com/showProposal";
    private string declineProposalUri = "https://metaverse-rest-api.herokuapp.com/declineProposal";
    private string confirmProposalUri = "https://metaverse-rest-api.herokuapp.com/confirmProposal";


    // Start is called before the first frame update
    void Start()
    {
        initScene.gameObject.SetActive(true);
        houseScene.gameObject.SetActive(false);
        userChoiceCanvas.gameObject.SetActive(true);
        homeMenuCanvas.gameObject.SetActive(false);

        houseSelectionCanvas.gameObject.SetActive(false);
        errorPopup.gameObject.SetActive(false);
        waitCanvas.gameObject.SetActive(false);
        resultCanvas.gameObject.SetActive(false);
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
            homeMenuCanvas.gameObject.SetActive(true);

            if (userType == 0)
            {
                ownerCanvas.gameObject.SetActive(true);
                buyerCanvas.gameObject.SetActive(false);
            }
            else
            {
                ownerCanvas.gameObject.SetActive(false);
                buyerCanvas.gameObject.SetActive(true);
            }
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

    public void StartProposalRequest()
    {
        StartCoroutine(StartRequestCoroutine(startProposalUri, OnStartProposalEnd));
    }

    private void OnStartProposalEnd(string result)
    {
        confirmText.text = "Proposal sent: " + result;
    }

    public void ShowProposalRequest()
    {
        StartCoroutine(StartRequestCoroutine(showProposalUri, OnShowProposalEnd));
    }

    private void OnShowProposalEnd(string result)
    {
        confirmText.text = "Proposal sent: " + result;
    }

    public void AcceptProposalRequest()
    {
        StartCoroutine(StartRequestCoroutine(confirmProposalUri, OnAcceptProposalEnd));
    }

    private void OnAcceptProposalEnd(string result)
    {
        confirmText.text = "Proposal accepted: " + result;
    }

    public void DeclineProposalRequest()
    {
        StartCoroutine(StartRequestCoroutine(declineProposalUri, OnDeclineProposalEnd));
    }

    private void OnDeclineProposalEnd(string result)
    {
        confirmText.text = "Proposal denied: " + result;
    }


    public IEnumerator StartRequestCoroutine(string uri, System.Action<string> onComplete)
    {
        waitCanvas.gameObject.SetActive(true);
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();
        var result = "";
        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            result = uwr.downloadHandler.text;
            Debug.Log("Image uri: " + result);
        }
        waitCanvas.gameObject.SetActive(false);
        resultCanvas.gameObject.SetActive(true);
        onComplete(result);
    }
}
