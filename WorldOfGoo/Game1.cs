using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WorldOfGoo
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D ballTexture;
        private List<VPoint> elements = new List<VPoint>();
        private MouseState previousMouseState;
        private VElement vElement;
        private Map map;
        private InputManager input;
        private Camera camera;

        private Vector2 parallaxPosition1;
        private Vector2 parallaxPosition2;
        private Vector2 parallaxPosition3;

        private Texture2D backgroundLayer1;
        private Texture2D backgroundLayer2;
        private Texture2D backgroundLayer3;

        private float backgroundSpeed1 = 0.1f; // Velocidad del fondo más cercano
        private float backgroundSpeed2 = 0.05f; // Velocidad de la capa media
        private float backgroundSpeed3 = 0.025f; // Velocidad del fondo más lejano

        private Texture2D resetButtonTexture;  // Textura para el botón de reinicio
        private Rectangle resetButtonRect;  // Área del botón de reinicio
        private Texture2D winMessageTexture;
        private Texture2D nextLevelButtonTexture;
        private Rectangle nextLevelButtonRect;
        private Texture2D brickTexture;

        private Song bgMusic;  // Cambia el tipo a Song
        private SoundEffectInstance bgMusicInstance;

        private SoundEffect ballSoundEffect;

        SpriteFont gameFont;





        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Configura el juego para ejecutarse en pantalla completa
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();




        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ballTexture = Content.Load<Texture2D>("goo"); // recurso "goo"
            Global.ballTexture = ballTexture;
            resetButtonTexture = Content.Load<Texture2D>("resetButton");
            Global.pipeTexture = Content.Load<Texture2D>("pipeTexture");
            Global.brickTexture = Content.Load<Texture2D>("brick");
            Global.vpoleTexture = Content.Load<Texture2D>("vpole");

            gameFont = Content.Load<SpriteFont>("Fonts/GameFont");


            int buttonWidth = 300;
            int buttonHeight = 140;
            resetButtonRect = new Rectangle(GraphicsDevice.Viewport.Width - buttonWidth - 10, 10, buttonWidth, buttonHeight);




            backgroundLayer1 = Content.Load<Texture2D>("layer1");
            backgroundLayer2 = Content.Load<Texture2D>("layer2");
            backgroundLayer3 = Content.Load<Texture2D>("layer3");

            //musica
            bgMusic = Content.Load<Song>("bgmusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bgMusic);

            //efectos de sonido
            ballSoundEffect = Content.Load<SoundEffect>("ballsound");
            Global.ballSoundEffect= ballSoundEffect;



            // Cargar otras texturas como se muestra anteriormente
            winMessageTexture = Content.Load<Texture2D>("winMessage"); // Asume que tienes una imagen llamada winMessage.png
            nextLevelButtonTexture = Content.Load<Texture2D>("nextLevelButton");


            int winMessageX = (GraphicsDevice.Viewport.Width - winMessageTexture.Width) / 2;
            int winMessageY = 20;  // Asumiendo que quieres un poco de espacio en la parte superior

            nextLevelButtonRect = new Rectangle(
                winMessageX + (winMessageTexture.Width - buttonWidth+610) / 2, // Centra el botón debajo del mensaje
                winMessageY + winMessageTexture.Height -210,  // 10 píxeles de espacio entre el mensaje y el botón
                buttonWidth,
                buttonHeight
            );



            List<string> mapStrings = new List<string> {
            ".......................................W.............................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            "....................................V....V...........................................................................................................\n" +
            "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB...............................................................\n" 
             ,

            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            "........................................................W............................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".V...V...............................................................................................................................................\n" +
            "BBBBBBB..........................................BBBBBBBBBBBBBBBBB...................................................................................\n" 
            ,

            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            "......................................................W..............................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            "...............................................BBBBBBBBBBBBBBBB......................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            ".....................................................................................................................................................\n" +
            "..V...V..............................................................................................................................................\n" +
            "BBBBBBBB.............................................................................................................................................\n",

            };


            map = new Map(GraphicsDevice, mapStrings);
            vElement = new VElement(map.WinTiles,map.Platforms, GraphicsDevice);
            map.SetVElement(vElement);

            input = new InputManager();
            camera = new Camera();
        }

        protected override void Update(GameTime gameTime)
        {

            Global.gameTime = gameTime;

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;


            input.Update(camera);


            MouseState currentMouseState = Mouse.GetState();

            //botton reset
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                if (resetButtonRect.Contains(currentMouseState.Position))
                {
                    ResetGame();  // Llama a tu método de reinicio
                    Global.won=false;
                }
            }



            //boton next level
            if (Global.won && nextLevelButtonRect.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                // Carga el siguiente nivel o haz lo que necesites hacer aquí
                map.NextLevel();
                ResetGame();
                Global.won = false;  // Resetea la condición de ganado después de cargar el nivel
            }





            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                ResetGame();
            }

            // Transforma la posición del mouse usando la matriz inversa de la cámara
            Vector2 mousePosition = currentMouseState.Position.ToVector2();
            mousePosition = Vector2.Transform(mousePosition, camera.InverseTransform);

            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                // Usa la posición del mouse transformada para crear un nuevo punto
                vElement.AddPoint(new VPoint(ballTexture, mousePosition));
            }
            previousMouseState = currentMouseState;

            // Actualiza la posición del mouse en VElement con la posición transformada
            vElement.CurrentMousePosition = mousePosition;

            vElement.Update(gameTime);


            // Actualiza las posiciones de los fondos
            parallaxPosition1.X += backgroundSpeed1 * elapsed;
            parallaxPosition2.X += backgroundSpeed2 * elapsed;
            parallaxPosition3.X += backgroundSpeed3 * elapsed;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();




            if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                map.NextLevel(); // Carga el siguiente nivel
                ResetGame();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                map.PreviousLevel(); // Carga el nivel anterior
                ResetGame();
            }



            base.Update(gameTime);

        }



        private void ResetGame()
        {

            Global.currentBalls = 0;

            // Reiniciar las listas y objetos al estado inicial
            elements.Clear();
            vElement = new VElement(map.WinTiles,map.Platforms, GraphicsDevice);
            map.SetVElement(vElement);

            // Resetear la posición de la cámara
            camera.Reset();

            // Restablecer las posiciones parallax
            parallaxPosition1 = Vector2.Zero;
            parallaxPosition2 = Vector2.Zero;
            parallaxPosition3 = Vector2.Zero;

            
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null);

            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            // Calcula la posición repetida del fondo para que siempre cubra la pantalla completa.
            float parallaxFactor1 = 0.1f; // Más lento para el fondo más lejano
            float parallaxFactor2 = 0.05f;
            float parallaxFactor3 = 0.025f;

            Vector2 backgroundPosition1 = new Vector2(-camera.Position.X * parallaxFactor1 % backgroundLayer1.Width, 0);
            Vector2 backgroundPosition2 = new Vector2(-camera.Position.X * parallaxFactor2 % backgroundLayer2.Width, 0);
            Vector2 backgroundPosition3 = new Vector2(-camera.Position.X * parallaxFactor3 % backgroundLayer3.Width, 0);

            // Dibuja suficientes copias del fondo para cubrir la pantalla completa
            for (int x = (int)backgroundPosition1.X - backgroundLayer1.Width; x < screenWidth; x += backgroundLayer1.Width)
            {
                _spriteBatch.Draw(backgroundLayer1, new Rectangle(x, 0, backgroundLayer1.Width, screenHeight), Color.White);
            }

            for (int x = (int)backgroundPosition2.X - backgroundLayer2.Width; x < screenWidth; x += backgroundLayer2.Width)
            {
                _spriteBatch.Draw(backgroundLayer2, new Rectangle(x, 0, backgroundLayer2.Width, screenHeight), Color.White);
            }

            for (int x = (int)backgroundPosition3.X - backgroundLayer3.Width; x < screenWidth; x += backgroundLayer3.Width)
            {
                _spriteBatch.Draw(backgroundLayer3, new Rectangle(x, 0, backgroundLayer3.Width, screenHeight), Color.White);
            }

            _spriteBatch.End();

            // Inicia otro sprite batch para dibujar los objetos que deben ser afectados por la cámara
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform);

            map.Draw(_spriteBatch);
            vElement.Draw(_spriteBatch);

            _spriteBatch.End();

            //botones
            _spriteBatch.Begin();




            int currentLevelIndex = Global.currentLevelIndex; // cuantas pelotitas se pueden poner  
            int maxBalls = Global.numberOfBalls[currentLevelIndex];

            string ballsText = $"Balls left: {maxBalls - Global.currentBalls}";

           
            _spriteBatch.DrawString(gameFont, ballsText, new Vector2(10, 10), Color.White);


            if (Global.won)
            {
                // Dibuja el mensaje de ganaste
                Vector2 winMessagePosition = new Vector2(
                    (GraphicsDevice.Viewport.Width - winMessageTexture.Width) / 2,
                    20
                );
                _spriteBatch.Draw(winMessageTexture, winMessagePosition, Color.White);

                // Dibuja el botón de siguiente nivel justo debajo
               //// _spriteBatch.Draw(nextLevelButtonTexture, nextLevelButtonRect, Color.White);
            }


            // Dibuja el botón de reinicio en la esquina superior derecha
            _spriteBatch.Draw(resetButtonTexture, resetButtonRect, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);


            base.Draw(gameTime);
        }
    }
}
