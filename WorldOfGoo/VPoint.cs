using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace WorldOfGoo
{
    public class VPoint
    {

        public readonly Texture2D texture;
        public Vector2 pos;
        private Vector2 origin;
        Rectangle rect;
        float angle;
        int t;
        public bool left;

        public Vector2 velocity; // Velocidad de la bolita
        private float gravity = 9.8f;
        public float Radius = 29f;
        private Rectangle sourceRectangle;

        public Vector2 OldPosition;
        public float Gravity = 29.8f;
        public float Friction = 0.99f;

        
        private int animationFrame;
        private float timeSinceLastFrame;
        private float frameTime = 0.07f; // Tiempo entre frames

        // Variables para control de animación
        private int totalFrames = 6; // Total de frames en la animación
        private int loopFrames = 1; // Frames para hacer loop al principio
        private int loopCount = 0; // Contador de loops
        private int loopsBeforeFullAnimation = 15; // Cantidad de loops antes de la animación completa
        private bool isFullAnimation = false;



        public VPoint(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.pos = position;
           // Asume que la altura es constante
            origin = new Vector2(rect.Width / 2, rect.Height / 2);
            this.velocity = new Vector2(0, 250);
            OldPosition = position - new Vector2(0, 10);

            sourceRectangle = new Rectangle(0, 0, 58, texture.Height); // Asume que la altura es constante
            animationFrame = 0;
            timeSinceLastFrame = 0.0f;
        }


        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 gravityEffect = new Vector2(0, gravity * deltaTime);
            velocity += gravityEffect; // Aplicar gravedad
            pos += velocity * deltaTime; 

                    timeSinceLastFrame += deltaTime;
            if (timeSinceLastFrame >= frameTime)
            {
                if (isFullAnimation)
                {
                    animationFrame++;
                    if (animationFrame >= totalFrames)
                    {
                        animationFrame = 0;
                        isFullAnimation = false; // Reiniciar para el próximo ciclo de loops
                        loopCount = 0;
                    }
                }
                else
                {
                    animationFrame = (animationFrame + 1) % loopFrames; // Loop entre los primeros 'loopFrames' frames
                    loopCount++;
                    if (loopCount >= loopsBeforeFullAnimation)
                    {
                        isFullAnimation = true; // Iniciar animación completa
                    }
                }

                sourceRectangle.X = animationFrame * 58;
                timeSinceLastFrame = 0;
            }
        }



        public void Draw(SpriteBatch sb)
        {
            // Para la bolita
            sb.Draw(texture, new Rectangle((int)pos.X - (int)Radius, (int)pos.Y - (int)Radius, (int)Radius * 2, (int)Radius * 2), sourceRectangle, Color.White);

        }

        public void ApplyForce(Vector2 force)
        {
            // Asumiendo que 'mass' es la masa del punto
            Vector2 acceleration = force / 10f;
            velocity += acceleration;
        }



    }

}
