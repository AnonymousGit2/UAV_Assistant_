using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Debug helper to mark pixels on a photo
public class MarkingHelper : MonoBehaviour
{
    private Color32[] currentColors;
    private int imageWidth;
    private int imageHeight;

    //Mark rectangle on a photo
    public void markRectangle(Vector2 pos1, Vector2 pos2,ref Texture2D texture) {
            currentColors = texture.GetPixels32();

         imageWidth = texture.width;
         imageHeight = texture.height;

        Vector2 pixelPosition1 = new Vector2(pos1.x * imageWidth, (imageHeight - pos1.y * imageHeight));
        Vector2 pixelPosition2 = new Vector2(pos2.x * imageWidth, (imageHeight - pos2.y * imageHeight));

        ColourBetween(pixelPosition1, new Vector2(pixelPosition1.x, pixelPosition2.y), 2, Color.red);
        ColourBetween(pixelPosition1, new Vector2(pixelPosition2.x, pixelPosition1.y), 2, Color.red);
        ColourBetween(pixelPosition2, new Vector2(pixelPosition1.x, pixelPosition2.y), 2, Color.red);
        ColourBetween(pixelPosition2, new Vector2(pixelPosition2.x, pixelPosition1.y), 2, Color.red);

        ApplyMarkedPixelChanges(ref texture);

        currentColors = null;
        imageHeight = 0;
        imageWidth = 0;
    }

    //Mark center of a photo
    public void MarkCenter(Vector2 pos1, Vector2 pos2, ref Texture2D texture)
    {
        currentColors = texture.GetPixels32();
        imageWidth = texture.width;
        imageHeight = texture.height;

        Vector2 pixelPosition1 = new Vector2(pos1.x * imageWidth, (imageHeight - pos1.y * imageHeight));
        Vector2 pixelPosition2 = new Vector2(pos2.x * imageWidth, (imageHeight - pos2.y * imageHeight));

        var center = (pixelPosition1 + pixelPosition2) / 2;

        ColourBetween(center - new Vector2(5, 5), center + new Vector2(5, 5), 2, Color.red);
        ColourBetween(center - new Vector2(-5, 5), center + new Vector2(-5, 5), 2, Color.red);

        ApplyMarkedPixelChanges(ref texture);
        currentColors = null;
        imageHeight = 0;
        imageWidth = 0;
    }

    //helper to draw
    public void ColourBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
    {
        // Get the distance from start to finish
        float distance = Vector2.Distance(start_point, end_point);
        Vector2 direction = (start_point - end_point).normalized;

        Vector2 cur_position = start_point;

        // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
        float lerp_steps = 1 / distance;

        for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
        {
            cur_position = Vector2.Lerp(start_point, end_point, lerp);
            MarkPixelsToColour(cur_position, width, color);
        }
    }

    public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        // Figure out how many pixels we need to colour in each direction (x and y)
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;
        //int extra_radius = Mathf.Min(0, pen_thickness - 2);

        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
            if (x >= imageWidth || x < 0)
                continue;

            for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
            {
                MarkPixelToChange(x, y, color_of_pen);
            }
        }
    }

    public void MarkPixelToChange(int x, int y, Color color)
    {
        // Need to transform x and y coordinates to flat coordinates of array
        int array_pos = y * imageWidth + x;

        // Check if this is a valid position
        if (array_pos >= currentColors.Length || array_pos < 0)
            return;

        currentColors[array_pos] = color;

    }

    public void ApplyMarkedPixelChanges(ref Texture2D texture)
    {
        texture.SetPixels32(currentColors);
        texture.Apply();
    }



}
