using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Главный скрипт игры, который следит за переключением стейтов игры и инициализацию.
/// 
/// Скрипт сделан по шаблону проектирования Singleton
/// </summary>
public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; } = null;

    public delegate void ChangeState();
    public event ChangeState changeGameModeEvent;

    /// <summary>
    /// Стейты игры. В тестовом задании не используются почти. Но для полноценной игры очень нужны.
    /// </summary>
    public GameMode CurrentGameMode;
    public enum GameMode
    {

        MainMenu,
        Game,
        PauseGame,
        Winner,
        Loose

    }

    /// <summary>
    /// Генерация тиррейна
    /// </summary>
    [Header("Длина тиррейна")]
    [SerializeField]
    private int lengthTerrain = 100;

    [Header("Ширина тиррейна")]
    [SerializeField]
    private int widthTerrain = 100;

    [Header("Высота геометрии (гор/вершин)")]
    [SerializeField]
    private float maxHeight = 5.0f;

    [Header("Частота геометрии (гор/вершин)")]
    [SerializeField]
    [Range(0.02f, 0.3f)]
    private float periodicityHeight = 0.04f;
    private float tempHeightTerrain = 0.0f;

    [Header("Количество предметов окружения.")]
    [SerializeField]
    private int envGameObjectCreate = 50;

    /// <summary>
    /// Объект создающегося тиррейна и все необходимые компоненты
    /// </summary>
    private GameObject terrainGO;

    private Mesh m_Mesh;
    private MeshFilter m_MeshFilter;
    private MeshRenderer m_MeshRenderer;
    private Material defaultTerrainMaterial;
    private MeshCollider m_MeshCollider;

    /// <summary>
    /// Все вершины и триугольники нового меша тиррейна
    /// </summary>
    private List<Vector3> allVertices = new List<Vector3>();
    private List<int> allVertecsTriangles = new List<int>();
    private List<Vector2> allVerticesUV = new List<Vector2>();

    /// <summary>
    /// Игрок
    /// </summary>
    private GameObject playerGO;
    [HideInInspector]
    public Transform playerTR;
    private CharacterController playerCharacterController;
    private FP_Controller playerFP_Controller;
    private Pawn playerPawn;

    /// <summary>
    /// Enemy
    /// </summary>
    /// Списки компонентов, для быстрого доступа к ним. Списки пополняются из авеёка павна врагов.
    [Header("Все появляющиеся враги добавляют свои компоненты в эти списки. И удаляют из них при уничтожении.")]
    public List<GameObject> allEnemyGO = new List<GameObject>();
    public List<Transform> allEnemyTR = new List<Transform>();
    public List<EnemyPawn> allEnemyPawns = new List<EnemyPawn>();

    //Парент для создания врагов, тобы инспектор не засоряли.
    private GameObject enemyParentGO;
    private Transform enemyParentTR;
    private float spawnEnemyTime = 3.0f;
    private float spawnEnemyTimer = 0.0f;

    //Список, чтобы показать, что можно и кучу врагов делать и спавнить.
    private List<GameObject> enemyPrefab = new List<GameObject>();

    private float soundEnemyTime;
    private float soundEnemyTimer = 0.0f;

    private int maxEnemyInScene = 150;

    /// <summary>
    /// Other
    /// </summary>
    private GUI_Manager m_GUI_Manager;

    //Дизайн уровней, список, если бы в игре было много уровней в разной стилистике и разным наполнением. 
    //Сейчас конечно это излишне, это для того, чтобы показать как можно легко увеличить игру
    private List<EnverontmentLevelSO> m_LevelsSO = new List<EnverontmentLevelSO>();
    private int currentLevelNumber = 0;
    private GameObject enverontmentParentGO;
    private Transform enverontmentParentTR;

    private Camera m_Camera;

    private RaycastHit m_Hit;    

    private GameObject tempGO;
    private Vector3 tempVector3;





    private void Awake()
    {

        SearchDestroyCopySingletonOrThisCreateInstance();

        InitializeAndLoadResources();

        CreateTerrain();

        CreateAndSetPositionDifferentLevelEnverontment();

        InitializeAndLoadPlayer();

    }

    /// <summary>
    /// Инициализация и подгрузка объектов для создания тиррейна, материалов для разных коробок и цвета пуль, а также префабы врагов
    /// </summary>
    private void InitializeAndLoadResources()
    {

        terrainGO = new GameObject("Terrain");
        terrainGO.layer = 9;

        m_MeshFilter = terrainGO.AddComponent<MeshFilter>();
        m_MeshRenderer = terrainGO.AddComponent<MeshRenderer>();
        m_Mesh = new Mesh();
        m_Mesh.name = "GenerateMeshTerrain";
        m_MeshFilter.mesh = m_Mesh;
        m_MeshCollider = terrainGO.AddComponent<MeshCollider>();

        m_LevelsSO.Add(Resources.Load("SO/Levels/Level01") as EnverontmentLevelSO);

        defaultTerrainMaterial = m_LevelsSO[currentLevelNumber].TerrainMaterial;
        m_MeshRenderer.material = defaultTerrainMaterial;
        m_MeshRenderer.sharedMaterial.SetTextureScale("_MainTex", new Vector2(1 / (float)widthTerrain, 1 / (float)lengthTerrain));      
        
        enemyParentGO = new GameObject("EnemyParent");
        enemyParentTR = enemyParentGO.transform;

        enemyPrefab.Add(Resources.Load("Pawn/Zombie") as GameObject);
        enemyPrefab.Add(Resources.Load("Pawn/ZombieTwo") as GameObject);
        enemyPrefab.Add(Resources.Load("Pawn/ZombieThree") as GameObject);

        soundEnemyTime = Random.Range(4.0f, 10.0f);

    }

    private void InitializeAndLoadPlayer()
    {
        
        tempGO = Resources.Load("Pawn/Player") as GameObject;
        playerGO = Instantiate(tempGO, new Vector3(lengthTerrain / 2,  maxHeight, widthTerrain / 2), Quaternion.identity);             

        playerCharacterController = playerGO.GetComponentInChildren<CharacterController>();
        playerFP_Controller = playerGO.GetComponentInChildren<FP_Controller>();
        playerPawn = playerGO.GetComponentInChildren<Pawn>();
        playerTR = playerPawn.gameObject.GetComponent<Transform>();
        playerFP_Controller.walkSpeed = playerPawn.m_PawnSO.Speed;
        playerFP_Controller.runSpeed = playerPawn.m_PawnSO.Speed * 2.0f;

        playerTR.position = new Vector3(lengthTerrain / 2, maxHeight + playerCharacterController.height, widthTerrain / 2);

        m_GUI_Manager = FindObjectOfType<GUI_Manager>();
        m_Camera = Camera.main;

        CurrentGameMode = GameMode.Game;

    }

    private void Update()
    {
        
        switch (CurrentGameMode)
        {

            case GameMode.Game:
                CreateEnemy();
                PlayEnemyArrrghh();
                KeyBoardHack();
                break;

        }

    }
    /// <summary>
    /// Смена стейта игры. В тестовом задании излишне, сделано, чтобы показать как обячно работаю с режимами игры.
    /// </summary>
    /// <param name="m_GameMode">Новый игровой режим.</param>
    public void ChangeGameMode(GameMode m_GameMode)
    {

        CurrentGameMode = m_GameMode;

        changeGameModeEvent();

        switch (m_GameMode)
        {

            case GameMode.Game:

                break;

            case GameMode.MainMenu:

                break;

            case GameMode.PauseGame:

                break;

            case GameMode.Loose:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;

            case GameMode.Winner:

                break;

        }

    }
    /// <summary>
    /// Спавн врагов за пределами поля зрения персонажа.
    /// 
    /// Можно было бы сделать и пул врагов. Но это я реализовал уже с пулями (HeroPawn).
    /// Здесь показал, что и так просто можно сделать.
    /// </summary>
    private void CreateEnemy()
    {

        if (allEnemyGO.Count < maxEnemyInScene)
        {

            if (spawnEnemyTimer < spawnEnemyTime) spawnEnemyTimer += Time.deltaTime;
            else
            {

                spawnEnemyTimer = 0.0f;

                //Вообще-то, что бы просто не было появления в не поля зрения хватит и половины угла зрения, но так больше.
                //Ведь половина угла зрения - это как раз максимальный угол между вектором вперёд и краем камеры.
                do
                {

                    tempVector3 = new Vector3(Random.Range(0, lengthTerrain), maxHeight, Random.Range(0, widthTerrain));

                } while (Vector3.Angle(playerTR.forward, (tempVector3 - playerTR.position)) < m_Camera.fieldOfView);
                
                Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Count - 1)], tempVector3, Quaternion.identity, enemyParentTR);

            }

            //Передаём аппроксимизированое значение в параметр музыки. Так как у нас с каждыми 20-30 новыми зомбями на сцене будет меняться музыка
            SoundAndMusic.Instance.SetMusicParameter((float)allEnemyGO.Count / (float)(maxEnemyInScene / 3));

        }

    }
    /// <summary>
    /// Процедурное создание тиррейна из вершин, треугольников и полигонов. 
    /// По заданным параметрам - ширины и длины, а также максимальной высоты холмов.
    /// </summary>
    private void CreateTerrain()
    {
        // Формируем сетку вершин полигонов. Высоту считаем специальным методом создания шума. Чтобы как в пустыне холмистый тиррейн получился.
        for (int i = 0; i <= lengthTerrain; i++)
        {

            for (int j = 0; j <= widthTerrain; j++)
            {

                tempHeightTerrain = Mathf.PerlinNoise(j * periodicityHeight, i * periodicityHeight) * maxHeight;

                allVertices.Add(new Vector3(j, tempHeightTerrain, i));
                allVerticesUV.Add(new Vector2(j, i));

            }

        }

        // Создаём и добавляем в правильной последовательности точки, которые образуют треугольники, и как следсткие - полигоны.
        for (int count = 0, i = 0; i < lengthTerrain; i++)
        {

            for (int j = 0; j < widthTerrain; j++)
            {

                allVertecsTriangles.Add(count);
                allVertecsTriangles.Add(count + widthTerrain + 1);
                allVertecsTriangles.Add(count + 1);
                allVertecsTriangles.Add(count + 1);
                allVertecsTriangles.Add(count + widthTerrain + 1);
                allVertecsTriangles.Add(count + widthTerrain + 2);

                count++;

            }

            count++;

        }

        // Создаём мешь на основе созданных списков вершин.
        m_Mesh.Clear();
        m_Mesh.vertices = allVertices.ToArray();
        m_Mesh.uv = allVerticesUV.ToArray();
        m_Mesh.triangles = allVertecsTriangles.ToArray();        
        m_MeshCollider.sharedMesh = m_Mesh;

        m_Mesh.RecalculateBounds();
        m_Mesh.RecalculateNormals();
        m_Mesh.Optimize();

    }

    /// <summary>
    /// Метод создаёт в случайном месте карты объекты из SO списка объектов нужного уровня.
    /// Т.е. можно используя разные SO по одному методу создавать совершенно разные по стилистике уровни.
    /// </summary>
    private void CreateAndSetPositionDifferentLevelEnverontment()
    {

        enverontmentParentGO = new GameObject("Enverontment Parent");
        enverontmentParentTR = enverontmentParentGO.transform;

        for (int i = 0; i < envGameObjectCreate; i++)
        {

            // Вычисляем случайную точку
            tempVector3 = new Vector3(Random.Range(0, lengthTerrain), maxHeight * 3, Random.Range(0, widthTerrain));

            // И проверяем, чтобы на этом месте уже не было другого объекта.
            do
            {

                Physics.Raycast(tempVector3, Vector3.down, out m_Hit, maxHeight * 5);

            } while (m_Hit.collider != null && m_Hit.collider.gameObject.name != "Terrain");

            tempGO = Instantiate(m_LevelsSO[currentLevelNumber].EnverontmentPrefabs[Random.Range(0, m_LevelsSO[currentLevelNumber].EnverontmentPrefabs.Length)], m_Hit.point, Quaternion.identity, enverontmentParentTR);

        }

    }

    private void SearchDestroyCopySingletonOrThisCreateInstance()
    {

        if (Instance)
        {

            DestroyImmediate(gameObject);
            return;

        }

        Instance = this;

    }
    /// <summary>
    /// Разнообразные методы доступа к приватным переменным. 
    /// Собраны самые важные, которые нельзя заменять.
    /// </summary>
    /// <returns></returns>
    public Pawn GetPlayerPawn() { return playerPawn; }

    public Vector3 GetPlayerPosition() { return playerTR.position; }

    public GUI_Manager GetGUI_Manager() { return m_GUI_Manager; }

    public float GetHeightTerrain() { return maxHeight; }

    public GameObject GetCameraGO() { return m_Camera.gameObject; }

    public Camera GetCameraMain() { return m_Camera; }

    /// <summary>
    /// Во имя оптимизации, чтобы не GetComponent'ить во время выполнения программы, при столкновении, искать в списке вргаов объект, в который попал снаряд.
    /// Если находим, то у нас есть прохешированные павн и аи врага, вызываем нужный метод сразу.
    /// </summary>
    /// <param name="enemyGO">Объект, в который попали.</param>
    /// <param name="bulletDamage">Урон пули на данный момент.</param>
    public void PlayerBulletHitInEnemy(GameObject enemyGO, float bulletDamage)
    {
        
        for(int i = 0; i < allEnemyGO.Count; i++)
        {

            if (allEnemyGO[i] == enemyGO)
            {

                allEnemyPawns[i].TakeDamage(bulletDamage);
                break;

            }

        }

        //Можно было и так поиск проводить. Но LInq тут не надёжно работал, кидал иногда ошибки. Это здесь оставил, чтобы показать, что предыдущую запись и так можно было сделать.
        //allEnemyPawns[allEnemyGO.FindIndex(x => x.gameObject == enemyGO)].TakeDamage(bulletDamage);

    }

    /// <summary>
    /// Метод воспроизведения рычания зомби через случайное количество времени. 
    /// Зомби тоже выбирается случайно. И так как звук в эфмоде настроен на расстояние, то и звуки тогда будут приходить с разных сторон и с разной громкостью.
    /// Какой-то зомби рычит издалека, а какой-то совсем рядом - за спиной.
    /// </summary>
    private void PlayEnemyArrrghh()
    {

        if (CurrentGameMode == GameMode.Game)
        {

            if (soundEnemyTimer < soundEnemyTime) soundEnemyTimer += Time.deltaTime;
            else
            {

                soundEnemyTime = Random.Range(4.0f, 10.0f);
                soundEnemyTimer = 0.0f;

                if (allEnemyGO.Count > 0) SoundAndMusic.Instance.PlayEnemyWalkVoiced(allEnemyGO[Random.Range(0, allEnemyGO.Count - 1)]);

            }

        }

    }

    private void KeyBoardHack()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeGameMode(GameMode.Loose);
        if (Input.GetKeyDown(KeyCode.Alpha4)) playerPawn.TakeDamage(20);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SoundAndMusic.Instance.PlayEnemyWalkVoiced(allEnemyGO[allEnemyGO.Count - 1]);

        if (Input.GetKeyDown(KeyCode.Alpha0)) SoundAndMusic.Instance.SetMusicParameter(0.30f);
        if (Input.GetKeyDown(KeyCode.Alpha1)) SoundAndMusic.Instance.SetMusicParameter(0.40f);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SoundAndMusic.Instance.SetMusicParameter(0.70f);
        */


        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

    }

}
