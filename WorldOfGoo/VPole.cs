using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WorldOfGoo
{
    public class VPole
    {
        public VPoint StartPoint { get; private set; }
        public VPoint EndPoint { get; private set; }
        private float stiffness; // Rigidez del resorte
        private float damping; // Amortiguamiento
        private float restLength; // Longitud de reposo del resorte

        private Texture2D dummyTexture; // Textura simple para la representación visual
        private Color color = Color.White;

        public VPole(GraphicsDevice graphicsDevice,VPoint startPoint, VPoint endPoint, float restLength, float stiffness, float damping)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            this.restLength = restLength;
            this.stiffness = stiffness;
            this.damping = damping;

            // Asumimos que el GraphicsDevice es accesible globalmente o se pasa de alguna manera
            dummyTexture = new Texture2D(graphicsDevice, 1, 1);
            dummyTexture.SetData(new[] { Color.Black });
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 edge = EndPoint.pos - StartPoint.pos;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            float length = edge.Length();

            // Dibuja una línea entre los dos puntos
            spriteBatch.Draw(Global.vpoleTexture,
                    StartPoint.pos,
                    null,
                    color,
                    angle,
                    new Vector2(0, Global.vpoleTexture.Height / 2), // El origen en el centro de la textura en Y
                    new Vector2(length / Global.vpoleTexture.Width, 0.3f), // Escala la textura para que se extienda entre los dos puntos
                    SpriteEffects.None,
                    0);
        }

        public void Update()
        {
            // Calcular vector de dirección y distancia actual
            Vector2 displacement = EndPoint.pos - StartPoint.pos;
            float currentLength = displacement.Length();
            Vector2 direction = displacement / currentLength;

            // Fuerza de Hooke (F = -kx)
            float x = currentLength - restLength;
            Vector2 force = direction * (-stiffness * x);

            // Damping (F = -bv)
            Vector2 relativeVelocity = EndPoint.velocity - StartPoint.velocity;
            force += -damping * relativeVelocity;

            // Aplicar fuerza a los puntos
            StartPoint.ApplyForce(-force);
            EndPoint.ApplyForce(force);
        }
    }
}
