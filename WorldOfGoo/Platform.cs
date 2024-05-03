using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfGoo
{
    public class Platform
    {
        public Rectangle Bounds { get; private set; }
        public Texture2D Texture { get; private set; }

        public Platform(Texture2D texture, int x, int y, int width, int height)
        {
            Texture = texture;
            Bounds = new Rectangle(x, y, width, height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Bounds, Color.White);
        }
    }
}
