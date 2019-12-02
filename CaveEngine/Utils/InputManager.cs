using Microsoft.Xna.Framework.Input;

namespace CaveEngine.Utils
{
    public static class InputManager
    {
        public static KeyboardState KeyState = Keyboard.GetState();
        public static KeyboardState PrevKeyState = Keyboard.GetState();
        public static MouseState MouseState = Mouse.GetState();
        public static MouseState PrevMouseState = Mouse.GetState();

        public static bool IsKeyPressed(Keys key)
        {
                return KeyState.IsKeyUp(key) && PrevKeyState.IsKeyDown(key);
        }

        public static bool IsLeftMouseButtonPressed()
        {
            return MouseState.LeftButton == ButtonState.Released && PrevMouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool IsRightMouseButtonPressed()
        {
            return MouseState.RightButton == ButtonState.Released && PrevMouseState.RightButton == ButtonState.Pressed;
        }
        
        
    }
}