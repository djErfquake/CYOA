using System.Collections;
using System.Collections.Generic;
using NiceJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Book : MonoBehaviour
{
    private const char VARIABLE_START = '<';
    private const char VARIABLE_END = '>';

    public TextMeshProUGUI bookTitle, pageText;
    public Transform optionButtonParent;
    public GameObject optionButtonPrefab;

    private Dictionary<string, Page> pages = new Dictionary<string, Page>();
    private Page currentPage = null;
    private Page startingPage = null;


    private Dictionary<string, string> variableQuestions = new Dictionary<string, string>();
    private Dictionary<string, string> variables = new Dictionary<string, string>();
    private string currentVariableName;
    public GameObject questionOverlay;
    public TextMeshProUGUI questionText;
    public InputField questionInputField;


    private const string PLAYER_VARIABLE = "player";
    private const string LAST_PLAYER_VARIABLE = "last-player";
    private List<string> playerNames = new List<string>();
    private string lastPlayerName = "";




    public void Load(string bookJsonString)
    {
        JsonNode bookConfig = JsonNode.ParseJsonString(bookJsonString);
        bookTitle.text = bookConfig["title"];

        // load variable questions in
        JsonArray variableConfig = bookConfig["variables"] as JsonArray;
        foreach (JsonNode variableInfo in variableConfig)
        {
            variableQuestions.Add(variableInfo["name"], variableInfo["question"]);
        }
        


        // load pages in
        JsonArray pagesConfig = bookConfig["pages"] as JsonArray;
        foreach (JsonNode pageConfig in pagesConfig)
        {
            Page page = new Page(pageConfig);
            pages.Add(page.pageName, page);

            if (currentPage == null)
            {
                currentPage = page;
            }
        }

        // populate with first page
        SetupPage(currentPage);
        startingPage = currentPage;
    }


    private void SetupPage(Page page)
    {
        if (!FindAndAskQuestions(page.pageText))
        {
            PopulatePage(page);
        }
    }

    private void PopulatePage(Page page)
    {
        pageText.text = GetTextWithVariables(page);

        // destroy all old options
        foreach (Transform child in optionButtonParent)
        {
            Destroy(child.gameObject);
        }

        if (page.options.Count == 0)
        {
            pageText.text += "\n\n\t\tThe End.";
            GameObject startOverButton = Instantiate(optionButtonPrefab);
            startOverButton.transform.SetParent(optionButtonParent);
            startOverButton.GetComponentInChildren<Text>().text = "Start Over?";
            startOverButton.GetComponent<Button>().onClick.AddListener(() => RestartBook());
        }
        else
        {
            // add all new options
            foreach (KeyValuePair<string, string> option in page.options)
            {
                GameObject optionButton = Instantiate(optionButtonPrefab);
                optionButton.transform.SetParent(optionButtonParent);
                optionButton.GetComponentInChildren<Text>().text = option.Key;
                optionButton.GetComponent<Button>().onClick.AddListener(() => SetupPage(pages[option.Value]));
            }
        }
    }



    private bool FindAndAskQuestions(string searchPageText)
    {
        string thisPageText = searchPageText;
        char[] charPageText = thisPageText.ToCharArray();

        bool startFound = false;
        string variableName = "";

        List<string> variableNames = new List<string>();

        for (int i = 0; i < charPageText.Length; i++)
        {
            if (charPageText[i] == VARIABLE_START)
            {
                startFound = true;
            }
            else if (charPageText[i] == VARIABLE_END)
            {
                variableNames.Add(variableName);
                startFound = false;
                variableName = "";
            }
            else if (startFound)
            {
                variableName += charPageText[i];
            }
        }

        // ask question if needed
        foreach (string foundVariable in variableNames)
        {
            if (foundVariable != PLAYER_VARIABLE && foundVariable != LAST_PLAYER_VARIABLE && !variables.ContainsKey(foundVariable))
            {
                questionOverlay.SetActive(true);
                questionText.text = variableQuestions[foundVariable];
                currentVariableName = foundVariable;
                return true;
            }
        }

        return false;
    }


    public void QuestionAnswered()
    {
        if (questionInputField.text != "")
        {
            variables[currentVariableName] = questionInputField.text;
            questionInputField.text = "";
            questionOverlay.SetActive(false);
            SetupPage(currentPage);            
        }
    }

    private string GetTextWithVariables(Page page)
    {
        string pageText = page.pageText;
        char[] charPageText = pageText.ToCharArray();

        bool replacingWord = false;
        string variableName = "";

        pageText = "";
        for (int i = 0; i < charPageText.Length; i++)
        {
            if (charPageText[i] == VARIABLE_START)
            {
                replacingWord = true;
            }
            else if (charPageText[i] == VARIABLE_END)
            {
                if (variableName == PLAYER_VARIABLE)
                {
                    lastPlayerName = playerNames[Random.Range(0, playerNames.Count)];
                    pageText += lastPlayerName;
                }
                else if (variableName == LAST_PLAYER_VARIABLE)
                {
                    pageText += lastPlayerName;
                }
                else
                {
                    pageText += variables[variableName];
                }

                variableName = "";
                replacingWord = false;
            }
            else if (replacingWord)
            {
                variableName += charPageText[i];
            }
            else
            {
                pageText += charPageText[i];
            }
        }

        return pageText;
    }


    public void RestartBook()
    {
        variables.Clear();
        SetupPage(startingPage);
    }



    public void AddPlayer(string playerName)
    {
        playerNames.Add(playerName);
    }





    // singleton
    public static Book instance;
    private void Awake()
    {
        if (instance && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }


}
