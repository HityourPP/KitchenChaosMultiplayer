using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenu,
        LoadingScene,
        GameScene,
        LobbyScene,
        CharacterSelectScene
    }

    public static Scene targetScene; //静态类只能包含静态对象

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());  //先进入到加载场景
    }
    /// <summary>
    /// 多人模式下加载场景
    /// </summary>
    public static void LoadNetwork(Scene targetScene)
    {   //LoadSceneMode.Single为单模式加载场景，加载的场景独立显示出来
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallBack()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
