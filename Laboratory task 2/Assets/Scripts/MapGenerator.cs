using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : Singleton<MapGenerator> 
{
    int mapSize;
    int itemSpace = 7;
    int itemCountInMap = 5;
    int coinsCountInItem = 10;
    public float laneOffset = 2.5f;
    float coinsHeight = 0.5f;
  
    enum TrackPos { Left = -1, Center = 0, Right = 1 };
    enum CoinsStyle { Line, Jump, Ramp };

    public GameObject ObstacleTopPrefab;
    public GameObject Rock1Prefab;
    public GameObject Rock2Prefab;
    public GameObject StumpPrefab;
    public GameObject SprucePrefab;
    public GameObject Tree1Prefab;
    public GameObject Tree2Prefab;
    public GameObject RampPrefab;
    public GameObject CoinPrefab;

    public List<GameObject> maps = new List<GameObject>();
    public List<GameObject> activeMaps = new List<GameObject>();
    public List<GameObject> obstacles = new List<GameObject>();

    struct MapItem
    {
        public void SetValues(GameObject obstacle, int trackPos, CoinsStyle coinsStyle)
        {
            this.obstacle = obstacle;
            this.trackPos = trackPos;
            this.coinsStyle = coinsStyle;
        }
        public GameObject obstacle;
        public int trackPos;
        public CoinsStyle coinsStyle;

    }

    private void Awake()
    {
        mapSize = itemCountInMap * itemSpace;
        for (int i = 0; i <= 100; i++)
        {
            maps.Add(MakeMap1());
        }
        foreach (GameObject map in maps)
        {
            map.SetActive(false);
        }
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (RoadGenerator.Instance.speed == 0)
            return;

        foreach (GameObject map in activeMaps)
        {
            map.transform.position -= new Vector3(0, 0, RoadGenerator.Instance.speed * Time.deltaTime);
        }

        if (activeMaps[0].transform.position.z < -mapSize)
        {
            RemoveFirstActiveMap();
            AddActiveMap();
        }
    }

    void RemoveFirstActiveMap()
    {
        activeMaps[0].SetActive(false);
        maps.Add(activeMaps[0]);
        activeMaps.RemoveAt(0);
    }

    public void ResetMaps()
    {
        while (activeMaps.Count > 0)
        {
            RemoveFirstActiveMap();
        }
        AddActiveMap();
        AddActiveMap();
    }

    void AddActiveMap()
    {
        int r = Random.Range(0, maps.Count);
        GameObject go = maps[r];
        go.SetActive(true);
        foreach (Transform child in go.transform)
        {
            child.gameObject.SetActive(true);
        }

        go.transform.position = activeMaps.Count > 0 ?
                                activeMaps[activeMaps.Count - 1].transform.position + Vector3.forward * mapSize :
                                new Vector3(0, 0, 10);

        maps.RemoveAt(r);
        activeMaps.Add(go);
    }
    
    class Obstacle
    {
        public Obstacle(string name, GameObject prefab, CoinsStyle coinsStyle)
        {
            Name = name;
            ObstaclePrefab = prefab;
            CoinsStyle = coinsStyle;
        }
        public string Name { get; set; }
        public GameObject ObstaclePrefab { get; set; }
        public CoinsStyle CoinsStyle { get; set; }

    }

    GameObject MakeMap1()
    {
        List<Obstacle> obstacles = new List<Obstacle>()
        {
        new Obstacle("RampPrefab", RampPrefab, CoinsStyle.Ramp),
        new Obstacle("Rock1Prefab", Rock1Prefab, CoinsStyle.Jump),
        new Obstacle("Rock2Prefab", Rock2Prefab, CoinsStyle.Jump),
        new Obstacle("Tree1Prefab", Tree1Prefab, CoinsStyle.Line),
        new Obstacle("Tree2Prefab", Tree2Prefab, CoinsStyle.Line),
        new Obstacle("SprucePrefab", SprucePrefab, CoinsStyle.Line),
        new Obstacle("StumpPrefab", StumpPrefab, CoinsStyle.Jump)
        };

        GameObject result = new GameObject("Map1");
        result.transform.SetParent(transform);
        MapItem item = new MapItem();
        int rndObstacle;
        int rndPos;

        for (int i = 0; i < itemCountInMap; i++)
        {
            rndObstacle = Random.Range(0, obstacles.Count);
            rndPos = Random.Range(-1, 2);

            item.SetValues(obstacles[rndObstacle].ObstaclePrefab, rndPos, obstacles[rndObstacle].CoinsStyle);

            Vector3 obstaclePos = new Vector3((int)item.trackPos * laneOffset, 0, i * itemSpace);
            CreateCoins(item.coinsStyle, obstaclePos, result);

            if (item.obstacle != null)
            {
                GameObject go = Instantiate(item.obstacle, obstaclePos, Quaternion.identity);
                go.transform.SetParent(result.transform);
            }
        }
        return result;
    }

    void CreateCoins(CoinsStyle style, Vector3 pos, GameObject parentObject)
    {
        Vector3 coinPos = Vector3.zero;
        if (style == CoinsStyle.Line)
        {
            for (int i = -coinsCountInItem / 2; i < coinsCountInItem / 2; i++)
            {
                coinPos.y = coinsHeight;
                coinPos.z = i * ((float)itemSpace / coinsCountInItem);
                GameObject go = Instantiate(CoinPrefab, coinPos + pos, Quaternion.identity);
                go.transform.SetParent(parentObject.transform);
            }
        }
        else if (style == CoinsStyle.Jump)
        {
            for (int i = -coinsCountInItem / 2; i < coinsCountInItem / 2; i++)
            {
                coinPos.y = Mathf.Max(-1 / 2f * Mathf.Pow(i, 2) + 3, coinsHeight);
                coinPos.z = i * ((float)itemSpace / coinsCountInItem);
                GameObject go = Instantiate(CoinPrefab, coinPos + pos, Quaternion.identity);
                go.transform.SetParent(parentObject.transform);
            }
        }
        else if (style == CoinsStyle.Ramp)
        {
            for (int i = -coinsCountInItem / 2; i < coinsCountInItem / 2; i++)
            {
                coinPos.y = Mathf.Min(Mathf.Max(0.7f * (i + 2), coinsHeight), 3.0f);
                coinPos.z = i * ((float)itemSpace / coinsCountInItem);
                GameObject go = Instantiate(CoinPrefab, coinPos + pos, Quaternion.identity);
                go.transform.SetParent(parentObject.transform);
            }
        }
    }
}