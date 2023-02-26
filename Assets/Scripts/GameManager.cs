using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float upLevelScore = 100; //100分过关
    public float level = 1; //关卡
    public GameObject upLevelPanel;

    public Text upLevelScoreText;
    public Text levelText;

    public float fillTime = 0.5f; //等待0.5s

    public AudioClip destroyMusic;

    private static GameManager _instance;
    public static GameManager Instance { get => _instance; set => _instance = value; }

    public GameObject catPrefab;
    public GameObject catsGameObject;
    private float catScale = 20;
    private float catOffset = 10;

    public int row;
    public int col;

    public Text timeText;
    public float gameTime = 60f; //60s
    public bool gameOver = false;
    public bool gamePause = false;

    public int score = 0;
    public Text scoreText;

    public GameObject endPanel;

    public GameObject pausePanel;

    //过关
    public void UpLevel()
    {
        if ((level + 1).ToString() == "6")
            return;

        SceneManager.LoadScene((level+1).ToString()); //下一关
    }

    //暂停游戏（打开暂停面板）
    public void OpenPausePanel()
    {
        gamePause = true;
        pausePanel.SetActive(true);
    }

    //取消暂停（关闭暂停面板）
    public void ClosePausePanel()
    {
        gamePause = false;
        pausePanel.SetActive(false);
    }

    //回到游戏主页
    public void Home()
    {
        SceneManager.LoadScene(0);
    }

    //重玩当前关卡
    public void Replay()
    {
        SceneManager.LoadScene(level.ToString());
    }


    private void Update()
    {
        if (gameOver || gamePause)
            return;

        //过关判定
        if (score >= upLevelScore)
        {
            gamePause = true;
            upLevelPanel.SetActive(true); //显示界面
        }

        gameTime -= Time.deltaTime;
        timeText.text = gameTime.ToString("0");
        if (gameTime <= 0)
        {
            endPanel.SetActive(true);

            gameTime = 0;
            gameOver = true;
        }

        scoreText.text = "Score: " + score.ToString();
    }

    //砖块类型 {空,普通,障碍,行,列,全部,标记}
    public enum BrickType
    {
        EMPTY,
        CAT,
        BARRIER,
        ROW_CLEAR, //清除列所有砖块，暂未使用
        COL_CLEAR, //清除行所有砖块，暂未使用
        ALL_CLEAR, //清除行列所有砖块，暂未使用
        SIGN //标记砖块，暂未使用
    }

    public Dictionary<BrickType, GameObject> brickPrefabDict;
    [System.Serializable]
    public struct BrickPrefab
    {
        public BrickType type;
        public GameObject prefab;
    }
    public BrickPrefab[] brickPrefabs;

    //砖块地图
    public GameBrick[,] brickMap; 

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        levelText.text = "level " + level.ToString() + "/5";
        upLevelScoreText.text = upLevelScore.ToString() + "分过关";

        brickMap = new GameBrick[row,col];

        //brickPrefabs结构体数组赋值给brickPrefabDict
        brickPrefabDict = new Dictionary<BrickType, GameObject>();
        for(int i = 0; i < brickPrefabs.Length; i++)
        {
            brickPrefabDict.Add(brickPrefabs[i].type, brickPrefabs[i].prefab);
        }

        //生成砖块
        for (int j = col-1; j >=0; j--)
        {
            for (int i = 0; i < row; i++)
            {
                CreateBrick(i, j, BrickType.CAT);
            }
        }

        ////空格子测试
        //Destroy(brickMap[0, 0].gameObject);
        //CreateBrick(0, 0, BrickType.EMPTY);

        //障碍砖块的定义
        //障碍测试
        Destroy(brickMap[2, 2].gameObject);
        CreateBrick(2, 2, BrickType.BARRIER);
        //障碍测试
        Destroy(brickMap[3, 2].gameObject);
        CreateBrick(3, 2, BrickType.BARRIER);

        StartCoroutine(AllFill()); //开启协程
    }

    //全图检测匹配
    private bool ClearAllMatchBrick() {
        bool isClear = false;

        for(int i = 0; i < row; i++)
        {
            for(int j = 0; j < col; j++)
            {
                if (brickMap[i, j].CanClear())
                {
                    List<GameBrick> matchBricks = MatchBrick(i,j);

                    if (matchBricks.Count!=0)
                    {
                        for (int k = 0; k < matchBricks.Count; k++)
                            ClearBrick((int)matchBricks[k].X, (int)matchBricks[k].Y);

                        AudioSource.PlayClipAtPoint(destroyMusic, transform.position); //播放声音
                        isClear = true;
                    }
                }
            }
        }

        return isClear;
    }

    //单点检测匹配
    private List<GameBrick> MatchBrick(int x,int y) {
        List<GameBrick> matchBricks = new List<GameBrick>();
        List<GameBrick> matchRowBricks = new List<GameBrick>();
        List<GameBrick> matchColBricks = new List<GameBrick>();

        int ox = x, oy = y;

        //向上向下扩展     
        matchRowBricks.Add(brickMap[ox, oy]);
        while(x-1>=0 && brickMap[x - 1, oy].CanColor() && brickMap[ox,oy].ColorComponent.Type == brickMap[x-1, oy].ColorComponent.Type) //向左
        {
            matchRowBricks.Add(brickMap[x-1, oy]);
            x -= 1;
        }
        x = ox;
        while (x + 1 <row && brickMap[x+1, oy].CanColor() && brickMap[ox, oy].ColorComponent.Type == brickMap[x + 1, oy].ColorComponent.Type) //向左
        {
            matchRowBricks.Add(brickMap[x + 1, oy]);
            x += 1;
        }
        if (matchRowBricks.Count >= 3)
        {
            for (int i = 0; i < matchRowBricks.Count; i++)
            {
                matchBricks.Add(matchRowBricks[i]);
            }
        }
        x = ox;

        //向左向右扩展
        matchColBricks.Add(brickMap[ox, oy]);
        while (y - 1 >= 0 && brickMap[ox,y-1].CanColor() && brickMap[ox, oy].ColorComponent.Type == brickMap[ox, y-1].ColorComponent.Type) //向左
        {
            matchColBricks.Add(brickMap[ox, y - 1]);
            y -= 1;
        }
        y = oy;
        while (y + 1 < col && brickMap[ox, y + 1].CanColor() && brickMap[ox, oy].ColorComponent.Type == brickMap[ox, y+1].ColorComponent.Type) //向右
        {
            matchColBricks.Add(brickMap[ox, y + 1]);
            y += 1;
        }
        if (matchColBricks.Count >= 3)
        {
            for (int i = 0; i < matchColBricks.Count; i++)
                matchBricks.Add(matchColBricks[i]);
        }
        y = oy;

        return matchBricks;
    }

    //清除方块
    public bool ClearBrick(int x,int y)
    {
        if(brickMap[x,y].CanClear() && !brickMap[x, y].ClearComponent.isClear)
        {
            brickMap[x, y].ClearComponent.Clear();
            CreateBrick(x, y, BrickType.EMPTY);

            score++; //得分+1
            return true;
        }

        return false;
    }

    //下降和消除方法协程
    public IEnumerator AllFill()
    {
        bool haveMatch = false;

        while (Fill() && !gameOver)
        {
            yield return new WaitForSeconds(fillTime);

            if (haveMatch) //如果上一轮有消除 那么在Fill()后补充一次
            {
                SupplyBrick();
                haveMatch = false;
                yield return new WaitForSeconds(fillTime);
            }

            haveMatch = ClearAllMatchBrick();
            if (!haveMatch)
                break;

            yield return new WaitForSeconds(fillTime);
        }
    }

    //补充函数 只补顶部到第一个不为空的所有位置
    public void SupplyBrick()
    {
        for(int i = 0; i < row; i++)
        {
            int j = col - 1;
            while (j-1>=0 && brickMap[i, j-1].Type == BrickType.EMPTY)
                j -= 1;

            if (brickMap[i, j].Type == BrickType.EMPTY)
            {
                for (; j < col; j++)
                {
                    Destroy(brickMap[i, j].gameObject); //消除空对象

                    CreateBrick(i, col - 1, BrickType.CAT); //在顶端创建新的猫咪
                    brickMap[i, col - 1].MoveComponent.Move(i, j); //下降
                    brickMap[i, j] = brickMap[i, col - 1];

                    if (j != col - 1)
                        CreateBrick(i, col - 1, BrickType.EMPTY); //补充空对象
                }
            }
        }
    }

    public bool Fill()
    {
        //检测空白填充 双指针法
        for(int i = 0; i < row; i++)
        {
            int len = 0;
            for(int j = 0; j < col; j++)
            {
                if (brickMap[i, j].Type != BrickType.EMPTY) {
                    if (brickMap[i, j].Type == BrickType.BARRIER) { //[i,j]是障碍直接把len=j+1; 而下一轮j也只能是j+1,加入边界检测就可以了
                        len = j;
                    }
                    else if (len<col && len != j)
                    {
                        Destroy(brickMap[i, len].gameObject); //消除空对象
                        brickMap[i, j].MoveComponent.Move(i, len); //下降
                        brickMap[i, len] = brickMap[i, j];
                        CreateBrick(i, j, BrickType.EMPTY); //新建空对象
                    }

                    len++;
                }
            }
        }
        return true;
    }

    //生成砖块函数
    public void CreateBrick(int i,int j,BrickType type)
    {
        GameObject newGameObject = Instantiate(brickPrefabDict[type], FixXY(i, j), Quaternion.identity);
        newGameObject.transform.SetParent(catsGameObject.transform);

        brickMap[i, j] = newGameObject.GetComponent<GameBrick>();
        brickMap[i, j].Init(i, j, type);

        //随机颜色
        if (brickMap[i, j].CanColor())
        {
            brickMap[i, j].ColorComponent.SetColor((ColorCat.ColorType)Random.Range(0, brickMap[i, j].ColorComponent.ColorTypeSize));
        }
    }

    //修正brick坐标
    public Vector3 FixXY(int x,int y)
    {
        float nx = x - (float)(row * 1.0) / 2 + 0.5f;
        float ny = y - (float)(col * 1.0) + 0.5f;
        return new Vector3(nx,ny, 0);
    }

    public GameBrick brickA;
    public GameBrick brickB;

    //相邻判定
    public bool IsFriend()
    {
        if (brickA.X == brickB.X)
            return Mathf.Abs(brickA.Y - brickB.Y) == 1;
        if (brickA.Y == brickB.Y)
            return Mathf.Abs(brickA.X - brickB.X) == 1;

        return false;
    }

    //交换砖块
    public void ExchangeBrick()
    {
        if (!IsFriend())
            return;

        if (!brickA.CanMove() || !brickB.CanMove())
            return;

        //互换地图位置
        int ax = (int)brickA.X, ay = (int)brickA.Y;
        int bx = (int)brickB.X, by = (int)brickB.Y;

        brickMap[ax, ay] = brickB;
        brickMap[bx, by] = brickA;

        //移动
        brickMap[ax, ay].MoveComponent.Move(ax, ay);
        brickMap[bx, by].MoveComponent.Move(bx, by);

        brickA = null;
        brickB = null;

        StartCoroutine(AllFill()); //开启协程
    }
}
