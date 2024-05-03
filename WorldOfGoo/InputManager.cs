using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace WorldOfGoo
{
    internal class InputManager
    {

        private KeyboardState previousState;
        private KeyboardState currentState;

        public InputManager()
        {
            previousState = Keyboard.GetState();
        }

        public void Update(Camera camera)
        {
            currentState = Keyboard.GetState();

            Vector2 movement = Vector2.Zero;
            if (currentState.IsKeyDown(Keys.W))
                movement.Y -= 10;
            if (currentState.IsKeyDown(Keys.A))
                movement.X -= 10;
            if (currentState.IsKeyDown(Keys.S))
                movement.Y += 10;
            if (currentState.IsKeyDown(Keys.D))
                movement.X += 10;

            if (movement != Vector2.Zero)
                camera.Move(movement);

            previousState = currentState;
        }

    }
}
