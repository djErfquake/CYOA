using NiceJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page
{
    public string pageName;
    public string pageText;

    public Dictionary<string, string> options;


    public Page(JsonNode pageConfig)
    {
        pageName = pageConfig["page-name"];
        pageText = pageConfig["page-text"];

        options = new Dictionary<string, string>();
        
        if (pageConfig.ContainsKey("options"))
        {
            JsonArray optionsConfig = pageConfig["options"] as JsonArray;
            foreach (JsonNode optionConfig in optionsConfig)
            {
                string optionText = optionConfig["text"];
                string optionGoToPageName = optionConfig["go-to-page"];
                options.Add(optionText, optionGoToPageName);
            }
        }


    }


}
