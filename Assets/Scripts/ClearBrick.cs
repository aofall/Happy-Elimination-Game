using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBrick : MonoBehaviour
{
    //清除方块相关

    public AnimationClip clearAnimation;
    private Animator animator;

    public bool isClear;
    protected GameBrick brick;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        brick = GetComponent<GameBrick>();
    }

    public void Clear()
    {
        if(!isClear)
            StartCoroutine(ClearCoroutine());

        isClear = true;
    }

    private IEnumerator ClearCoroutine()
    {
        if (animator != null)
        {
            animator.Play(clearAnimation.name);
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(gameObject);
        }
    }
}
