using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomUI.GameplaySettings;
using CustomUI.Settings;
using static BeatSaberTrainer.Util;

namespace BeatSaberTrainer.UI
{
    class BasicUI
    {
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
            var njsOption = GameplaySettingsUI.CreateListOption(GameplaySettingsPanels.ModifiersLeft, "Min. NJS", "TrainingMod", "Song NJS will be at least this much");
            for(int i = 0; i < 100; i++)
            {
                njsOption.AddOption((float) i);
            }
            njsOption.GetValue = delegate { return (float) Plugin.NjsOverride; };
            njsOption.OnChange += (float njsVal) => { Plugin.NjsOverride = (int) njsVal; DebugMessage($"NJS set to: {Plugin.NjsOverride}"); };
        }

        



    }
}
