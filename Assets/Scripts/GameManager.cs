
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Book book;

    public GameObject playersOverlay;
    public InputField playersInput;
    public Button playersDoneButton;



	private void Start ()
    {
        book = Book.instance;
        
        ExhibitBase.instance.AddCornerCode("33333", WriteMode);
        //ExhibitBase.instance.SetCornerCodeButtonColor(Color.red);

        playersOverlay.SetActive(true);
        playersDoneButton.interactable = false;
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call<bool>("moveTaskToBack", true);
            }
            else
            {
                Application.Quit();
            }
        }
    }


    private void WriteMode()
    {

    }


    public void PlayerAdded()
    {
        book.AddPlayer(playersInput.text);
        playersInput.text = "";
        playersDoneButton.interactable = true;
    }

    public void PlayerAddingDone()
    {
        playersOverlay.SetActive(false);

        // load the book
        TextAsset file = Resources.Load("ADayWithDinosaurs") as TextAsset;
        string bookJsonString = file.ToString();
        book.Load(bookJsonString);
    }




    // singleton
    public static GameManager instance;
    private void Awake()
    {
        if (instance && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

}
