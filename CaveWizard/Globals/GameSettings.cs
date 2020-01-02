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

        public static void InitParser()
        {
            try
            {
                settingsData = settingsParser.ReadFile("settings.ini");
                GameSettings._Volume = bool.Parse(settingsData["Sound"]["MasterVolume"]);
            }
            catch (ParsingException e)
            {
                settingsData = new IniData();
                settingsData["Sound"]["MasterVolume"] = GameSettings._Volume.ToString();
                settingsParser.WriteFile("settings.ini", settingsData);
            }

        }

        public static void SaveSettings()
        {
            settingsData["Sound"]["MasterVolume"] = GameSettings._Volume.ToString();
            settingsParser.WriteFile("settings.ini", settingsData);

        }
    }
    
    
}