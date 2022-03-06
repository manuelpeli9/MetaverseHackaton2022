using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class NFTCall : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button showDocument;
    [SerializeField] private Transform waitingCanvas;
    private Sprite mySprite;
    // [SerializeField] private Texture2D myTexture2D;
    // Start is called before the first frame update
    public void ShowDocument()
    {
        StartCoroutine(getRequest("https://metaverse-rest-api.herokuapp.com/getDocuments/0"));
        showDocument.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
    }
    IEnumerator getRequest(string uri)
    {
        waitingCanvas.gameObject.SetActive(true);
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();
        var image_uri = "";
        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            image_uri = uwr.downloadHandler.text.Substring(2, uwr.downloadHandler.text.Length - 4);
            Debug.Log("Image uri: " + image_uri);
        }
        UnityWebRequest uwr2 = UnityWebRequestTexture.GetTexture(image_uri);
        yield return uwr2.SendWebRequest();
        if (uwr2.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr2.error);
        }
        else
        {
            var myTexture = ((DownloadHandlerTexture)uwr2.downloadHandler).texture;
            mySprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            image.sprite = mySprite;
            image.gameObject.SetActive(true);
            waitingCanvas.gameObject.SetActive(false);
        }

    }
}