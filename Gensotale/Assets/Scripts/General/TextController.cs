using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextController : MonoBehaviour
{
    public static TextController textController;

    public TextData textData;

    public float charChangeTime;
    public SpriteRenderer portrait;
    Transform portraitTrans;
    public TextMeshPro dialogueText;
    Transform container;
    Transform textBox;
    TMP_TextInfo textInfo;
    public AnimationCurve bounceCurve;
    public AnimationCurve scaleCurve;

    [Space]
    
    Color characterSpeakingColor = Color.white;
    Color characterIdleColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);
    Vector2 speakingOffset = new Vector2(10, 0);
    Vector2 idleOffset = new Vector2(-10, -5);
    Vector3 idleScale = Vector3.one * .85f;
    Vector3 talkingScale = Vector3.one;

    public enum PortraitExpression { General, Smile, Laugh, Frown, Disappointed, Angry, Flustered, Sad, Crying, AngryCry, Sigh, Drained };

    int textTimer;
    int textIndex = -1;
    int basicsIndex;
    int[] colorIndex;
    float[] colorTime;

    int lineIndex;
    int currentChar;
    public bool addStar = false;
    public bool locked;
    public int shootTimeHeld;
    public bool holdingShoot;

    public bool dialogueBoxOpen;
    private bool dialogueBoxPrevOpen;
    public bool dialogueJustFinished;

    float lineTransTime = 0.25f;
    float boxOpenCloseTime = 0.25f;
    float timeSinceLineChange = 0;
    float timeSinceBoxOpenClosed = 0;

    GameMaster gameMaster;
    InputScript input;
    Transform thisTrans;
    int languageSetting;


    float offscreenPosition = 500;
    float verticalOffset = 0;
    float nameRemainTime = 3;

    private void Awake()
    {
        textController = this;
        thisTrans = transform;
        portraitTrans = portrait.transform;
        textBox = dialogueText.transform.parent;
        container = thisTrans.GetChild(0);
        textBox.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameMaster = GameMaster.gameMaster;
        languageSetting = (int)gameMaster.languageSetting;
        input = InputScript.inputScript;
        
        dialogueText.enableWordWrapping = true;

        dialogueText.ForceMeshUpdate();
        textInfo = dialogueText.textInfo;
    }

    private void Update()
    {
        dialogueJustFinished = false;
        timeSinceLineChange += gameMaster.frameTime * gameMaster.timeScale;
        timeSinceBoxOpenClosed += gameMaster.frameTime * gameMaster.timeScale;
        if (dialogueBoxOpen)
        {

            if (gameMaster.curSceneType == GameMaster.SceneType.Overworld)
                gameMaster.lockMovement = true;
            languageSetting = (int)gameMaster.languageSetting;
            TextData.TextLine thisLine = textData.lines[lineIndex];
            if (input.shootDown)
                holdingShoot = true;
            if (holdingShoot)
            {
                shootTimeHeld++;
                if (!input.shootPressed)
                    holdingShoot = false;
            }
            else
                shootTimeHeld = 0;

            if(thisLine.charIndex > 0)
                dialogueText.margin = new Vector4(textData.chars[thisLine.charIndex - 1].leftMargin, 0, 0, 0);
            else
                dialogueText.margin = new Vector4(0, 0, 0, 0);

            textTimer++;
            while (textIndex < textInfo.characterCount - 1 && textTimer >= thisLine.textBasics[basicsIndex].textDelay)
            {
                do
                {
                    do
                        textIndex++;
                    while (textInfo.characterInfo[textIndex].character == ' ');

                    if (thisLine.textBasics[basicsIndex].endIndex[languageSetting] < textIndex)
                        if (thisLine.textBasics.Count > basicsIndex + 1)
                            basicsIndex++;
                } while (thisLine.textBasics[basicsIndex].textDelay < 0);


                if (thisLine.textBasics[basicsIndex].textSound != null)
                    AudioSourceExtensions.PlayClip2D(thisLine.textBasics[basicsIndex].textSound);
                textTimer -= thisLine.textBasics[basicsIndex].textDelay;
            }
           

            MoveDialogueBits();

            if (textIndex >= textInfo.characterCount - 1)
            {
                if ((Input.GetKeyDown(KeyCode.LeftControl) || shootTimeHeld >= 60 || input.shootDown || thisLine.progressAutomatically))
                {
                    NextLine();
                }
            }
        }
        else if (timeSinceLineChange < 25)
            MoveEverythingOffscreen();
       
        dialogueBoxPrevOpen = dialogueBoxOpen;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(dialogueBoxOpen)
        {
            TextEffectProcessing(textData.lines[lineIndex], dialogueText, textInfo, ref colorTime, ref colorIndex, languageSetting);
            TextVisibility(textIndex, dialogueText, textInfo);
        }
    }

    public void StartDialogue(TextData newDialogue)
    {
        textData = newDialogue;
        textTimer -= Mathf.CeilToInt(boxOpenCloseTime * 60);
        lineIndex = 0;
        dialogueBoxOpen = true;
        addStar = textData.lines[0].addStar;
        colorTime = new float[textData.lines[0].colorList.Count];
        colorIndex = new int[textData.lines[0].colorList.Count];


        timeSinceLineChange = 0;
        timeSinceBoxOpenClosed = 0;
        dialogueText.text = (addStar ? "* " : "") + textData.lines[lineIndex].translation[languageSetting];
        textBox.localScale = new Vector3(0.01f, 1, 1);
        textBox.gameObject.SetActive(true);


        StartCoroutine(ChangePortrait(textData.lines[0].charIndex > 0 ? textData.chars[textData.lines[0].charIndex - 1].portraits[textData.lines[0].portraitIndex] :
            null, textData.lines[0].charIndex));
    }

    IEnumerator ChangePortrait(Sprite newSprite, int newChar)
    {
        while(dialogueBoxOpen && timeSinceBoxOpenClosed <= boxOpenCloseTime)
        {
            portraitTrans.localScale = new Vector3(1, 0, 1);
            yield return null;
        }
        float timePassed = 0;
        if(currentChar != newChar && currentChar != 0)
        {
            while(timePassed < charChangeTime)
            {
                timePassed += gameMaster.frameTime * gameMaster.timeScale;
                if (timePassed >= charChangeTime * 0.1f)
                    portraitTrans.localScale = new Vector3(1, Mathf.Lerp(1.1f, 0, (timePassed - charChangeTime * 0.1f) / (charChangeTime * 0.9f)), 1);
                else
                    portraitTrans.localScale = new Vector3(1, Mathf.Lerp(1, 1.1f, timePassed / (charChangeTime * 0.1f)), 1);

                yield return null;
            }
        }
        timePassed = 0;
        portrait.sprite = newSprite;
        if (currentChar != newChar && newChar != 0)
        {
            while (timePassed < charChangeTime)
            {
                timePassed += gameMaster.frameTime * gameMaster.timeScale;

                if (timePassed >= charChangeTime * 0.9f)
                    portraitTrans.localScale = new Vector3(1, Mathf.Lerp(1.1f, 1f, (timePassed - charChangeTime * 0.9f) / (charChangeTime * 0.1f)), 1);
                else
                    portraitTrans.localScale = new Vector3(1, Mathf.Lerp(0, 1.1f, timePassed / (charChangeTime * 0.9f)), 1);

                yield return null;
            }
        }

        currentChar = newChar;
    }

    void MoveDialogueBits()
    {
        textBox.localScale = new Vector3(Mathf.Lerp(0.01f, 1, timeSinceBoxOpenClosed / boxOpenCloseTime), 1, 1);
        float linePercentage = timeSinceLineChange / lineTransTime;
        
    }

    void EndDialogue()
    {
        dialogueJustFinished = true;
        dialogueBoxOpen = false;
        timeSinceLineChange = 0;
        timeSinceBoxOpenClosed = 0;
        if (gameMaster.curSceneType == GameMaster.SceneType.Overworld)
            gameMaster.lockMovement = false;
    }

    void MoveEverythingOffscreen()
    {
        float linePercentage = timeSinceLineChange / lineTransTime;
        textBox.localScale = new Vector3(1, Mathf.Lerp(1, 0.01f, timeSinceBoxOpenClosed / boxOpenCloseTime), 1);

        if (timeSinceBoxOpenClosed >= boxOpenCloseTime)
            textBox.gameObject.SetActive(false);
    }

    void NextLine()
    {
        lineIndex++;
        textIndex = -1;
        textTimer = 0;
        basicsIndex = 0;

        if (lineIndex >= textData.lines.Count)
        {
            EndDialogue();
            return;
        }
        addStar = textData.lines[lineIndex].addStar;


        StartCoroutine(ChangePortrait(textData.lines[lineIndex].charIndex > 0 ? textData.chars[textData.lines[lineIndex].charIndex - 1].portraits[textData.lines[lineIndex].portraitIndex] :
            null, textData.lines[lineIndex].charIndex));

        colorIndex = new int[textData.lines[lineIndex].colorList.Count];
        colorTime = new float[textData.lines[lineIndex].colorList.Count];

        dialogueText.text = (addStar ? "* " : "") + textData.lines[lineIndex].translation[languageSetting];
        
        timeSinceLineChange = 0;
    }

    public static void TextVisibility(int endVisibility, TextMeshPro dialogueText, TMP_TextInfo textInfo)
    {
        
        Color32[] colors = dialogueText.mesh.colors32;
        int vertIndex;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (i > endVisibility)
            {
                if (textInfo.characterInfo[i].character == ' ')
                    continue;

                vertIndex = textInfo.characterInfo[i].vertexIndex;
                for (int k = 0; k < 4; k++)
                {
                    colors[vertIndex + k] = new Color32(0, 0, 0, 0);
                }
            }
        }
        dialogueText.mesh.colors32 = colors;
    }

    public static void TextEffectProcessing(TextData.TextLine line, TextMeshPro dialogueText, TMP_TextInfo textInfo, ref float[] colorTimes, ref int[] colorIndex, int languageSetting)
    {
        dialogueText.ForceMeshUpdate();
        Vector3[] vertPos;
        int vertIndex;
        vertPos = dialogueText.mesh.vertices;
        bool addStar = line.addStar;
        GameMaster gameMaster = GameMaster.gameMaster;
        AnimationCurve bounceCurve = gameMaster.bounceCurve;
        AnimationCurve scaleCurve = gameMaster.scaleCurve;

        float scaleSize;

        for (int i = 0; i < line.effectList.Count; i++)
        {
            switch (line.effectList[i].effectType)
            {
                case TextData.EffectType.Bounce:
                    for (int a = line.effectList[i].startIndex[languageSetting] + (addStar ? 2 : 0);
                        a < line.effectList[i].endIndex[languageSetting] + 1 + (addStar ? 2 : 0); a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;
                        for (int b = 0; b < 4; b++)
                        {
                            vertPos[vertIndex + b].y += bounceCurve.Evaluate(Mathf.Repeat(a / 5f + (Time.time * line.effectList[i].speed), 1))
                                * line.effectList[i].distance;
                        }
                    }
                    break;

                case TextData.EffectType.Shake:
                    Vector3 newOffset;
                    for (int a = line.effectList[i].startIndex[languageSetting] + (addStar ? 2 : 0);
                        a < line.effectList[i].endIndex[languageSetting] + 1 + (addStar ? 2 : 0); a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;
                        newOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)).normalized * line.effectList[i].distance;

                        for (int b = 0; b < 4; b++)
                        {
                            vertPos[vertIndex + b] += newOffset;
                        }
                    }
                    break;

                case TextData.EffectType.WindWave:
                    for (int a = line.effectList[i].startIndex[languageSetting] + (addStar ? 2 : 0);
                        a < line.effectList[i].endIndex[languageSetting] + 1 + (addStar ? 2 : 0); a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;

                        for (int b = 0; b < 2; b += 2)
                        {
                            scaleSize = scaleCurve.Evaluate(Mathf.Repeat((a) / 5f + (Time.time * line.effectList[i].speed), 1))
                                * line.effectList[i].distance;
                            vertPos[vertIndex + b].x += scaleSize * Mathf.Sign(b - 1);
                            vertPos[vertIndex + b].y += scaleSize * Mathf.Sign(b - 1);
                            vertPos[vertIndex + b + 1].x += scaleSize * Mathf.Sign(b - 1);
                            vertPos[vertIndex + b + 1].y += scaleSize * Mathf.Sign(b - 1);
                        }
                    }
                    break;

                case TextData.EffectType.Dance:
                    for (int a = line.effectList[i].startIndex[languageSetting] + (addStar ? 2 : 0);
                        a < line.effectList[i].endIndex[languageSetting] + 1 + (addStar ? 2 : 0); a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;

                        for (int b = 0; b < 4; b++)
                        {
                            scaleSize = scaleCurve.Evaluate(Mathf.Repeat((b) / 5f + (Time.time * line.effectList[i].speed), 1))
                                * line.effectList[i].distance;
                            vertPos[vertIndex + b].x += scaleSize * Mathf.Sign(b - 1);
                            vertPos[vertIndex + b].y += scaleSize * Mathf.Sign(b - 1);
                        }
                    }
                    break;

                /*case TextData.EffectType.:
                    for (int a = line.effectList[i].startIndex[languageSetting];
                        a < line.effectList[i].endIndex[languageSetting] + 1; a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;

                        for (int b = 0; b < 4; b++)
                        {
                            scaleSize = scaleCurve.Evaluate(Mathf.Repeat((b) / 5f + (Time.time * textSettings[lineIndex].effectList[i].speed), 1))
                                * textSettings[lineIndex].effectList[i].distance;
                            vertPos[vertIndex + b].x += scaleSize * Mathf.Sign(b - 1);
                            vertPos[vertIndex + b].y += scaleSize * Mathf.Sign(b - 1);
                        }
                    }
                    break;*/
            }
        }

        dialogueText.mesh.vertices = vertPos;

        Color32[] colors = dialogueText.mesh.colors32;
        float colorValue = 0;
        for (int i = 0; i < line.colorList.Count; i++)
        {
            switch (line.colorList[i].effectType)
            {
                case TextData.ColorType.Color:
                    for (int a = line.colorList[i].startIndex[languageSetting] + (addStar ? 2 : 0);
                        a < line.colorList[i].endIndex[languageSetting] + 1 + (addStar ? 2 : 0); a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;

                        for (int b = 0; b < 4; b++)
                        {
                            colors[vertIndex + b] = line.colorList[i].colors[Mathf.Clamp(b, 0,
                                line.colorList[i].colors.Count - 1)];
                        }
                    }
                    break;

                case TextData.ColorType.Flash:
                    colorTimes[i] += gameMaster.frameTime * line.colorList[i].colors.Count;
                    if (colorTimes[i] > line.colorList[i].time)
                    {
                        colorIndex[i]++;
                        colorTimes[i] -= line.colorList[i].time;
                    }
                    colorValue = colorTimes[i] / line.colorList[i].time;
                    for (int a = line.colorList[i].startIndex[languageSetting] + (addStar ? 2 : 0);
                        a < line.colorList[i].endIndex[languageSetting] + 1 + (addStar ? 2 : 0); a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;

                        for (int b = 0; b < 4; b++)
                        {
                            colors[vertIndex + b] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i], line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + 1, line.colorList[i].colors.Count)], 
                                colorValue);
                        }
                    }
                    break;

                case TextData.ColorType.Wave:
                    colorTimes[i] += gameMaster.frameTime * line.colorList[i].colors.Count;
                    if (colorTimes[i] > line.colorList[i].time)
                    {
                        colorIndex[i]++;
                        colorTimes[i] -= line.colorList[i].time;
                    }
                    colorValue = colorTimes[i] / line.colorList[i].time;
                    for (int a = line.colorList[i].startIndex[languageSetting] + (addStar ? 2 : 0);
                        a < line.colorList[i].endIndex[languageSetting] + 1 + (addStar ? 2 : 0); a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;

                        for (int b = 0; b < 4; b++)
                        {
                            colors[vertIndex + b] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)], colorValue);
                        }
                    }
                    break;

                case TextData.ColorType.Vert:
                    colorTimes[i] += gameMaster.frameTime * line.colorList[i].colors.Count;
                    if (colorTimes[i] > line.colorList[i].time)
                    {
                        colorIndex[i]++;
                        colorTimes[i] -= line.colorList[i].time;
                    }
                    colorValue = colorTimes[i] / line.colorList[i].time;
                    for (int a = line.colorList[i].startIndex[languageSetting] + (addStar ? 2 : 0);
                        a < line.colorList[i].endIndex[languageSetting] + 1 + (addStar ? 2 : 0); a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;

                        colors[vertIndex + 0] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 1] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 2, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 2] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 2, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 3, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 3] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 3, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 4, line.colorList[i].colors.Count)], colorValue);
                    }
                    break;
                    
                case TextData.ColorType.VertWaveLeft:
                    colorTimes[i] += gameMaster.frameTime * line.colorList[i].colors.Count;
                    if (colorTimes[i] > line.colorList[i].time)
                    {
                        colorIndex[i]++;
                        colorTimes[i] -= line.colorList[i].time;
                    }
                    colorValue = colorTimes[i] / line.colorList[i].time;
                    for (int a = line.colorList[i].startIndex[languageSetting] + (addStar ? 2 : 0);
                        a < line.colorList[i].endIndex[languageSetting] + 1 + (addStar ? 2 : 0); a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;

                        colors[vertIndex + 0] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 1] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 2] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 2, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 3] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 2, line.colorList[i].colors.Count)], colorValue);
                    }
                    break;

                case TextData.ColorType.VertWaveRight:
                    colorTimes[i] += gameMaster.frameTime * line.colorList[i].colors.Count;
                    if (colorTimes[i] > line.colorList[i].time)
                    {
                        colorIndex[i]++;
                        colorTimes[i] -= line.colorList[i].time;
                    }
                    colorValue = colorTimes[i] / line.colorList[i].time;
                    for (int a = line.colorList[i].startIndex[languageSetting] + (addStar ? 2 : 0);
                        a < line.colorList[i].endIndex[languageSetting] + 1 + (addStar ? 2 : 0); a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;

                        colors[vertIndex + 0] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 2, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 1] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 2, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 2] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 3] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)], colorValue);
                    }
                    break;

                case TextData.ColorType.StripeDiagonalLeft:
                    colorTimes[i] += gameMaster.frameTime * line.colorList[i].colors.Count;
                    if (colorTimes[i] > line.colorList[i].time)
                    {
                        colorIndex[i]++;
                        colorTimes[i] -= line.colorList[i].time;
                    }
                    colorValue = colorTimes[i] / line.colorList[i].time;
                    for (int a = line.colorList[i].startIndex[languageSetting] + (addStar ? 2 : 0);
                        a < line.colorList[i].endIndex[languageSetting] + 1 + (addStar ? 2 : 0); a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;

                        colors[vertIndex + 0] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 1] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 2, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 2] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 3] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 2, line.colorList[i].colors.Count)], colorValue);
                    }
                    break;

                case TextData.ColorType.StripeDiagonalRight:
                    colorTimes[i] += gameMaster.frameTime * line.colorList[i].colors.Count;
                    if (colorTimes[i] > line.colorList[i].time)
                    {
                        colorIndex[i]++;
                        colorTimes[i] -= line.colorList[i].time;
                    }
                    colorValue = colorTimes[i] / line.colorList[i].time;
                    for (int a = line.colorList[i].startIndex[languageSetting] + (addStar ? 2 : 0);
                        a < line.colorList[i].endIndex[languageSetting] + 1 + (addStar ? 2 : 0); a++)
                    {
                        if (textInfo.characterInfo[a].character == ' ')
                            continue;

                        vertIndex = textInfo.characterInfo[a].vertexIndex;

                        colors[vertIndex + 0] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 2, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 1] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 2] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 2, line.colorList[i].colors.Count)], colorValue);
                        colors[vertIndex + 3] = Color32.Lerp(line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a, line.colorList[i].colors.Count)],
                                line.colorList[i].colors[(int)Mathf.Repeat(colorIndex[i] + a + 1, line.colorList[i].colors.Count)], colorValue);
                    }
                    break;
            }
        }

        dialogueText.mesh.colors32 = colors;
    }
}
