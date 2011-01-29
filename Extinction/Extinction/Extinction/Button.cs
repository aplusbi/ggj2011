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
        public bool is_pressed = false, is_active = false;
        public int x, y;
        public string name;
        static public int bwidth=80, bheight=20;

        public Button(string name, Texture2D up, Texture2D down, Texture2D overlay, 
            int x_in, int y_in)
        {
            this.name = name;
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
                Pressed(this, e);
        }
        public void Update(MouseState M)
        {

            Rectangle pointRect = new Rectangle(M.X, M.Y, 1, 1); 
            Rectangle buttonRect = 
                new Rectangle(this.x, this.y, Button.bwidth, Button.bheight);
            if (buttonRect.Intersects(pointRect))
            {
                //mousing over
                if (M.LeftButton == ButtonState.Pressed)
                {
                    if (is_active) is_pressed = false;
                    else is_pressed = true;
                }
                else
                {
                    if (is_pressed)
                    {
                        OnPressed(new EventArgs());
                        is_active = true;
                    }
                    else
                    {
                        is_active = false;
                    }
                }
            }
            else if (M.LeftButton != ButtonState.Pressed)
            {
                
            }
        }
        public void DeSelect()
        {
            is_active = false;
            is_pressed = false;
        }
        public void Draw(SpriteBatch S)
        {
            Texture2D tex_base = (is_pressed==true)?(tex_down):(tex_up);
            S.Draw(tex_base, new Vector2(x, y), Color.White);
            S.Draw(tex_overlay, new Vector2(x, y), Color.White);
        }
    }
}
