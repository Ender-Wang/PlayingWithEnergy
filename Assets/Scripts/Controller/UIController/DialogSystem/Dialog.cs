using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class Dialog : MonoBehaviour
{
    public static Dialog Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            characterImages = new Dictionary<string, Sprite>();
            Sprite[] sprites = Resources.LoadAll<Sprite>("Achievement Resources/Small Tasks/NPCs/Profile Images");
            foreach (Sprite sprite in sprites)
            {
                characterImages.Add(sprite.name, sprite);
            }
        }
        else
            Destroy(gameObject);
    }

    [Header("UI")]
    public TextMeshProUGUI dialogText;
    public Image characterImage;

    public bool dialogFinished { get; set; } // dialog finishes


    bool textFinished = false;  //text finished typing
    bool isTyping;  //text is typing
    bool begin = false; // dialog begins

    List<string> textList = new List<string>();
    int index = 0;
    GameObject character;
    Dictionary<string, Sprite> characterImages;
    StringBuilder sentence = new StringBuilder(1024); // one sentence no more than 1024 characters

    // Update is called once per frame
    void Update()
    {
        if (begin)
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonUp(0)) && index >= textList.Count)
            { // if the dialog finishes
                SelectingController.Instance.enabled = true;
                dialogFinished = true;
                Destroy(gameObject);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonUp(0))
            {
                if (textFinished) // if the text finishes typing
                    StartCoroutine(SetTextUI());
                else // if the text is typing -> skip typing
                    isTyping = false;
            }
        }
    }

    public void SetDialogUI(List<string> textList, GameObject character)
    {
        SelectingController.Instance.enabled = false; // disable the selecting controller
        this.textList = textList;
        textFinished = true;
        index = 0;
        begin = true;
        isTyping = true;
        dialogFinished = false;
        this.character = character;
        StartCoroutine(SetTextUI());
    }

    IEnumerator SetTextUI()
    {
        textFinished = false;
        dialogText.text = "";
        string characterName = textList[index].Split(':')[0].Trim();
        switch (characterName) // set character name
        {
            case "Player":
                characterName = "Player";
                break;
            case "NPC":
                characterName = character.name.Replace("[", "").Replace("]", "");
                break;
            default:
                characterName = "Player";
                break;
        }

        if (characterImage)
        {
            Sprite sprite = characterImages[characterName];
            if (sprite != null)
                characterImage.sprite = sprite;
            else
                characterImage.gameObject.SetActive(false);
        }

        int word = 0;
        if (textList[index] != "")
        {
            string t = characterName + " : " + textList[index].Split(':')[1].Trim(); // reset the name of the character
            while (isTyping && word < t.Length - 1)
            {
                sentence.Append(t[word]);
                dialogText.text = sentence.ToString();
                word++;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            dialogText.text = t;
            sentence.Clear();
        }

        textFinished = true;
        index++;
        isTyping = true;
    }
}
