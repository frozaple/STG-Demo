using LuaInterface;
using UnityEngine;
using System.Collections.Generic;

class BattleScriptManager
{
    private LuaState luaState;
    private LuaFunction updateFunc;
    private LuaFunction despawnFunc;

    public void Init()
    {
        InitLoader();
        luaState = new LuaState();
        OpenLibs();
        luaState.LuaSetTop(0);
        Bind();
        StartMain();
        StartFunc();
    }

    private LuaFileUtils InitLoader()
    {
        return new LuaResLoader();
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

    private void StartFunc()
    {
        updateFunc = luaState.GetFunction("Update");
        despawnFunc = luaState.GetFunction("Despawn");
    }

    public void Update()
    {
        updateFunc.BeginPCall();
        updateFunc.Push(Time.timeScale);
        updateFunc.PCall();
        updateFunc.EndPCall();
    }

    public void Despawn(GameObject obj)
    {
        despawnFunc.BeginPCall();
        despawnFunc.Push(obj.GetInstanceID());
        despawnFunc.PCall();
        despawnFunc.EndPCall();
    }

    public void Dispose()
    {
        updateFunc.Dispose();
        updateFunc = null;
        despawnFunc.Dispose();
        despawnFunc = null;

        luaState.Dispose();
        luaState = null;
    }
}
