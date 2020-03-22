using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BasicEditablePattern), true), CanEditMultipleObjects]
public class BasicEditablePatternEditor : Editor
{
    static int shownPattern = 0;

    public override void OnInspectorGUI()
    {
        if (targets.Length > 1)
        {
            DrawDefaultInspector();
            return;
        }

        BasicEditablePattern thisData = (BasicEditablePattern)target;
        //editingDifficulty = (int)(GameMasterScript.Difficulty)EditorGUILayout.EnumPopup("Editing Difficulty", (GameMasterScript.Difficulty)editingDifficulty);
        
        

        int patternCount = thisData.patterns.Count;

        GUILayout.Space(8);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.IntField("Current pattern: " + shownPattern.ToString(), shownPattern);
        if (GUILayout.Button("Delete"))
        {
            thisData.patterns.RemoveAt(shownPattern);
            patternCount = thisData.patterns.Count;
            shownPattern = Mathf.Clamp(shownPattern, 0, patternCount - 1);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(8);

        EditorGUILayout.LabelField("Pattern Selection:");
        string[] patternSelections = new string[patternCount + 1];
        for (int i = 0; i < patternSelections.Length; i++)
        {
            if (i == patternCount)
                patternSelections[i] = "Add";
            else
                patternSelections[i] = i.ToString();
        }
        shownPattern = GUILayout.SelectionGrid(shownPattern, patternSelections, 3);


        if (shownPattern == patternCount)
        {
            thisData.patterns.Add(thisData.patterns[thisData.patterns.Count - 1].CopyPattern());
            patternCount = thisData.patterns.Count;
            shownPattern = Mathf.Clamp(shownPattern, 0, patternCount - 1);
        }

        if (patternCount == 0)
            return;


        GUILayout.Space(16);

        Pattern thisPattern = thisData.patterns[shownPattern];

        thisPattern.burstCount = EditorGUILayout.IntField("Burst Count", thisPattern.burstCount);
        thisPattern.bulletsInBurst = EditorGUILayout.IntField("Bullets in Burst", thisPattern.bulletsInBurst);
        thisPattern.overallDegrees = EditorGUILayout.FloatField("Overall Degrees", thisPattern.overallDegrees);
        thisPattern.degreeOffset = EditorGUILayout.FloatField("Degree Offset", thisPattern.degreeOffset);
        thisPattern.randomDegreeOffset = EditorGUILayout.FloatField("Random Offset", thisPattern.randomDegreeOffset);
        thisPattern.perBulletRandomOffset = EditorGUILayout.Toggle("Per-Bullet Offset", thisPattern.perBulletRandomOffset);
        thisPattern.aimed = EditorGUILayout.Toggle("Aimed", thisPattern.aimed);

        GUILayout.Space(8);

        thisPattern.burstDelay = EditorGUILayout.FloatField("Burst Delay", thisPattern.burstDelay);
        thisPattern.initialDelay = EditorGUILayout.FloatField("Initial Delay", thisPattern.initialDelay);

        if (thisPattern.perBulletDelay.Count == 0)
            thisPattern.perBulletDelay.Add(0);

        for (int k = 0; k < thisPattern.perBulletDelay.Count; k++)
        {
            thisPattern.perBulletDelay[k] = EditorGUILayout.FloatField("Per-Bullet Delay " + k.ToString(), thisPattern.perBulletDelay[k]);
        }
        if (GUILayout.Button("Add new bullet delay"))
        {
            if (thisPattern.perBulletDelay.Count > 0)
                thisPattern.perBulletDelay.Add(thisPattern.perBulletDelay[thisPattern.perBulletDelay.Count - 1]);
            else
                thisPattern.perBulletDelay.Add(0);
        }
        else if (GUILayout.Button("Remove bullet delay"))
        {
            thisPattern.perBulletDelay.RemoveAt(thisPattern.perBulletDelay.Count - 1);
        }

        GUILayout.Space(8);

        for (int k = 0; k < thisPattern.bulletType.Count; k++)
        {
            /*string[] typeSelections = typeof(BaseBulletScript.BulletType).GetEnumNames();
            thisPattern.bulletType[k] = (BaseBulletScript.BulletType)GUILayout.SelectionGrid((int)thisPattern.bulletType[k], typeSelections, 6);*/
            GUILayout.Space(8);
            thisPattern.bulletType[k] = (BaseBulletScript.BulletType)EditorGUILayout.EnumPopup("Bullet Type " + k.ToString(), thisPattern.bulletType[k]);
            if (thisPattern.bulletType[k] == BaseBulletScript.BulletType.Custom)
            {
                thisPattern.customBullet[k] = (GameObject)EditorGUILayout.ObjectField("Custom Bullet", thisPattern.customBullet[k], typeof(GameObject), false);
                thisPattern.customHasScript[k] = EditorGUILayout.Toggle("Custom Has Script", thisPattern.customHasScript[k]);
                thisPattern.customHasSprite[k] = EditorGUILayout.Toggle("Custom Has Sprite", thisPattern.customHasSprite[k]);
                if (!thisPattern.customHasSprite[k])
                    thisPattern.customSpriteType[k] = (BaseBulletScript.BulletType)EditorGUILayout.EnumPopup("Custom Sprite Type " + k.ToString(), thisPattern.customSpriteType[k]);
            }
            /*else if (thisPattern.bulletType[k] == BaseBulletScript.BulletType.CustomLaser)
            {
                thisPattern.customBullet[k] = (GameObject)EditorGUILayout.ObjectField("Custom Laser", thisPattern.customBullet[k], typeof(GameObject), false);
                thisPattern.customHasScript[k] = EditorGUILayout.Toggle("Laser Flip X", thisPattern.customHasScript[k]);
                thisPattern.customHasSprite[k] = EditorGUILayout.Toggle("Laser Flip Y", thisPattern.customHasSprite[k]);
            }*/
        }
        if (GUILayout.Button("Add new bullet type"))
        {
            if (thisPattern.bulletType.Count > 0)
            {
                thisPattern.bulletType.Add(thisPattern.bulletType[thisPattern.bulletType.Count - 1]);
                thisPattern.customSpriteType.Add(thisPattern.customSpriteType[thisPattern.customSpriteType.Count - 1]);
                thisPattern.customHasScript.Add(thisPattern.customHasScript[thisPattern.customHasScript.Count - 1]);
                thisPattern.customHasSprite.Add(thisPattern.customHasSprite[thisPattern.customHasSprite.Count - 1]);
                thisPattern.customBullet.Add(thisPattern.customBullet[thisPattern.customBullet.Count - 1]);
            }
            else
            {
                thisPattern.bulletType.Add(BaseBulletScript.BulletType.Round);
                thisPattern.customSpriteType.Add(BaseBulletScript.BulletType.Round);
                thisPattern.customHasScript.Add(false);
                thisPattern.customHasSprite.Add(false);
                thisPattern.customBullet.Add(null);
            }
        }
        else if (GUILayout.Button("Remove bullet type"))
        {
            thisPattern.bulletType.RemoveAt(thisPattern.bulletType.Count - 1);
            thisPattern.customSpriteType.RemoveAt(thisPattern.customSpriteType.Count - 1);
            thisPattern.customHasScript.RemoveAt(thisPattern.customHasScript.Count - 1);
            thisPattern.customHasSprite.RemoveAt(thisPattern.customHasSprite.Count - 1);
            thisPattern.customBullet.RemoveAt(thisPattern.customBullet.Count - 1);
        }
        GUILayout.Space(8);
        for (int k = 0; k < thisPattern.bulletColors.Count; k++)
        {
            thisPattern.bulletColors[k] = (BaseBulletScript.BulletColor)EditorGUILayout.EnumPopup("Bullet Color " + k.ToString(), thisPattern.bulletColors[k]);
            GUILayout.Space(8);
            //thisPattern.bulletColors[k] = (BaseBulletScript.BulletColor)EditorGUILayout.EnumPopup("Bullet color " + k.ToString(), thisPattern.bulletColors[k]);
        }
        if (GUILayout.Button("Add new bullet color"))
        {
            if (thisPattern.bulletColors.Count > 0)
                thisPattern.bulletColors.Add(thisPattern.bulletColors[thisPattern.bulletColors.Count - 1]);
            else
                thisPattern.bulletColors.Add(BaseBulletScript.BulletColor.White);
        }
        else if (GUILayout.Button("Remove bullet color"))
        {
            thisPattern.bulletColors.RemoveAt(thisPattern.bulletColors.Count - 1);
        }

        GUILayout.Space(8);
        //bool isLaser = thisPattern.bulletType[0] == BaseBulletScript.BulletType.StraightLaser || thisPattern.bulletType[0] == BaseBulletScript.BulletType.CustomLaser;
        bool isLaser = false;
        for (int k = 0; k < thisPattern.bulletEndSpeed.Count; k++)
        {
            EditorGUILayout.LabelField("Pattern 1 Movement set " + k.ToString());
            thisPattern.bulletEndSpeed[k] = EditorGUILayout.FloatField(isLaser ?
                "Time multiplier" : "Speed", thisPattern.bulletEndSpeed[k]);
            thisPattern.bulletStartSpeed[k] = EditorGUILayout.FloatField(isLaser ?
                "Width" : "Start Speed", thisPattern.bulletStartSpeed[k]);
            thisPattern.bulletTimeToSpeedUp[k] = EditorGUILayout.FloatField(isLaser ?
                "Segment Delay" : "Speed Up Time", thisPattern.bulletTimeToSpeedUp[k]);
            if (!isLaser)
            {
                thisPattern.turn[k] = EditorGUILayout.Toggle("Turn", thisPattern.turn[k]);
            }
            if (k < thisPattern.bulletEndSpeed.Count - 1)
                GUILayout.Space(8);
        }
        if (GUILayout.Button("Add new movement"))
        {
            if (thisPattern.bulletEndSpeed.Count > 0)
            {
                thisPattern.bulletEndSpeed.Add(thisPattern.bulletEndSpeed[thisPattern.bulletEndSpeed.Count - 1]);
                thisPattern.bulletStartSpeed.Add(thisPattern.bulletStartSpeed[thisPattern.bulletStartSpeed.Count - 1]);
                thisPattern.bulletTimeToSpeedUp.Add(thisPattern.bulletTimeToSpeedUp[thisPattern.bulletTimeToSpeedUp.Count - 1]);
                thisPattern.turn.Add(thisPattern.turn[thisPattern.turn.Count - 1]);
            }
            else
            {
                thisPattern.bulletEndSpeed.Add(0);
                thisPattern.bulletStartSpeed.Add(0);
                thisPattern.bulletTimeToSpeedUp.Add(0);
                thisPattern.turn.Add(false);
            }
        }
        else if (GUILayout.Button("Remove movement"))
        {
            thisPattern.bulletEndSpeed.RemoveAt(thisPattern.bulletEndSpeed.Count - 1);
            thisPattern.bulletStartSpeed.RemoveAt(thisPattern.bulletStartSpeed.Count - 1);
            thisPattern.bulletTimeToSpeedUp.RemoveAt(thisPattern.bulletTimeToSpeedUp.Count - 1);
            thisPattern.turn.RemoveAt(thisPattern.turn.Count - 1);
        }
        GUILayout.Space(16);
        for (int k = 0; k < thisPattern.offsets.Count; k++)
        {
            thisPattern.offsets[k] = EditorGUILayout.Vector2Field("Offset", thisPattern.offsets[k]);
            thisPattern.turnOffsets[k] = EditorGUILayout.Toggle("Turn", thisPattern.turnOffsets[k]);
            GUILayout.Space(8);
        }
        if (GUILayout.Button("Add new offset"))
        {
            if (thisPattern.offsets.Count > 0)
                thisPattern.offsets.Add(thisPattern.offsets[thisPattern.offsets.Count - 1]);
            else
                thisPattern.offsets.Add(Vector2.zero);
            thisPattern.turnOffsets.Add(true);
        }
        else if (GUILayout.Button("Remove offset"))
        {
            thisPattern.offsets.RemoveAt(thisPattern.offsets.Count - 1);
            thisPattern.turnOffsets.RemoveAt(thisPattern.turnOffsets.Count - 1);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(thisData);
        }
    }
}
