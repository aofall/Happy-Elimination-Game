using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBrick : MonoBehaviour
{
    //移动方块相关

    private GameBrick brick;

    private void Awake()
    {
        brick = GetComponent<GameBrick>();
    }

    private IEnumerator moveCoroutine;
    public void Move(int nx, int ny)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = MoveCoroutine(nx, ny, GameManager.Instance.fillTime);
        StartCoroutine(moveCoroutine);
    }
    public IEnumerator MoveCoroutine(int nx, int ny, float time)
    {
        brick.X = nx;
        brick.Y = ny;

        Vector3 startPos = transform.position;
        Vector3 endPos = GameManager.Instance.FixXY(nx, ny);

        for(float t = 0; t < time; t += Time.deltaTime)
        {
            brick.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }

        brick.transform.position = endPos; //安全检测 强制归位
    }
}
