using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using IllusionPlugin;
using static BeatSaberTrainer.Util;
using static BeatSaberTrainer.ReflectionUtil;


namespace BeatSaberTrainer
{
    public class Plugin : IPlugin
    {
        public string Name => PluginName;
        public string Version => "0.1.0.0";
        public static string PluginName = "BeatSaber-Trainer";
        bool hasBSUtils;

        public const int NjsMaxSize = 100;
        public const int NjsMinSize = -100;
        public const int NjstepSize = 1;
        private static float _NjsOverride = 10;
        public static float NjsOverride
        {
            get
            {
                return _NjsOverride;
            }
            set
            {
                _NjsOverride = value;                
            }
        }
        private static float _maxNjs = 10;
        public static float MaxNJS
        {
            get { return _maxNjs; }
            set { _maxNjs = value; }
        }
        public static bool enableNjsOverride = false;
        public static bool njsAsMin = true;
        public static bool dynamicNJS = true;
        public static NJSController _njsController;
        private static GameScenesManager _scenesManager;
        public void OnApplicationStart()
        {

            
            Util.LEMsgLevel = MSGLEVEL.DEBUG;
            //Checks if a IPlugin with the name in quotes exists, in case you want to verify a plugin exists before trying to reference it, or change how you do things based on if a plugin is present
            hasBSUtils = IllusionInjector.PluginManager.Plugins.Any(x => x.Name == "Beat Saber Utils");

            if (hasBSUtils)
            {
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
                SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            }
            else
                WriteMsg("Disabled, Beat Saber Utils not found");

        }

        private void SceneManagerOnActiveSceneChanged(Scene oldScene, Scene newScene)
        {

            if (newScene.name == "Menu")
            {
                //Code to execute when entering The Menu


            }

            if (newScene.name == "GameCore")
            {
                //Code to execute when entering actual gameplay
                DebugMessage("Entering OnActiveSceneChanged");
                if (_scenesManager == null)
                {
                    _scenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();

                    if (_scenesManager != null)
                        _scenesManager.transitionDidFinishEvent += OnSceneTransitionDidFinish;
                }

            }


        }

        private void OnSceneTransitionDidFinish()
        {
            if (SceneManager.GetActiveScene().name == "GameCore")
            {

                try
                {
                    if (enableNjsOverride)
                    {
                        DebugMessage("NJS Override is enabled");
                        
                        _njsController = new GameObject("NJSController").AddComponent<NJSController>();


                    }

                }
                catch (Exception ex)
                {
                    WriteMsg($"{ex.Message}\n{ex.StackTrace}", MSGLEVEL.ERROR);
                }
            }
        }



        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            //Create GameplayOptions/SettingsUI if using either
            if (scene.name == "Menu")
            {
                DebugMessage("CreatingUI");
                UI.TrainerUI.CreateUI();
                DebugMessage("UI Created");
            }

        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnLevelWasLoaded(int level)
        {

        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
            if (Input.GetKeyUp(KeyCode.N))
            {
                DebugMessage("Keypress 'N' detected");
                _njsController.AdjustNJS();
            }


            if (((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) && Input.GetKeyDown(KeyCode.J))
            {
                DebugMessage("Keypress 'Ctrl+J' detected");
                if (enableNjsOverride && (NjsOverride > NjsMinSize))
                {
                    NjsOverride -= NjstepSize;
                    DebugMessage($"Decreasing jump speed to: {NjsOverride}");
                    UI.TrainerUI.DecrementNJS();
                    if (_njsController != null)
                        _njsController.AdjustNJS();
                }
            }

            if (((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) && Input.GetKeyDown(KeyCode.K))
            {
                DebugMessage("Keypress 'Ctrl+K' detected");
                if (enableNjsOverride && (NjsOverride < NjsMaxSize))
                {
                    NjsOverride += NjstepSize;
                    UI.TrainerUI.IncrementNJS();
                    DebugMessage($"Increasing jump speed to: {NjsOverride}");
                    if(_njsController != null)
                        _njsController.AdjustNJS();
                }
            }


        }

        public void OnFixedUpdate()
        {
        }
    }
}
