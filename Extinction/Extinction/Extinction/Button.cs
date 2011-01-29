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

namespace Extinction
{
    class Button
    {
        public Texture2D tex_up, tex_down, tex_overlay;
        public bool is_pressed = false;
        public int x, y;

        public Button(Texture2D up, Texture2D down, Texture2D overlay, int x_in, int y_in)
        {
            tex_down = down;
            tex_up = up;
            tex_overlay = overlay;
            x = x_in;
            y = y_in;
        }

        public delegate void ButtonPressedHandler(object sender, EventArgs e);
        public event ButtonPressedHandler Pressed;
        public void OnPressed(EventArgs e)
        {
            if (Pressed != null)
                OnPressed(e);
        }
        public void Update(MouseState M)
        {
        }
        public void Draw(SpriteBatch S)
        {
            Texture2D tex_base = (is_pressed==true)?(tex_down):(tex_up);
            S.Draw(tex_base, new Vector2(x, y), Color.White);
            S.Draw(tex_overlay, new Vector2(x, y), Color.White);
        }
    }
}
