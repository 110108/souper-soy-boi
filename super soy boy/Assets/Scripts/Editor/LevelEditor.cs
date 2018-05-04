using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Save level"))
        {
            //Get refrence to level component and set its origin pos and rot
            Level level = (Level)target;
            level.transform.position = Vector3.zero;
            level.transform.rotation = Quaternion.identity;
            //Get a reference to the level Game Object
            var levelRoot = GameObject.Find("Level");
            //create new instances of the classes to store the level data
            var ldr = new LevelDataRepresentation();
            var levelItems = new List<LevelItemRepresentation>();

            foreach(Transform t in levelRoot.transform)
            {
                //Loop thorugh all the child objects in the level object and get a refrence to its sprite render and set its position, scale and rotation
                var sr = t.GetComponent<SpriteRenderer>();
                var li = new LevelItemRepresentation()
                {
                    position = t.position,
                    rotation = t.rotation.eulerAngles,
                    scale = t.localScale
                };
                //Get the child objects name and remove any empty space or numbers from cloned objects
                if(t.name.Contains(" "))
                {
                    li.prefabName = t.name.Substring(0, t.name.IndexOf(" "));
                }
                else
                {
                    li.prefabName = t.name;
                }
                //If teh level object has a sprite render attached get all relevent information from it
                if (sr != null)
                {
                    li.spriteLayer = sr.sortingLayerName;
                    li.spriteColor = sr.color;
                    li.spriteOrder = sr.sortingOrder;
                }
                
                levelItems.Add(li);
            }

            ///convert teh list of level items to an array and store in the level representqatin object
            ldr.levelItems = levelItems.ToArray();
            ldr.playerStartPosition = GameObject.Find("SoyBoy").transform.position;
            //Finds the camera lerp script and saves its settings to the camera settings representive object
            var currentCamSettings = FindObjectOfType<CameraLerpToTransform>();
            if(currentCamSettings != null)
            {
                ldr.cameraSettings = new CameraSettingsRepresentation()
                {
                    cameratrackingTarget = currentCamSettings.camTarget.name,
                    cameraZDepth = currentCamSettings.cameraZDepth,
                    minX = currentCamSettings.minX,
                    minY = currentCamSettings.minY,
                    maxX = currentCamSettings.maxX,
                    maxY = currentCamSettings.maxY,
                    trackingSpeed = currentCamSettings.trackingSpeed
                };
            }

            //Write the data created with the LevelDataRepresentation class to a .json file
            var levelDataToJson = JsonUtility.ToJson(ldr);
            var savePath = System.IO.Path.Combine(Application.persistentDataPath, level.levelName + ".json");
            System.IO.File.WriteAllText(savePath, levelDataToJson);
            Debug.Log("Level saved to " + savePath);
        }
    }
}