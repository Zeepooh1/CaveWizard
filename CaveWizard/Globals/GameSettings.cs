using System;
using IniParser;
using IniParser.Exceptions;
using IniParser.Model;

namespace CaveWizard.Globals
{
    public static class GameSettings
    {
        public static FileIniDataParser settingsParser = new FileIniDataParser();
        public static IniData settingsData;

        
        public static bool _Volume = true;
        public static bool _FullScreen = false;

        public static void InitParser()
        {
            try
            {
                settingsData = settingsParser.ReadFile("settings.ini");
                _Volume = bool.Parse(settingsData["Sound"]["MasterVolume"]);
                _FullScreen = bool.Parse(settingsData["Graphics"]["FullScreen"]);
            }
            catch (Exception e)
            {
                settingsData = new IniData();
                settingsData["Sound"]["MasterVolume"] = _Volume.ToString();
                settingsData["Graphics"]["FullScreen"] = _FullScreen.ToString();
                settingsParser.WriteFile("settings.ini", settingsData);
            }

        }

        public static void SaveSettings()
        {
            settingsData["Sound"]["MasterVolume"] = _Volume.ToString();
            settingsData["Graphics"]["FullScreen"] = _FullScreen.ToString();
            settingsParser.WriteFile("settings.ini", settingsData);

        }
    }
    
    
}