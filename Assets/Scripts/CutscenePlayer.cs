using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutscenePlayer : MonoBehaviour
{
    private PlayableDirector _timeline;
    
    public GameObject cutsceneElements;

    public List<GameObject> neededInCutscene;

    public List<GameObject> objectsToDestroy;
    public List<GameObject> objectsNotToSpawn;
    
    // Start is called before the first frame update
    void Start()
    {
        _timeline = GetComponent<PlayableDirector>();

        foreach (var objectToDestroy in objectsToDestroy)
        {
            Destroy(objectToDestroy);
        }
        
        foreach (var objectInScene in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (objectInScene.transform.parent == null)
            {
                if (neededInCutscene.Contains(objectInScene) || objectInScene == cutsceneElements)
                {
                    objectInScene.SetActive(true);
                }
                else if (objectInScene != gameObject)
                {
                    objectInScene.SetActive(false);
                }
            }
                
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CutsceneEnd()
    {
        Destroy(cutsceneElements);

        foreach (var objectInScene in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (objectInScene.transform.parent == null 
                && !objectInScene.activeInHierarchy 
                && !objectInScene.CompareTag("Cutscene")
                && !objectsNotToSpawn.Contains(objectInScene))
            {
                objectInScene.SetActive(true);
            }
        }
        
        Destroy(gameObject);
    }
}
