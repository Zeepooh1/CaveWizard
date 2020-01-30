using CaveEngine.DrawingSystem;
using CaveEngine.ScreenSystem;
using CaveWizard.Game;
using CaveWizard.UIElements;

namespace CaveWizard.Helpers
{
    public class CaveWizardUIElementFactory: IUiElementFactory
    {
        private Player _player;
        public CaveWizardUIElementFactory(Player player)
        {
            _player = player;
        }
        public UIElement CreateElement(string elementName)
        {
            return elementName switch
            {
                "HudHotBar" => (UIElement) new UIHotBar(),
                "HudHealthBar" => new UIHealthBar(_player),
                _ => null
            };
        }
    }
}