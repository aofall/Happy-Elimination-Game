using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeBtnController : MonoBehaviour
{
    //主界面按钮监听

    public void ClickStartBtn()
    {
        SceneManager.LoadScene(1); //加载场景1
    }

    public void ClickExitBtn()
    {
        Application.Quit(); //退出应用
    }
}
