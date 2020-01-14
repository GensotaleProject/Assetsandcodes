using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewText", menuName = "TextData/Text", order = 0)]
public class TextData : ScriptableObject
{
    [SerializeField] public List<TextLine> lines = new List<TextLine>();
    
    [System.Serializable]
    public class TextBasics
    {
        public int textDelay;
        public AudioClip textSound;
        public List<int> startIndex = new List<int>();
        public List<int> endIndex = new List<int>();
    }

    [System.Serializable]
    public class TextEffects
    {
        public EffectType effectType;
        public float distance;
        public float speed;
        public List<int> startIndex = new List<int>();
        public List<int> endIndex = new List<int>();
    }

    [System.Serializable]
    public class ColorEffects
    {
        public ColorType effectType;
        public List<Color32> colors = new List<Color32>();
        public float speed;
        public List<int> startIndex = new List<int>();
        public List<int> endIndex = new List<int>();
    }

    public enum EffectType { None, Bounce, Shake, WindWave, Dance };
    public enum ColorType { Color, Flash, Wave, VertWave };

    [System.Serializable]
    public class TextLine
    {
        public int skipDelay = 0;
        public bool progressAutomatically;
        public List<string> translation = new List<string>();
        [Space]
        [Space]
        public List<TextBasics> textBasics = new List<TextBasics>();
        [Space]
        [Space]
        public List<TextEffects> effectList = new List<TextEffects>();
        [Space]
        [Space]
        public List<ColorEffects> colorList = new List<ColorEffects>();

        public TextLine()
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(GameMaster.Language)).Length; i++)
            {
                translation.Add("");
            }
        }
    }
    
}
