using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Синглтон, отвечающий за всё музыкальное сопровождение. Музыку и звуки задаёт автоматически в соответствии со списком уровней и привязанных
/// к ним звуков. Именно синглтон нужен, так как FMOD умеет не прерывать музыку между загрузками сцен, а одиночка как раз и следит за тем, 
/// надо ли переключать композициию.
/// 
/// Доступ из любого скрипта проекта -> SoundAndMusic.Instance. <-
/// 
/// </summary> 
public class SoundAndMusic : MonoBehaviour
{

    private static SoundAndMusic _instance = null;

    private VCA MusicVCA;
    private VCA FXVCA;
    private float volumeMusicVCA;
    private float volumeFXVCA;

    private GameObject m_CameraGO;
    
    /// <summary>
    /// /////////////////////////// = Sound And Music path in FMOD = /////////////////////////////////////
    /// </summary>   

    /// <summary>
    /// Ниже идёт список эвентов FMOD. А так же индивидуальные эвенты.
    /// </summary>
    //Melody
    private string gameMusic = "event:/MusicGame";

    //Game - Variable
    //Player
    private string playerShoot =        "event:/Player/PlayerShoot";
    private string playerDamage =       "event:/Player/PlayerDamage";
    private string playerFootSteps =    "event:/Player/PlayerFootStep";
    private string playerJump =         "event:/Player/PlayerJump";

    //Enemy
    private string enemyAttack =    "event:/Enemy/EnemyAttack";
    private string enemyDamage =    "event:/Enemy/EnemyDamage";
    private string enemyDie =       "event:/Enemy/EnemyDie";
    private string enemyWalkVoiced = "event:/Enemy/EnemyWalkVoiced";

    //FX
    private string psycho = "event:/PSYCHO";
    
    //FMOD Events
    private EventInstance musicEvent;
    private EventDescription musicDes;
    private PARAMETER_DESCRIPTION musicPD;
    private PARAMETER_ID musicPiD;
    private string tempPath;
    
       

    /// <summary>
    /// /////////////////////////// = Initializations = /////////////////////////////////////
    /// </summary>
    void Awake()
    {

        if (_instance == null)
        {

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
                        
        }
        else
        {

            Destroy(gameObject);

        }

    }
    void Start() { InitializeManager(); }
    private void InitializeManager()
    {

        MusicVCA = RuntimeManager.GetVCA("vca:/Music");
        FXVCA = RuntimeManager.GetVCA("vca:/FX");
        SetFXVolume(1.0f);
        SetMusicVolume(1.0f);

        CheckCamera();
                
        ChangePlayingMusic();

    }

    /// <summary>
    /// Так как в каждой сцене своя камера, то нужно следить, чтобы на неё всегда была ссылка. 
    /// Из камеры должны звучать все звуки и мелодии связанные с интерфейсом и т.п.
    /// </summary>
    public void CheckCamera()
    {

        if (GameManager.Instance == null) m_CameraGO = GameManager.Instance.GetCameraGO();
        else m_CameraGO = gameObject;

    }

    private void ChangePlayingMusic()
    {

        if (GetMusicPath() != null) StopMusic();

        musicEvent = RuntimeManager.CreateInstance(gameMusic);
        musicEvent.set3DAttributes(RuntimeUtils.To3DAttributes(m_CameraGO));
        musicEvent.start();

        musicDes = RuntimeManager.GetEventDescription(gameMusic);
        musicDes.getParameterDescriptionByName("ChangeTrack", out musicPD);
        musicPiD = musicPD.id;

    }

    public void StopMusic()
    {

        musicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

    }

    public string GetMusicPath()
    {

        musicEvent.getDescription(out musicDes);
        musicDes.getPath(out tempPath);
        return tempPath;

    }

    /// <summary>
    /// /////////////////////////// = Sound And Music Methods = /////////////////////////////////////
    /// 
    /// Общедоступные методы для вызова их через синглтон. 
    /// Здесь и надо создавать новые методы для новых звуков в игре.
    /// Заделены по зонам ответственности для более лёгкой навигаии.
    /// 
    /// </summary>

    ///////////// = GAME = ///////////////
    public void PlayGamePSYCHO(GameObject forFMOD) { RuntimeManager.PlayOneShotAttached(psycho, forFMOD); }


    //////// = Player = //////////
    public void PlayPlayerShoot(GameObject forFMOD) { RuntimeManager.PlayOneShotAttached(playerShoot, forFMOD); }
    public void PlayPlayerDamage(GameObject forFMOD) { RuntimeManager.PlayOneShotAttached(playerDamage, forFMOD); }
    public void PlayPlayerFootSteps(GameObject forFMOD) { RuntimeManager.PlayOneShotAttached(playerFootSteps, forFMOD); }
    public void PlayPlayerJump(GameObject forFMOD) { RuntimeManager.PlayOneShotAttached(playerJump, forFMOD); }

    //////// = Enemy = //////////
    public void PlayEnemyAttack(GameObject forFMOD) { RuntimeManager.PlayOneShotAttached(enemyAttack, forFMOD); }
    public void PlayEnemyDamage(GameObject forFMOD) { RuntimeManager.PlayOneShotAttached(enemyDamage, forFMOD); }
    public void PlayEnemyDie(GameObject forFMOD) { RuntimeManager.PlayOneShotAttached(enemyDie, forFMOD); }
    public void PlayEnemyWalkVoiced(GameObject forFMOD) { RuntimeManager.PlayOneShotAttached(enemyWalkVoiced, forFMOD); }

    //////// = OTHER = //////////
    /// <summary>
    /// Задаём параметр эвента музыки, который в зависимости от количества врогов, переключает треки.
    /// </summary>
    /// <param name="param">От 0 до 1</param>
    public void SetMusicParameter(float param)
    {

        param = Mathf.Clamp(param, 0.0f, 1.0f);
        musicEvent.setParameterByID(musicPiD, param);

    }
    
    public void SetMusicVolume(float volume)
    {

        MusicVCA.setVolume(volume);
        volumeMusicVCA = volume;

    }
    public void SetFXVolume(float volume)
    {

        FXVCA.setVolume(volume);
        volumeFXVCA = volume;

    }

    public static SoundAndMusic Instance
    {

        get
        {
            if (_instance == null)
            {

                _instance = FindObjectOfType<SoundAndMusic>();

                if (_instance == null)
                {

                    GameObject go = new GameObject();
                    go.name = "SingletonController";
                    _instance = go.AddComponent<SoundAndMusic>();
                    DontDestroyOnLoad(go);

                }

            }

            return _instance;

        }

    }

}
