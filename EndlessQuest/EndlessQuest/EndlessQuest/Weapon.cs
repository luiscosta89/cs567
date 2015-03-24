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
    class Weapon : Sprite
    {
        public string weaponName;
        public int damage = 0;

        public override Vector2 Direction
        {
            get { return speed; }
            
        }

         public Weapon(Texture2D textureImage, Vector2 position, Point frameSize,
                               int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, string collisionCueName)
                               : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, collisionCueName)
        {
        }

        public Weapon(Texture2D textureImage, Vector2 position, Point frameSize,
                                    int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, int millisecondsPerFrame, string collisionCueName)
                                    : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, millisecondsPerFrame, collisionCueName)
        {
        }
    }
}
