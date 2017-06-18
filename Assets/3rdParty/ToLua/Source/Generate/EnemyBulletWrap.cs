﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class EnemyBulletWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(EnemyBullet), typeof(BattleObject));
		L.RegFunction("OnCollision", OnCollision);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("speed", get_speed, set_speed);
		L.RegVar("movingBorder", get_movingBorder, set_movingBorder);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnCollision(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			EnemyBullet obj = (EnemyBullet)ToLua.CheckObject(L, 1, typeof(EnemyBullet));
			BattleObject arg0 = (BattleObject)ToLua.CheckUnityObject(L, 2, typeof(BattleObject));
			obj.OnCollision(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_speed(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			EnemyBullet obj = (EnemyBullet)o;
			float ret = obj.speed;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index speed on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_movingBorder(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			EnemyBullet obj = (EnemyBullet)o;
			MovingBorder ret = obj.movingBorder;
			ToLua.PushValue(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index movingBorder on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_speed(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			EnemyBullet obj = (EnemyBullet)o;
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.speed = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index speed on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_movingBorder(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			EnemyBullet obj = (EnemyBullet)o;
			MovingBorder arg0 = (MovingBorder)ToLua.CheckObject(L, 2, typeof(MovingBorder));
			obj.movingBorder = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index movingBorder on a nil value" : e.Message);
		}
	}
}
