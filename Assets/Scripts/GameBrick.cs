using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBrick : MonoBehaviour
{
    //方块的基础类

    private float x, y; //坐标
    private GameManager.BrickType type; //砖块类型
    private MoveBrick moveComponent; //移动组件
    private ColorCat colorComponent; //着色组件
    private ClearBrick clearComponent; //清除组件

    public float X {
        get => x;
        set {
            if (CanMove())
                x = value;
        }
    }
    public float Y { get => y; set => y = value; }
    public GameManager.BrickType Type { get => type; set => type = value; }
    public MoveBrick MoveComponent { get => moveComponent; set => moveComponent = value; }
    public ColorCat ColorComponent { get => colorComponent; set => colorComponent = value; }
    public ClearBrick ClearComponent { get => clearComponent; set => clearComponent = value; }

    private void Awake()
    {
        moveComponent = GetComponent<MoveBrick>();
        ColorComponent = GetComponent<ColorCat>();
        ClearComponent = GetComponent<ClearBrick>();
    }

    public bool CanMove()
    {
        return MoveComponent != null;
    }
    public bool CanColor()
    {
        return ColorComponent != null;
    }
    public bool CanClear()
    {
        return ClearComponent != null;
    }

    public void Init(float x,float y,GameManager.BrickType type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }
    
    //监听操作函数
    private void OnMouseDown()
    {
        if (GameManager.Instance.gameOver || GameManager.Instance.gamePause)
            return;

        GameManager.Instance.brickA = this;
    }
    private void OnMouseEnter()
    {
        if (GameManager.Instance.gameOver || GameManager.Instance.gamePause)
            return;

        GameManager.Instance.brickB = this;
    } 
    private void OnMouseUp()
    {
        if (GameManager.Instance.gameOver || GameManager.Instance.gamePause)
            return;

        GameManager.Instance.ExchangeBrick();
    }
}
