using LuaInterface;
using UnityEngine;
using System.Collections.Generic;

class BattleScriptManager
{
    private LuaState luaState;
    private LuaFunction updateFunc;

    public void Init()
    {
        InitLoader();
        luaState = new LuaState();
        OpenLibs();
        luaState.LuaSetTop(0);
        Bind();
        StartMain();
        StartUpdate();
    }

    private LuaFileUtils InitLoader()
    {
        if (LuaFileUtils.Instance != null)
            return LuaFileUtils.Instance;

        return null;
    }

    private void OpenLibs()
    {
        luaState.OpenLibs(LuaDLL.luaopen_pb);
        luaState.OpenLibs(LuaDLL.luaopen_struct);
        luaState.OpenLibs(LuaDLL.luaopen_lpeg);
    }

    private void Bind()
    {
        LuaBinder.Bind(luaState);
    }

    private void StartMain()
    {
        luaState.Start();
        luaState.DoFile("main.lua");
        LuaFunction main = luaState.GetFunction("Main");
        main.Call();
        main.Dispose();
        main = null;
    }

    private void StartUpdate()
    {
        updateFunc = luaState.GetFunction("Update");
    }

    public void Update()
    {
        updateFunc.BeginPCall();
        updateFunc.Push(Time.timeScale);
        updateFunc.PCall();
        updateFunc.EndPCall();
    }

    public void Dispose()
    {
        updateFunc.Dispose();
        updateFunc = null;

        luaState.Dispose();
        luaState = null;
    }
}
