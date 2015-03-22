using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace EndlessQuest
{
    public class HUD
    {
        private Vector2 healthPointsPos = new Vector2(20, 10);
        private Vector2 magicPointsPos = new Vector2(20, 30);

        public SpriteFont Font { get; set; }

        public static int healthPoints = 100;
        public static int magicPoints = 50;

        public static void changeHealthPoints (int points)
        {
            healthPoints += points;
        }
        public static void changeMagicPoints (int points)
        {
            magicPoints += points;
        }

        public HUD()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the Score in the top-left of screen
            spriteBatch.DrawString(
                Font,                          // SpriteFont
                "HP: " + healthPoints.ToString(),  // Text
                healthPointsPos,                      // Position
                Color.Black);                  // Tint
            spriteBatch.DrawString(
                Font,
                "MP: " + magicPoints.ToString(),
                magicPointsPos,
                Color.Black);
        }
    }
}
