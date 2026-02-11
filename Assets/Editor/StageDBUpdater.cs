using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

public class StageDBUpdater
{
    private const string STAGE_FOLDER_PATH = "Assets/Scenes/Stages";

    [MenuItem("Tools/Update Stage Database")]
    static void UpdateDatabaseFromMenu()
    {
        // StageDatabaseを探す
        string[] guids = AssetDatabase.FindAssets("t:StageDatabase");
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("エラー", "StageDatabaseが見つかりません。先に作成してください。", "OK");
            return;
        }

        string dbPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        StageDatabase db = AssetDatabase.LoadAssetAtPath<StageDatabase>(dbPath);

        if (db != null)
        {
            UpdateDatabase(db);
        }
    }

    static void UpdateDatabase(StageDatabase database)
    {
        if (!AssetDatabase.IsValidFolder(STAGE_FOLDER_PATH))
        {
            EditorUtility.DisplayDialog("エラー", $"フォルダが存在しません: {STAGE_FOLDER_PATH}", "OK");
            return;
        }

        // フォルダ内のシーンファイルを検索
        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { STAGE_FOLDER_PATH });

        // 既存のシーン名リストを作成
        HashSet<string> existingSceneNames = new HashSet<string>(
            database.stages.Select(s => s.sceneName)
        );

        int addedCount = 0;
        int skippedCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = Path.GetFileNameWithoutExtension(path);

            // 同名のステージが既に存在する場合はスキップ
            if (existingSceneNames.Contains(sceneName))
            {
                skippedCount++;
                continue;
            }

            // 新しいステージデータを作成
            StageData newStage = new StageData
            {
                sceneName = sceneName,
                scenePath = path
            };

            database.stages.Add(newStage);
            existingSceneNames.Add(sceneName);
            addedCount++;
            EditorUtility.DisplayDialog("シーン追加", $"{sceneName}", "OK");
        }

        // データベースを保存
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        string message = $"追加: {addedCount}個\nスキップ: {skippedCount}個\n合計: {database.stages.Count}個";
        EditorUtility.DisplayDialog("完了", message, "OK");
    }
}