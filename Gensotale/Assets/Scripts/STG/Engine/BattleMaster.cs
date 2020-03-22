using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleMaster : MonoBehaviour
{
    public static BattleMaster battleMaster = null;

    public static string[] DEF = new string[] { " DEF", " DEF", " DEF", " DEF" };
    public static string[] ATK = new string[] { " ATK", " ATK", " ATK", " ATK" };
    public static string[] CURRENCY = new string[] { " G", " G", " G", " G" };
    public static string[] EXP = new string[] { " EXP", " EXP", " EXP", " EXP" };
    public static string[] MAXHP = new string[] { " MAX HP", " MAX HP", " MAX HP", " MAX HP" };
    public static string[] HP = new string[] { " HP", " HP", " HP", " HP" };

    public BattlePhase phase;

    public Color buttonInactive;
    public Color buttonNormal;
    public Color buttonHighlight;
    public Color spareColor;
    public EncounterData encounter;
    public TextMeshPro mainTextBoxText;

    TextData.TextLine currentLine;
    TextMeshPro currentTextDisplay;
    TMP_TextInfo currentTextInfo;

    public Transform enemyBulletContainer;

    int languageSetting = 0;

    int turnCount = 0;

    public enum BattlePhase {MainMenu, ActMenu, ActSelect, ItemMenu, STG}
    int buttonIndex;
    InputScript input;
    GameMaster gameMaster;
    bool writingText;
    bool turnStart;
    int textIndex;
    int textTimer;
    int basicsIndex;
    int[] colorIndex;
    float[] colorTime;

    public Button[] mainMenuButtons = new Button[3];

    [System.Serializable]
    public class Button
    {
        public Image buttonOutline;
        public Image icon;
        public TextMeshPro text;
        public Sprite unfocusedIcon;
        public Sprite focusedIcon;
    }
    // Start is called before the first frame update
    void Start()
    {
        battleMaster = this;
        gameMaster = GameMaster.gameMaster;
        input = GameMaster.inputScript;
        languageSetting = (int)gameMaster.languageSetting;
        turnStart = true;
        gameMaster.playerTrans = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (turnStart)
        {
            SelectBattleText();
            turnStart = false;
        }

        switch (phase)
        {
            case BattlePhase.MainMenu:
                ProgressText();
                break;
        }

        ChangeMenuSelection();
    }

    private void LateUpdate()
    {
        if (writingText)
        {
            TextController.TextEffectProcessing(currentLine, currentTextDisplay, currentTextInfo, ref colorTime, ref colorIndex, languageSetting);
            TextController.TextVisibility(textIndex, currentTextDisplay, currentTextInfo);
        }
    }

    void LoadBattle()
    {

    }

    void ProgressText()
    {
        textTimer++;
        while (textIndex < currentTextInfo.characterCount - 1 && textTimer >= currentLine.textBasics[basicsIndex].textDelay)
        {
            do
            {
                do
                    textIndex++;
                while (currentTextInfo.characterInfo[textIndex].character == ' ');

                if (currentLine.textBasics[basicsIndex].endIndex[languageSetting] < textIndex)
                    if (currentLine.textBasics.Count > basicsIndex + 1)
                        basicsIndex++;
            } while (currentLine.textBasics[basicsIndex].textDelay < 0);
            
            if (currentLine.textBasics[basicsIndex].textSound != null)
                AudioSourceExtensions.PlayClip2D(currentLine.textBasics[basicsIndex].textSound);
            textTimer -= currentLine.textBasics[basicsIndex].textDelay;
        }
    }

    void SelectBattleText()
    {
        if(turnCount == 0)
        {
            SetText(mainTextBoxText, encounter.openingLine);
        }
        writingText = true;
    }

    void SetText(TextMeshPro newDisplay, TextData.TextLine newLine)
    {
        if (currentTextDisplay != null)
            currentTextDisplay.text = "";

        currentTextDisplay = newDisplay;
        currentLine = newLine;

        textIndex = currentLine.addStar ? 1 : -1;
        textTimer = 0;
        basicsIndex = 0;
        colorIndex = new int[currentLine.colorList.Count];
        colorTime = new float[currentLine.colorList.Count];

        currentTextDisplay.text = (currentLine.addStar ? "* " : "") + currentLine.translation[languageSetting];
        currentTextInfo = currentTextDisplay.textInfo;
    }

    void SetText(TextMeshPro newDisplay, TextData.TextLine newLine, EnemyData enemy)
    {
        if (currentTextDisplay != null)
            currentTextDisplay.text = "";
        
        currentTextDisplay = newDisplay;
        currentLine = new TextData.TextLine();

        currentLine.addStar = newLine.addStar;
        currentLine.charIndex = newLine.charIndex;
        currentLine.portraitIndex = newLine.portraitIndex;
        currentLine.progressAutomatically = newLine.progressAutomatically;
        currentLine.skipDelay = newLine.skipDelay;

        for (int i = 0; i < newLine.translation.Count; i++)
        {
            currentLine.translation.Add(newLine.translation[i]);
        }
        for (int i = 0; i < newLine.colorList.Count; i++)
        {
            currentLine.colorList.Add(new TextData.ColorEffects());
            currentLine.colorList[i].effectType = newLine.colorList[i].effectType;
            currentLine.colorList[i].time = newLine.colorList[i].time;
            for (int k = 0; k < newLine.colorList[i].colors.Count; k++)
            {
                currentLine.colorList[i].colors.Add(newLine.colorList[i].colors[k]);
            }
            for (int k = 0; k < newLine.colorList[i].endIndex.Count; k++)
            {
                currentLine.colorList[i].endIndex.Add(newLine.colorList[i].endIndex[k]);
            }
            for (int k = 0; k < newLine.colorList[i].startIndex.Count; k++)
            {
                currentLine.colorList[i].startIndex.Add(newLine.colorList[i].startIndex[k]);
            }
        }
        for (int i = 0; i < newLine.effectList.Count; i++)
        {
            currentLine.effectList.Add(new TextData.TextEffects());
            currentLine.effectList[i].effectType = newLine.effectList[i].effectType;
            currentLine.effectList[i].speed = newLine.effectList[i].speed;
            currentLine.effectList[i].distance = newLine.effectList[i].distance;
            for (int k = 0; k < newLine.effectList[i].endIndex.Count; k++)
            {
                currentLine.effectList[i].endIndex.Add(newLine.effectList[i].endIndex[k]);
            }
            for (int k = 0; k < newLine.effectList[i].startIndex.Count; k++)
            {
                currentLine.effectList[i].startIndex.Add(newLine.effectList[i].startIndex[k]);
            }
        }
        for (int i = 0; i < newLine.textBasics.Count; i++)
        {
            currentLine.textBasics.Add(new TextData.TextBasics());
            currentLine.textBasics[i].textDelay = newLine.textBasics[i].textDelay;
            currentLine.textBasics[i].textSound = newLine.textBasics[i].textSound;
            for (int k = 0; k < newLine.textBasics[i].endIndex.Count; k++)
            {
                currentLine.textBasics[i].endIndex.Add(newLine.textBasics[i].endIndex[k]);
            }
            for (int k = 0; k < newLine.textBasics[i].startIndex.Count; k++)
            {
                currentLine.textBasics[i].startIndex.Add(newLine.textBasics[i].startIndex[k]);
            }
        }

        textIndex = currentLine.addStar ? 1 : -1;
        textTimer = 0;
        basicsIndex = 0;
        colorIndex = new int[currentLine.colorList.Count];
        colorTime = new float[currentLine.colorList.Count];

        string parsedString = currentLine.translation[languageSetting];
        int difference = 0;
        string stringChange = "";

        for (int i = 0; i < parsedString.Length - 2; i++)
        {

            if (parsedString[i] == '{')
            {
                stringChange = "";
                difference = 0;
                switch (parsedString.Substring(i, 3))
                {
                    case "{n}":
                        stringChange = enemy.enemyName[languageSetting];
                        difference = stringChange.Length - 3;
                        break;
                    case "{d}":
                        stringChange = enemy.defense.ToString() + DEF[languageSetting];
                        difference = stringChange.Length - 3;
                        break;
                    case "{a}":
                        stringChange = enemy.attack.ToString() + ATK[languageSetting];
                        difference = stringChange.Length - 3;
                        break;
                    case "{m}":
                        stringChange = enemy.maxHp.ToString() + MAXHP[languageSetting];
                        difference = stringChange.Length - 3;
                        break;
                    case "{h}":
                        stringChange = enemy.hp.ToString() + HP[languageSetting];
                        difference = stringChange.Length - 3;
                        break;
                    case "{e}":
                        stringChange = enemy.xpDrop.ToString() + EXP[languageSetting];
                        difference = stringChange.Length - 3;
                        break;
                    case "{c}":
                        stringChange = enemy.attack.ToString() + CURRENCY[languageSetting];
                        difference = stringChange.Length - 3;
                        break;
                }
            }
            else
                continue;

            if (stringChange != "" || difference != 0)
            {
                for (int k = 0; k < currentLine.effectList.Count; k++)
                {
                    for (int n = 0; n < currentLine.textBasics[k].startIndex.Count; n++)
                    {
                        if (currentLine.textBasics[k].startIndex[n] > i)
                            currentLine.textBasics[k].startIndex[n] += difference;
                    }
                    for (int n = 0; n < currentLine.textBasics[k].endIndex.Count; n++)
                    {
                        if (currentLine.textBasics[k].endIndex[n] > i)
                            currentLine.textBasics[k].endIndex[n] += difference;
                    }
                }
                for (int k = 0; k < currentLine.effectList.Count; k++)
                {
                    for (int n = 0; n < currentLine.effectList[k].startIndex.Count; n++)
                    {
                        if (currentLine.effectList[k].startIndex[n] > i)
                            currentLine.effectList[k].startIndex[n] += difference;
                    }
                    for (int n = 0; n < currentLine.effectList[k].endIndex.Count; n++)
                    {
                        if (currentLine.effectList[k].endIndex[n] > i)
                            currentLine.effectList[k].endIndex[n] += difference;
                    }
                }
                for (int k = 0; k < currentLine.colorList.Count; k++)
                {
                    for (int n = 0; n < currentLine.colorList[k].startIndex.Count; n++)
                    {
                        if (currentLine.colorList[k].startIndex[n] > i)
                            currentLine.colorList[k].startIndex[n] += difference;
                    }
                    for (int n = 0; n < currentLine.colorList[k].endIndex.Count; n++)
                    {
                        if (currentLine.colorList[k].endIndex[n] > i)
                            currentLine.colorList[k].endIndex[n] += difference;
                    }
                }
                parsedString.Remove(i, 3);
                parsedString.Insert(i, stringChange);
            }
        }
        if (currentLine.addStar)
            parsedString = "* " + parsedString;

        currentTextDisplay.text = parsedString;
        currentTextInfo = currentTextDisplay.textInfo;
    }

    void ColorButton(Button thisButton, bool highlight)
    {
        thisButton.buttonOutline.color = highlight ? buttonHighlight : buttonNormal;
        thisButton.text.color = highlight ? buttonHighlight : buttonNormal;
        if(thisButton.icon != null)
        {
            thisButton.icon.color = highlight ? buttonHighlight : buttonNormal;
            if (highlight && thisButton.focusedIcon != null)
                thisButton.icon.sprite = thisButton.focusedIcon;
            else if(!highlight && thisButton.unfocusedIcon != null)
                thisButton.icon.sprite = thisButton.unfocusedIcon;
        }
    }

    void ChangeMenuSelection()
    {
        switch(phase)
        {
            case BattlePhase.MainMenu:
                if (buttonIndex > 0 && input.directionDown.x < 0)
                    buttonIndex--;
                else if (buttonIndex < 2 && input.directionDown.x > 0)
                    buttonIndex++;
                for (int i = 0; i < mainMenuButtons.Length; i++)
                {
                    ColorButton(mainMenuButtons[i], i == buttonIndex);
                }
                break;
        }
    }

    void SelectMenuOption()
    {
        switch(phase)
        {
            case BattlePhase.MainMenu:
                switch(buttonIndex)
                {
                    case 0:
                        phase = BattlePhase.STG;
                        break;
                    case 1:
                        phase = BattlePhase.ActMenu;
                        break;
                    case 2:
                        phase = BattlePhase.ItemMenu;
                        break;
                }
                break;
        }
    }
}
