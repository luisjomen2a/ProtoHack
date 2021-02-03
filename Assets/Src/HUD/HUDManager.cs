using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    Text m_promptText;
    // Start is called before the first frame update
    void Start()
    {
        m_promptText = GameObject.Find("PromptText").GetComponent<Text>();

        // clear the prompt.
        m_promptText.text = "";
    }

    void Update()
    {
        if (Input.anyKey)
        {
            m_promptText.text = "";
        }
    }

    // Updates the prompt with the given message
    public void prompt(string message)
    {
        m_promptText.text = message;
    }
}
