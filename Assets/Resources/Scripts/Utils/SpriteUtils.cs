using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Heroicsolo.Utils
{
    public static class SpriteUtils
    {
        public static Sprite Create (Texture2D texture)
        {
            if (texture == null) return null;

            Rect rect = new(0, 0, texture.width, texture.height);
            Vector2 pivot = new(.5f, .5f);
            return Sprite.Create(texture, rect, pivot);
        }
    }
}
