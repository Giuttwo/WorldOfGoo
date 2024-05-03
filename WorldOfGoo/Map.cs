using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WorldOfGoo
{
    public class Map
    {
        private List<Platform> platforms; // Lista interna de plataformas
        public List<Platform> Platforms => platforms;
        public List<WinTile> WinTiles => winTiles;
        public Vector2 SpawnPoint { get; private set; }
        private Texture2D platformTexture;
        private GraphicsDevice graphicsDevice;
        private string mapString;
        private int tileWidth = 30; // Ancho de cada 'tile' o bloque en el mapa
        private int tileHeight = 50;
        private VElement vElement;


        private List<string> mapStrings; // Lista de strings de mapas
        private int currentMapIndex;

        private List<WinTile> winTiles;  // Lista para tiles de victoria
        private Texture2D winTileTexture;

        public Map(GraphicsDevice graphicsDevice, List<string> mapStrings)
        {
            this.graphicsDevice = graphicsDevice;
            this.mapStrings = mapStrings;
            platformTexture = new Texture2D(graphicsDevice, 1, 1);
            winTileTexture = new Texture2D(graphicsDevice, 1, 1); // Textura para los tiles de victoria
            platformTexture.SetData(new[] { Color.Gray });
            
            platforms = new List<Platform>();
            winTiles = new List<WinTile>();
            




        }

        public void SetVElement(VElement vElement)
        {
            this.vElement = vElement;
            LoadMap(currentMapIndex);
        }



        private void LoadMap(int mapIndex)
        {
            platforms.Clear(); // Limpia las plataformas existentes
            winTiles.Clear();
            currentMapIndex = mapIndex;
            currentMapIndex = Global.currentLevelIndex;
            string mapString = mapStrings[currentMapIndex];
            int tileWidth = 30;
            int tileHeight = 50;
            int rows = mapString.Length / 150;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < 150; j++)
                {
                    char tileType = mapString[i * 150 + j];
                    if (tileType == 'B')
                    {
                        int x = j * tileWidth;
                        int y = i * tileHeight;
                        platforms.Add(new Platform(Global.brickTexture, x, y, 45, tileHeight));
                    }
                    else if (tileType == 'V')
                    {
                        int x = j * tileWidth;
                        int y = i * tileHeight;
                        VPoint vPoint = new VPoint(Global.ballTexture, new Vector2(x, y)); // Crea una nueva bolita VPoint en las coordenadas actuales
                        vElement.AddPoint(vPoint, false); // Agrega la bolita al elemento VElement
                    }
                    else if (tileType == 'W')
                    {
                        int x = j * tileWidth;
                        int y = i * tileHeight;
                        winTiles.Add(new WinTile(Global.pipeTexture, x, y, 100, 200)); // Usar Global.pipeTexture para los WinTiles
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var platform in platforms)
            {
                platform.Draw(spriteBatch);
            }

            foreach (var winTile in winTiles)
            {
                winTile.Draw(spriteBatch);
            }
        }



        public void NextLevel()
        {
            int nextIndex = (currentMapIndex + 1) % mapStrings.Count;
            LoadMap(nextIndex);
            Global.currentLevelIndex = nextIndex;

        }

        public void PreviousLevel()
        {
            int previousIndex = (currentMapIndex - 1 + mapStrings.Count) % mapStrings.Count;
            LoadMap(previousIndex);
            Global.currentLevelIndex = previousIndex;
        }




    }

}
