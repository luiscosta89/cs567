using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EndlessQuest
{
    public class Camera
    {
        private Matrix transform;

        public Matrix Transform
        {
            get { return transform; }
            set { value = transform; }
        }

        public Viewport view;
        public Vector2 center;
        float zoom;
        float rotation;
        public float Zoom
        {
            get { return zoom; }
            set { value = zoom; }
        }

        public Camera(Viewport newView)
        {
            view = newView;
            zoom = 2;
            rotation = 0;
        }

        public void Update(Vector2 position, int xOffset, int yOffset)
        {

            if (position.X < view.Width / 2)
                center.X = view.Width / 2;
            else if (position.X > xOffset - (view.Width / 2))
                center.X = xOffset - (view.Width / 2);
            else center.X = position.X;


            if (position.Y < view.Height / 2)
                center.Y = view.Height / 2;
            else if (position.Y > yOffset - (view.Height / 2))
                center.Y = yOffset - (view.Height / 2);
            else center.Y = position.Y;




            transform = Matrix.CreateTranslation(
                new Vector3(-center.X + (view.Width / 2), -center.Y + (view.Height / 2), 0)) *
                Matrix.CreateScale(Zoom, Zoom, 1.0f) * Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(-center.X + (view.Width / 2), -center.Y + (view.Height / 2), 0);





        }
    }
}
