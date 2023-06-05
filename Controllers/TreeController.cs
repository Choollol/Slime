using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System.Diagnostics;
using System.Numerics;
using Slime.Sprites;

namespace Slime.Controllers
{
    public class TreeController
    {
        private Texture2D texture;
        private List<Vector2> treePos = new List<Vector2>();
        private Vector2 startPos = new Vector2(-100, -300);
        private Random rand = new Random();
        private int treeCount = 0;

        public List<Sprite> trees;

        public TreeController(Texture2D newTexture, List<Vector2> treePositions)
        {
            texture = newTexture;
            treePos = treePositions;
            trees = new List<Sprite>();
            foreach (Vector2 treePosition in treePositions)
            {
                Sprite tree = new Sprite(texture);
                tree.position = treePosition;
                tree.depth = 0.6f + treeCount * 0.00001f;
                trees.Add(tree);
                treeCount++;
            }
        }
        private void clearTrees()
        {
            foreach (Sprite tree in trees.ToList())
            {
                Rectangle treeRect = new Rectangle((int)tree.position.X + 130, (int)tree.position.Y + 382, 118, 76);
                if (
                    //Clearing - top left
                    treeRect.Intersects(new Rectangle(960, 1055, 1090, 670)) ||
                    //Clearing - top right
                    treeRect.Intersects(new Rectangle(2050, 1085, 865, 610)) ||
                    //Clearing - middle
                    treeRect.Intersects(new Rectangle(990, 1725, 1925, 400)) ||
                    //Clearing - bottom
                    treeRect.Intersects(new Rectangle(1340, 2010, 1160, 450)) ||

                    //Bottom path - below nest path - bottom
                    treeRect.Intersects(new Rectangle(1885, 3645, 225, 350)) ||
                    //Bottom path - below nest path - top
                    treeRect.Intersects(new Rectangle(1820, 3165, 295, 485)) ||
                    //Bottom path - above nest path - bottom
                    treeRect.Intersects(new Rectangle(1760, 2815, 225, 355)) ||
                    //Bottom path - above nest path - top
                    treeRect.Intersects(new Rectangle(1695, 2465, 195, 350)) ||

                    //Left path - right
                    treeRect.Intersects(new Rectangle(445, 1470, 515, 260)) ||
                    //Left path - left
                    treeRect.Intersects(new Rectangle(0, 1500, 445, 260)) ||

                    //Top path - bottom
                    treeRect.Intersects(new Rectangle(1820, 575, 195, 480)) ||
                    //Top path - top
                    treeRect.Intersects(new Rectangle(1885, 0, 195, 575)) ||

                    //Right path - left
                    treeRect.Intersects(new Rectangle(2880, 1730, 320, 190)) ||
                    //Right path - right
                    treeRect.Intersects(new Rectangle(3200, 1780, 800, 255)) ||

                    //Bottom right nest path - left
                    treeRect.Intersects(new Rectangle(1985, 3010, 480, 100)) ||
                    //Bottom right nest path - right - left
                    treeRect.Intersects(new Rectangle(2815, 2880, 320, 70)) ||
                    //Bottom right nest - left - top
                    treeRect.Intersects(new Rectangle(2400, 2815, 450, 255)) ||
                    //Bottom right nest - left - bottom
                    treeRect.Intersects(new Rectangle(2465, 3070, 480, 290)) ||
                    //Bottom right nest - right
                    treeRect.Intersects(new Rectangle(3100, 2530, 550, 280)) ||

                    //Bottom left nest path - right
                    treeRect.Intersects(new Rectangle(1090, 2175, 120, 255)) ||
                    //Bottom left nest path - left
                    treeRect.Intersects(new Rectangle(930, 2335, 160, 255)) ||
                    //Bottom left nest - left
                    treeRect.Intersects(new Rectangle(380, 2370, 460, 735)) ||
                    //Bottom left nest - right
                    treeRect.Intersects(new Rectangle(840, 2625, 225, 480)) ||

                    //Top left nest path - bottom
                    treeRect.Intersects(new Rectangle(930, 1055, 190, 160)) ||
                    //Top left nest path - middle
                    treeRect.Intersects(new Rectangle(610, 935, 250, 225)) ||
                    //Top left nest path - top
                    treeRect.Intersects(new Rectangle(545, 670, 180, 290)) ||
                    //Top left nest - left
                    treeRect.Intersects(new Rectangle(415, 195, 415, 505)) ||
                    //Top left nest - right
                    treeRect.Intersects(new Rectangle(835, 225, 215, 475)) ||

                    //Top right nest path - left
                    treeRect.Intersects(new Rectangle(2785, 1250, 190, 225)) ||
                    //Top right nest path - middle
                    treeRect.Intersects(new Rectangle(2975, 1090, 130, 285)) ||
                    //Top right nest path - right
                    treeRect.Intersects(new Rectangle(3105, 1025, 125, 255)) ||
                    //Top right nest - left
                    treeRect.Intersects(new Rectangle(2945, 200, 700, 865)) ||
                    //Top right nest - right
                    treeRect.Intersects(new Rectangle(3425, 200, 510, 960))
                    )
                {
                    trees.Remove(tree);
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            foreach (Sprite tree in trees)
            {
                tree.doSpriteEffect = false;
                spriteBatch.Draw(texture, tree.position, null, Color.White * tree.opacity, 0, Vector2.Zero, 1f, tree.spriteEffect,
                    tree.depth);
            }
        }
        public void Update(GameTime gameTime, Player player)
        {
            foreach (Sprite tree in trees)
            {
                Rectangle leavesRect = new Rectangle((int)tree.position.X, (int)tree.position.Y, 366, 260);
                Rectangle trunkRect = new Rectangle((int)tree.position.X + 130, (int)tree.position.Y + 260, 100, 120);
                int flipChance = (int)rand.Next(0, 2);
                if (player.Rectangle.Intersects(leavesRect) || player.Rectangle.Intersects(trunkRect))
                {
                    if (tree.opacity > 0.25)
                    {
                        tree.opacity -= 0.05f;
                    }
                }
                else
                {
                    if (tree.opacity < 1)
                    {
                        tree.opacity += 0.05f;
                    }
                }
                if (tree.doSpriteEffect)
                {
                    if (flipChance == 0)
                    {
                        tree.spriteEffect = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        tree.spriteEffect = SpriteEffects.None;
                    }
                }
            }
            clearTrees();
        }
    }
}
