using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace WorldOfGoo
{
    internal class Global
    {
        public static GraphicsDeviceManager g;
        public static SpriteBatch s;
        public static GameTime gameTime;
        public static Texture2D ballTexture;
        public static Texture2D pipeTexture;
        public static Texture2D brickTexture;
        public static Texture2D vpoleTexture;
        public static SoundEffect ballSoundEffect;
         
        public static bool won;

        public static List<int> numberOfBalls = new List<int> { 10, 20, 22 };
        public static int currentLevelIndex;

        public static int currentBalls=0;



        public void UpdateGameTime(GameTime gametime)
        {

            gameTime = gametime;

        }
        public void loadTexttures(Texture2D ball)
        {

            ballTexture = ball;

        }

        public void GameWon() {
            won = true;
        }

        public void NewGame()
        {
            won = false;
        }


    }
}
