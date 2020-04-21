using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class deals with the tutorial UI for the game
//The amount of text should be 3 times the amount of pages. Each page supports 3 lines of text
public class TutorialController : MonoBehaviour
{
    int pos = 0;
    public int noOfScreens;
    public List<Sprite> tutSprites;
    public List<string> tutTexts;
    Image image;
    Text captionLine1;
    Text captionLine2;
    Text captionLine3;
    GameObject nextButton;
    GameObject prevButton;
    GameObject closeLeft;
    GameObject closeRight;


    // Start is called before the first frame update
    void Start()
    {
        //Set the handles for the parts of the UI
        image = gameObject.transform.Find("Image").GetComponent<Image>();
        captionLine1 = gameObject.transform.Find("CaptionLine1").GetComponent<Text>();
        captionLine2 = gameObject.transform.Find("CaptionLine2").GetComponent<Text>();
        captionLine3 = gameObject.transform.Find("CaptionLine3").GetComponent<Text>();
        nextButton = gameObject.transform.Find("NextButton").gameObject;
        prevButton = gameObject.transform.Find("PrevButton").gameObject;
        closeLeft = gameObject.transform.Find("CloseLeft").gameObject;
        closeRight = gameObject.transform.Find("CloseRight").gameObject;
        resetUI();
    }

    void resetUI() 
    {
        //Set the initial page of the tutorial
        pos = 0;
        image.sprite = tutSprites[0];
        captionLine1.text = tutTexts[0];
        captionLine2.text = tutTexts[1];
        captionLine3.text = tutTexts[2];
        nextButton.SetActive(true);
        closeLeft.SetActive(true);
        prevButton.SetActive(false);
        closeRight.SetActive(false);
    }

    //The next page button has been clicked
    public void next() 
    {
        closeLeft.SetActive(false);
        pos += 1;
        prevButton.SetActive(true);
        if (pos >= noOfScreens - 1) 
        {
            nextButton.SetActive(false);
            closeRight.SetActive(true);
        }
        updateTutorial();
    }

    //The previous page button has been clicked
    public void prev() 
    {
        closeRight.SetActive(false);
        pos -= 1;
        nextButton.SetActive(true);
        if (pos <= 0) 
        {
            prevButton.SetActive(false);
            closeLeft.SetActive(true);
        }
        updateTutorial();
    }

    //Update the information on the screen
    void updateTutorial()
    {
        image.sprite = tutSprites[pos];
        captionLine1.text = tutTexts[3*pos];
        captionLine2.text = tutTexts[3*pos + 1];
        captionLine3.text = tutTexts[3*pos + 2];
    }

    public void close() 
    {
        resetUI();
        FindObjectOfType<MainMenu>().closeTutorial();
    }
}
