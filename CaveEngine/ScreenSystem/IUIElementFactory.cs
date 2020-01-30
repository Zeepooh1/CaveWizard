using CaveEngine.DrawingSystem;

namespace CaveEngine.ScreenSystem
{
    public interface IUiElementFactory
    {
        UIElement CreateElement(string elementName);
    }
}