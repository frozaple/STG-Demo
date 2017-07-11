using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager
{
    private Texture2D[] bulletTextures;
    private Sprite[] bulletSprites;

    public void InitSprites()
    {
        bulletTextures = new Texture2D[1];
        bulletTextures[0] = Resources.Load<Texture2D>("Enemy/Bullet/bullet1");
        bulletSprites = new Sprite[11 * 16];

        Vector2 center = Vector2.one / 2;
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 11; y++)
            {
                Rect rect = new Rect(x * 16, 240 - y * 16, 16, 16);
                bulletSprites[x + y * 16] = Sprite.Create(bulletTextures[0], rect, center, 1f);
            }
        }
    }

    public Sprite GetBulletSprite(int shape, int color)
    {
        int spriteIndex = shape * 16 + color;
        if (spriteIndex >= 0 && spriteIndex < bulletSprites.Length)
            return bulletSprites[spriteIndex];
        else
            return null;
    }
}
