using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomUI.GameplaySettings;
using CustomUI.Settings;
using UnityEngine;
using static BeatSaberTrainer.Util;

namespace BeatSaberTrainer.UI
{
    class TrainerUI
    {
        public static MultiSelectOption njsOptionInstance;

        public static void CreateUI()
        {
            //This will create the UI for the plugin when called, keep in mind that the mod will require CustomUI when executing this as it calls functions etc from the library
            CreateGameplayOptionsUI();
        }



        public static void CreateSettingsUI()
        {
            //This will create a menu tab in the settings menu for your plugin
            var pluginSettingsSubmenu = SettingsUI.CreateSubMenu("Submenu Name");

            var exampleToggle = pluginSettingsSubmenu.AddBool("Example Toggle");
            //Fetch your initial value for the option from within the braces, or simply have it default to a value
            exampleToggle.GetValue += delegate { return false; };
            exampleToggle.SetValue += delegate (bool value) {
                //Whatever execution you want to occur after setting the value

            };

        }

        public static void CreateGameplayOptionsUI()
        {
            var pluginSubmenu = GameplaySettingsUI.CreateSubmenuOption(GameplaySettingsPanels.ModifiersLeft, "Training Mod", "MainMenu", "TrainingMod", 
                "Training mods");
            var enableOption = GameplaySettingsUI.CreateToggleOption(GameplaySettingsPanels.ModifiersLeft, "Enable NJS Override", "TrainingMod", "");
            enableOption.GetValue = Plugin.enableNjsOverride;
            enableOption.OnToggle += (value) => { Plugin.enableNjsOverride = value; DebugMessage($"Enable NJS Override set to: {Plugin.enableNjsOverride}"); };
            
            var minOption = GameplaySettingsUI.CreateToggleOption(GameplaySettingsPanels.ModifiersLeft, "Raise NJS Only", "TrainingMod", "If NJS override is less than the song's, use the song's");
            minOption.GetValue = Plugin.njsAsMin;
            minOption.OnToggle += (value) => { Plugin.njsAsMin = value; DebugMessage($"Raise NJS Only set to: {Plugin.njsAsMin}"); };

            var dynNjsOption = GameplaySettingsUI.CreateToggleOption(GameplaySettingsPanels.ModifiersLeft, "Dynamic NJS", "TrainingMod", "NJS will change depending on your performance");
            dynNjsOption.GetValue = Plugin.dynamicNJS;
            dynNjsOption.OnToggle += (value) => { Plugin.dynamicNJS = value; DebugMessage($"Dynamic NJS set to: {Plugin.dynamicNJS}"); };


            njsOptionInstance = GameplaySettingsUI.CreateListOption(GameplaySettingsPanels.ModifiersLeft, "Min. NJS", "TrainingMod", "Song NJS will be at least this much");
            for(int i = 0; i < 100; i++)
            {
                njsOptionInstance.AddOption((float) i);
            }
            njsOptionInstance.GetValue = delegate { return (float) Plugin.NjsOverride; };
            njsOptionInstance.OnChange += (float njsVal) => { Plugin.NjsOverride = njsVal; DebugMessage($"NJS set to: {Plugin.NjsOverride}"); };

            var maxNjsOption = GameplaySettingsUI.CreateListOption(GameplaySettingsPanels.ModifiersLeft, "Max. NJS", "TrainingMod", "NJS won't increase beyond this value");
            for (int i = 1; i < 100; i++)
            {
                maxNjsOption.AddOption((float) i);
            }
            maxNjsOption.GetValue = delegate { return (float) Plugin.MaxNJS; };
            maxNjsOption.OnChange += (float njsVal) => {
                Plugin.MaxNJS = njsVal;
                DebugMessage($"Max NJS set to: {Plugin.MaxNJS}");
                while (Plugin.MaxNJS < Plugin.NjsOverride)
                    DecrementNJS();
            };

        }

        public static void IncrementNJS()
        {
            njsOptionInstance.InvokeIncButtonPress();
        }

        public static void DecrementNJS()
        {
            njsOptionInstance.InvokeDecButtonPress();
        }




    }
}
