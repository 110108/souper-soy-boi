using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour {

    public string playerName;
    public static GameManager instance;
    public GameObject buttonPrefab;
    private string selectedLevel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        DiscoverLevels();
        SceneManager.sceneLoaded += OnSceneLoaded;
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public void RestartLevel(float delay)
    {
        StartCoroutine(RestartLevelDelay(delay));
    }

    private IEnumerator RestartLevelDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Game");
    }

    public List<PlayerTimeEntry> LoadPreviousTimes()
    {
        //Find the file of saved times and use a binary formatted to deserialize the data from the save file and return the list of entries
        try
        {
            var levelName = Path.GetFileName(selectedLevel);
            var scoresFile = Application.persistentDataPath + "/" + playerName + "_" + levelName + "_times.dat";

            using (var stream = File.Open(scoresFile, FileMode.Open))
            {
                var bin = new BinaryFormatter();
                var times = (List<PlayerTimeEntry>)bin.Deserialize(stream);
                return times;
            }
        }

        //If file deserialization of the saved times file failes log the error and return an empty list to store times
        catch (IOException ex)
        {
            Debug.Log("Could not load previous times for: " + playerName + ". Exception: " + ex.Message);
            return new List<PlayerTimeEntry>();
        }

    }

    public void SaveTime(decimal time)
    {
        //Load any old saved times
        var times = LoadPreviousTimes();
        
        //Create a new entry and update its values to be saved
        var newTime = new PlayerTimeEntry();
        newTime.entryDate = DateTime.Now;
        newTime.time = time;

        //Save the data to the existing save file or create a new one using a binary formatter
        var bFormatter = new BinaryFormatter();
        var levelName = Path.GetFileName(selectedLevel);
        var filePath = Application.persistentDataPath + "/" + playerName + "_" + levelName + "_times.dat";
        using (var file = File.Open(filePath, FileMode.Create))
        {
            times.Add(newTime);
            bFormatter.Serialize(file, times);
        }
    }

    public void DisplayPreviousTimes()
    {
        //Load the previous times
        var times = LoadPreviousTimes();
        var levelName = Path.GetFileName(selectedLevel);
        if (levelName != null)
        {
            levelName = levelName.Replace(".json", "");
        }

        //Use a LINQ query to sort the previouus times from fastest to slowest and then take the fiorst three entries
        var topThree = times.OrderBy(time => time.time).Take(3);
        var timesLabel = GameObject.Find("PreviousTimes")
        .GetComponent<Text>();
        timesLabel.text = levelName + "\n";
        timesLabel.text += "BEST TIMES \n";
        foreach (var time in topThree)
        {
            timesLabel.text += time.entryDate.ToShortDateString()
            + ": " + time.time + "\n";
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //Load the selected level when the game level is loaded
        if (!string.IsNullOrEmpty(selectedLevel) && scene.name == "Game")
        {
            Debug.Log("Loading level content for: " + selectedLevel);
            LoadLevelContent();
            DisplayPreviousTimes();
        }
        //Load the level files if the menu is loaded
        if (scene.name == "Menu")
        {
            DiscoverLevels();
        }
    }

    private void SetLevelName(string levelFilePath)
    {
        selectedLevel = levelFilePath;
        SceneManager.LoadScene("Game");
    }

    private void DiscoverLevels()
    {
        //Get a reference to the rect transform that will hold the buttons to load each level
        var levelPanelRectTransform = GameObject.Find("LevelItemsPanel").GetComponent<RectTransform>();
        //Get a refrence to any JSON files
        var levelFiles = Directory.GetFiles(Application.persistentDataPath, "*.json");

        Debug.Log(levelFiles.Length.ToString());

        var yOffset = 0f;
        for(var i = 0; i < levelFiles.Length; i++)
        {
            //Set the offset for the buttons position in the rect tranfsform
            if(i == 0)
            {
                yOffset = -30f;
            }
            else
            {
                yOffset -= 65f;
            }
            //Get the name of the current file
            var levelFile = levelFiles[i];
            var levelName = Path.GetFileName(levelFile);
            //Spawn a new button instance
            var levelButtonObj = (GameObject)Instantiate(buttonPrefab, Vector2.zero, Quaternion.identity);
            //Make the spawned button a child of the panel and gets its transform
            var levelButtonRectTransform = levelButtonObj.GetComponent<RectTransform>();
            levelButtonRectTransform.SetParent(levelPanelRectTransform, true);
            //Set the position of the button
            levelButtonRectTransform.anchoredPosition = new Vector2(212.5f, yOffset);
            //Change the button text to the name of the level
            var levelButtonText = levelButtonObj.transform.GetChild(0).GetComponent<Text>();
            levelButtonText.text = levelName;
            //Use delegate to call SetLevelName and pass in different values
            var levelButton = levelButtonObj.GetComponent<Button>();
            levelButton.onClick.AddListener(delegate { SetLevelName(levelFile); });
            //Expand the size of the panel to fit all the buttons
            levelPanelRectTransform.sizeDelta = new Vector2(levelPanelRectTransform.sizeDelta.x, 60f * i);
        }
        //Reset the panels scroll value
        levelPanelRectTransform.offsetMax = new Vector2(levelPanelRectTransform.offsetMax.x, 0f);
    }

    private void LoadLevelContent()
    {
        //Delete the old level object and its children
        var existingLevelRoot = GameObject.Find("Level");
        Destroy(existingLevelRoot);
        //Create a new level object to hold the new level layout children
        var levelRoot = new GameObject("Level");

        //Read the json file into a levelDataRepresentation class object
        var levelFileJsonContent = File.ReadAllText(selectedLevel);
        var levelData = JsonUtility.FromJson<LevelDataRepresentation>(levelFileJsonContent);
        //Loop through all of the level items in the object
        foreach (var li in levelData.levelItems)
        {
            //Get the items prefab object
            var pieceResource = Resources.Load("Prefabs/" + li.prefabName);
            if(pieceResource == null)
            {
                Debug.Log("Could not find object: " + li.prefabName);
            }
            //Spawn the item into the level and update its sprite render settings if it has one
            var piece = (GameObject)Instantiate(pieceResource, li.position, Quaternion.identity);
            var pieceSprite = piece.GetComponent<SpriteRenderer>();
            if(pieceSprite != null)
            {
                pieceSprite.sortingOrder = li.spriteOrder;
                pieceSprite.sortingLayerName = li.spriteLayer;
                pieceSprite.color = li.spriteColor;
            }
            //Set the objects parent and positioning
            piece.transform.parent = levelRoot.transform;
            piece.transform.position = li.position;
            piece.transform.rotation = Quaternion.Euler(li.rotation.x, li.rotation.y, li.rotation.z);
            piece.transform.localScale = li.scale;
        }
        var SoyBoy = GameObject.Find("SoyBoy");
        SoyBoy.transform.position = levelData.playerStartPosition;
        Camera.main.transform.position = new Vector3(SoyBoy.transform.position.x, SoyBoy.transform.position.y, Camera.main.transform.position.z);
        //Get the camera follow script
        var camSettings = FindObjectOfType<CameraLerpToTransform>();
        //Update the cameras settings
        if (camSettings != null)
        {
            camSettings.cameraZDepth =
            levelData.cameraSettings.cameraZDepth;
            camSettings.camTarget = GameObject.Find(
            levelData.cameraSettings.cameratrackingTarget).transform;
            camSettings.maxX = levelData.cameraSettings.maxX;
            camSettings.maxY = levelData.cameraSettings.maxY;
            camSettings.minX = levelData.cameraSettings.minX;
            camSettings.minY = levelData.cameraSettings.minY;
            camSettings.trackingSpeed = levelData.cameraSettings.trackingSpeed;
        }
    }
}