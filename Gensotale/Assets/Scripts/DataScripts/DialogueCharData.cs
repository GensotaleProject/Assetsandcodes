using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "NewDialogueChar", menuName = "TextData/Character", order = 1)]
public class DialogueCharData : ScriptableObject
{
    public List<Sprite> portraits = new List<Sprite>();
    public AudioClip speakSound;
    public float horizontalPosition = -120;
    public float leftMargin = 85;
}
