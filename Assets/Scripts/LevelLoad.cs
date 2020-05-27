using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoad : MonoBehaviour
{
    [SerializeField] List<int> levelIndexes;

    [SerializeField] bool preventSameSceneIndexFromLoading = false;

    private Dictionary<int, string> levelIndexToLevelKey = new Dictionary<int, string>();

    private void Awake()
    {
        for (int i = 0; i < levelIndexes.Count; i++)
        {
            int levelIndex = levelIndexes[i];
            if(UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings <= levelIndex || levelIndex < 0)
            {
                levelIndexes.Remove(levelIndex);
            }
            else if (levelIndexToLevelKey.ContainsKey(levelIndex))
            {
                continue;
            }
            else
            {
                levelIndexToLevelKey.Add(levelIndex, (levelIndex + 1).ToString());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        LoadLevelIfNecessary();
    }

    private void LoadLevelIfNecessary()
    {
        foreach (int levelIndex in levelIndexes)
        {
            if(Input.GetKeyDown(levelIndexToLevelKey[levelIndex]))
            {
                    if(preventSameSceneIndexFromLoading && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == levelIndex)
                    {
                        return;
                    }
                    else
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(levelIndex);
                    }
            }
        }
    }
}
