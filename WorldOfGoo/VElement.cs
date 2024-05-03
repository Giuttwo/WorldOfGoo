using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace WorldOfGoo
{
    public class VElement
    {
        public List<VPoint> Points;
        private List<Platform> platforms;

        private List<WinTile> wintile;
        public List<VPole> Poles { get; private set; }

        private GraphicsDevice graphicsDevice;

        public Vector2 CurrentMousePosition { get; set; }

        private Texture2D dummyTexture;

      



        public VElement(List <WinTile> wintile,List<Platform> platforms, GraphicsDevice graphicsDevice)
        {
            Points = new List<VPoint>();
            this.platforms = platforms;
            this.wintile = wintile;
            Poles = new List<VPole>();
            this.graphicsDevice = graphicsDevice;

            dummyTexture = new Texture2D(graphicsDevice, 1, 1);
            dummyTexture.SetData(new[] { Microsoft.Xna.Framework.Color.White });
        }

        public void AddPoint(VPoint point, bool checkConnections = true)
        {

            int currentLevelIndex = Global.currentLevelIndex; // cuantas pelotitas se pueden poner  
            int maxBalls = Global.numberOfBalls[currentLevelIndex];




            if (Global.currentBalls < maxBalls) {

                if (checkConnections)
                {
                    if (CanAddPoint(point.pos, 210)) // Verifica si hay dos conexiones válidas
                    {
                        Global.ballSoundEffect.Play(1f, 0.0f, 0.0f);
                        Points.Add(point);
                        ConnectToClosestPoints(point, 210);
                        CheckCollisions();
                        Global.currentBalls++;
                    }

                    
                }
                else
                {
                    // Agrega el punto sin comprobar las conexiones
                    Points.Add(point);
                    ConnectToClosestPoints(point, 210);
                    CheckCollisions();
                    
                }



            }
           



        }

        public bool CanAddPoint(Vector2 position, float maxDistance)
        {
            List<VPoint> closestPoints = Points
                .Where(p => Vector2.Distance(position, p.pos) <= maxDistance)
                .OrderBy(p => Vector2.Distance(position, p.pos))
                .Take(2)
                .ToList();

            return closestPoints.Count == 2;
        }

        private void ConnectToClosestPoints(VPoint newPoint, float maxDistance)
        {
            List<VPoint> closestPoints = Points
             .Where(p => p != newPoint && Vector2.Distance(newPoint.pos, p.pos) <= maxDistance)
             .OrderBy(p => Vector2.Distance(newPoint.pos, p.pos))
             .Take(2) // Conectar con hasta 2 puntos más cercanos
             .ToList();

            foreach (var point in closestPoints)
            {
                float distance = Vector2.Distance(newPoint.pos, point.pos);
                AddPole(newPoint, point, distance); // Usar la distancia actual como la longitud de reposo inicial
            }
        }

        public void AddPole(VPoint startPoint, VPoint endPoint, float restLength)
        {
            VPole pole = new VPole(graphicsDevice, startPoint, endPoint, restLength, 0.6f, 0.8f); // Ajusta la rigidez y amortiguamiento según necesidad
            Poles.Add(pole);
        }


        private void CheckCollisions()
        {
            foreach (var point in Points)
            {
                // Primero, chequear colisiones entre puntos
                foreach (var otherPoint in Points)
                {
                    if (point != otherPoint)
                    {
                        float distance = Vector2.Distance(point.pos, otherPoint.pos);
                        float collisionDistance = point.Radius + otherPoint.Radius;
                        if (distance < collisionDistance)
                        {
                            ResolveCollision(point, otherPoint, distance, collisionDistance);
                        }
                    }
                }

                // Luego, chequear colisiones con plataformas
                foreach (var platform in platforms)
                {
                    CheckCollisionWithPlatform(point, platform);
                }

                // Luego, chequear colisiones con wintiles
                foreach (var wintile in wintile)
                {
                    CheckCollisionWithWintile(point, wintile);
                }
            }
        }

        private void ResolveCollision(VPoint p1, VPoint p2, float distance, float collisionDistance)
        {
            float overlap = 0.5f * (collisionDistance - distance);
            Vector2 difference = p2.pos - p1.pos;
            Vector2 direction = Vector2.Normalize(difference);
            if (!float.IsNaN(direction.X) && !float.IsNaN(direction.Y))
            {
                p1.pos -= direction * overlap;
                p2.pos += direction * overlap;
            }
        }


        private void CheckCollisionWithWintile(VPoint point, WinTile winTile)
        {
            if (point.pos.X + point.Radius > winTile.Bounds.Left &&
         point.pos.X - point.Radius < winTile.Bounds.Right &&
         point.pos.Y + point.Radius > winTile.Bounds.Top &&
         point.pos.Y - point.Radius < winTile.Bounds.Bottom)
            {
                // Si hay colisión, llama a la función que actualiza la variable global 'won'
                Global.won = true;
            }
        }


        private void CheckCollisionWithPlatform(VPoint point, Platform platform)
        {
            if (point.pos.X + point.Radius > platform.Bounds.Left &&
         point.pos.X - point.Radius < platform.Bounds.Right)
            {
                float bottomOfBall = point.pos.Y + point.Radius; // Borde inferior de la bolita
                float topOfPlatform = platform.Bounds.Top; // Parte superior de la plataforma

                // Verificar si el borde inferior de la bolita está tocando la parte superior de la plataforma
                if (bottomOfBall > topOfPlatform && point.pos.Y < topOfPlatform)
                {
                    float penetrationDepth = bottomOfBall - topOfPlatform;

                    // Ajustar la posición de la bolita para que justo toque la plataforma
                    point.pos.Y -= penetrationDepth;

                    // Detener el movimiento vertical para simular que la bolita está reposando sobre la plataforma
                    point.velocity.Y = 0;
                }
            }
        }

        public void Update(GameTime gameTime)
        {



            foreach (var pole in Poles)
            {
                pole.Update();
            }

            foreach (var point in Points)
            {
                point.Update(gameTime);
            }


            CheckCollisions();
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            foreach (var pole in Poles)
            {
                pole.Draw(spriteBatch); // Asegúrate de que VPole tenga el método Draw definido
            }

            foreach (var point in Points)
            {
                point.Draw(spriteBatch);
            }


            // Dibujar líneas de preview
            if (Points.Count > 0)
            {
                float maxDistanceForPreview = 210; // Este debe ser el mismo valor que usas para crear los VPoles en ConnectToClosestPoints

                // Obtener los puntos más cercanos que cumplen con la distancia máxima permitida
                var validClosestPoints = Points
                    .Where(p => Vector2.Distance(CurrentMousePosition, p.pos) < maxDistanceForPreview)
                    .OrderBy(p => Vector2.Distance(CurrentMousePosition, p.pos))
                    .Take(2)
                    .ToList();

                // Solo dibujar si hay dos puntos que cumplen la condición
                if (validClosestPoints.Count == 2)
                {
                    foreach (var point in validClosestPoints)
                    {
                        DrawPreviewLine(spriteBatch, CurrentMousePosition, point.pos);
                    }
                }
            }

        }


        private void DrawPreviewLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            float length = edge.Length();

            spriteBatch.Draw(dummyTexture, start, null, Color.White, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        


    }
}
