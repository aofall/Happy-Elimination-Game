using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorCat : MonoBehaviour
{
    //猫的颜色相关

    public enum ColorType
    {
        WHITE,
        GRAY,
        GREEN,
        YELLOW,
        RED,
        PURPLE
            //枚举定义猫的颜色类型
    }

    private ColorType type;

    private int colorTypeSize;
    public int ColorTypeSize { 
        get => colorSprites.Length; 
    }
    public ColorType Type { 
        get => type; set => type = value; 
    }

    private Dictionary<ColorType, Sprite> colorSpriteDict;
    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType type;
        public Sprite sprite;
    }
    public ColorSprite[] colorSprites;

    private SpriteRenderer render;

    private void Awake()
    {
        render = GetComponentInChildren<SpriteRenderer>();

        colorSpriteDict = new Dictionary<ColorType, Sprite>();
        for (int i = 0; i < colorSprites.Length; i++)
        {
            colorSpriteDict.Add(colorSprites[i].type, colorSprites[i].sprite);
        }
    }

    public void SetColor(ColorType type)
    {
        render.sprite = colorSpriteDict[type];
        this.type = type;
    }
}
