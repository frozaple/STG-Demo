using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEditor;

public class SpawnEnemyBehaviour : LoopBehaviour
{
    public string enemyPrefab;
    public Vector2 spawnPosition;
    public Vector2 spawnPositionLoopDelta;

    protected override void OnLoopCompleteOnce()
    {
        SpawnEnemy(spawnPosition + loopCount * spawnPositionLoopDelta);
    }

    private void SpawnEnemy(Vector3 pos)
    {
        GameObject enemy = BattleStageManager.Instance.SpawnObject(enemyPrefab);
        enemy.transform.localPosition = pos;
    }
}

[System.Serializable]
public class SpawnEnemy : PlayableAsset
{
    public float loopGap;
    public string enemyPrefab;
    public Vector2 spawnPosition;
    public Vector2 spawnPositionLoopDelta;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<SpawnEnemyBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.loopGap = loopGap;
        behaviour.enemyPrefab = enemyPrefab;
        behaviour.spawnPosition = spawnPosition;
        behaviour.spawnPositionLoopDelta = spawnPositionLoopDelta;
        return playable;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpawnEnemy))]
public class SpawnEnemyEditor : Editor
{
    SerializedProperty loopGap;
    SerializedProperty enemyPrefab;
    SerializedProperty spawnPosition;
    SerializedProperty spawnPositionLoopDelta;

    GameObject prefab;

    private void Awake()
    {
        loopGap = serializedObject.FindProperty("loopGap");
        enemyPrefab = serializedObject.FindProperty("enemyPrefab");
        spawnPosition = serializedObject.FindProperty("spawnPosition");
        spawnPositionLoopDelta = serializedObject.FindProperty("spawnPositionLoopDelta");
    }

    public override void OnInspectorGUI()
    {
        GameObject newPrefab = EditorGUILayout.ObjectField("Enemy Prefab", prefab, typeof(GameObject), false) as GameObject;
        if (newPrefab != prefab && newPrefab.GetComponent<Enemy>() != null)
        {
            prefab = newPrefab;
            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            int startIndex = prefabPath.IndexOf("Resources/") + 10;
            int endIndex = prefabPath.IndexOf(".prefab");
            enemyPrefab.stringValue = prefabPath.Substring(startIndex, endIndex - startIndex);
        }

        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField(enemyPrefab.stringValue);
        EditorGUI.indentLevel--;

        spawnPosition.vector2Value = EditorGUILayout.Vector2Field("Spawn Position", spawnPosition.vector2Value);

        bool loop = loopGap.floatValue > 0;
        loop = EditorGUILayout.Toggle("Loop", loop);
        if (loop)
        {
            float minGap = 1.0f / 60;
            loopGap.floatValue = EditorGUILayout.Slider("Loop Gap", loopGap.floatValue, minGap, 100.0f);
            spawnPositionLoopDelta.vector2Value = EditorGUILayout.Vector2Field("Loop Position Delta", spawnPositionLoopDelta.vector2Value);
        }
        else
        {
            loopGap.floatValue = -1f;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
