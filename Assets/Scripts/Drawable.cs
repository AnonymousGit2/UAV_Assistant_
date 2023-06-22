using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FreeDraw
{
    [RequireComponent(typeof(SpriteRenderer))]

    // 1. Attach this to a read/write enabled sprite image
    // 2. Set the drawing_layers  to use in the raycast
    // 3. Attach a 2D collider (like a Box Collider 2D) to this sprite
    // 4. Hold down left mouse to draw on this texture!
    public class Drawable : MonoBehaviour
    {
        // PEN COLOUR
        public static Color Pen_Colour = Color.red;     // Change these to change the default drawing settings
        // PEN WIDTH (actually, it's a radius, in pixels)
        public int Pen_Width = 3;

        public string FileName;
        

        public delegate void Brush_Function(Vector3 world_position);
        // This is the function called when a left click happens
        // Pass in your own custom one to change the brush type
        // Set the default function in the Awake method
        public Brush_Function current_brush;

        public LayerMask Drawing_Layers;

        public bool Reset_Canvas_On_Play = true;
        //public bool loadImageOnStart = false;
        //public string FileToLoadOnStart;
        // The colour the canvas is reset to each time
        public Color Reset_Colour = new Color(0, 0, 0, 0);  // By default, reset the canvas to be transparent

        // Used to reference THIS specific file without making all methods static
        public static Drawable drawable;
        // MUST HAVE READ/WRITE enabled set in the file editor of Unity
        Sprite drawable_sprite;
        Texture2D drawable_texture;

        private Color32[] original_texture;


        Vector2 previous_drag_position;
        Color[] clean_colours_array;
        Color transparent;
        Color32[] cur_colors;
        bool mouse_was_previously_held_down = false;
        bool no_drawing_on_current_drag = false;
        bool mouseHold = false;






        //////////////////////////////////////////////////////////////////////////////
        // BRUSH TYPES. Implement your own here


        // When you want to make your own type of brush effects,
        // Copy, paste and rename this function.
        // Go through each step
        public void BrushTemplate(Vector3 world_position)
        {
            // 1. Change world position to pixel coordinates
            Vector2 pixel_pos = WorldToPixelCoordinates(world_position);

            // 2. Make sure our variable for pixel array is updated in this frame
            cur_colors = drawable_texture.GetPixels32();

            ////////////////////////////////////////////////////////////////
            // FILL IN CODE BELOW HERE

            // Do we care about the user left clicking and dragging?
            // If you don't, simply set the below if statement to be:
            //if (true)

            // If you do care about dragging, use the below if/else structure
            if (previous_drag_position == Vector2.zero)
            {
                // THIS IS THE FIRST CLICK
                // FILL IN WHATEVER YOU WANT TO DO HERE
                // Maybe mark multiple pixels to colour?
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                // THE USER IS DRAGGING
                // Should we do stuff between the previous mouse position and the current one?
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ////////////////////////////////////////////////////////////////

            // 3. Actually apply the changes we marked earlier
            // Done here to be more efficient
            ApplyMarkedPixelChanges();

            // 4. If dragging, update where we were previously
            previous_drag_position = pixel_pos;
        }




        // Default brush type. Has width and colour.
        // Pass in a point in WORLD coordinates
        // Changes the surrounding pixels of the world_point to the static pen_colour
        public void PenBrush(Vector3 world_point)
        {
            Vector2 pixel_pos = WorldToPixelCoordinates(world_point);

            //var projection = Vector3.ProjectOnPlane(pixel_pos, transform.position + transform.forward);


            //cur_colors = drawable_texture.GetPixels32();

            if (previous_drag_position == Vector2.zero)
            {
                // If this is the first time we've ever dragged on this image, simply colour the pixels at our mouse position
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
                // Debug.Log("Dot");
            }
            else
            {
                // Colour in a line from where we were on the last update call
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
                //Debug.Log("Line");
            }
            ApplyMarkedPixelChanges();

            //Debug.Log("Dimensions: " + pixelWidth + "," + pixelHeight + ". Units to pixels: " + unitsToPixels + ". Pixel pos: " + pixel_pos);
            previous_drag_position = pixel_pos;
        }


        // Helper method used by UI to set what brush the user wants
        // Create a new one for any new brushes you implement
        public void SetPenBrush()
        {
            // PenBrush is the NAME of the method we want to set as our current brush
            current_brush = PenBrush;
        }



        // This is where the magic happens.
        // Detects when user is left clicking, which then call the appropriate function
        void Update()
        {



            // Is the user holding down the left mouse button?
            bool mouse_held_down = Input.GetMouseButton(0);

            /*
                if (mouse_held_down && !no_drawing_on_current_drag)
                {
                 

                    RaycastHit hit;
                    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                    Vector3 objectHit = Vector3.zero;
                    if (Physics.Raycast(ray, out hit, Drawing_Layers.value))
                    {
                        objectHit = hit.point;
                        Vector3 rawLocalCoordinates = WorldToPixelCoordinates(objectHit);
                       // Vector3 transformedLocalCoordinates = new Vector2(rawLocalCoordinates.x + transform.position.x, rawLocalCoordinates.y + transform.position.y);

                   

                        // if (xScale > yScale) {
                        //    float ratio = xScale / yScale;
                        //  transformedLocalCoordinates = new Vector2(transformedLocalCoordinates.x, (transformedLocalCoordinates.y / ratio)- (1- 1/ratio) *3.3f);
                        //}

                        // Debug.Log("Drawing Position" + ob +" 2D "+ ob2);

                        // Do something with the object that was hit by the raycast.
                        Debug.Log("Raycast hit at" + rawLocalCoordinates);

                        current_brush(rawLocalCoordinates);

                    }

                    //if (hit != null && hit.transform != null)
                    //{
                    //    // We're over the texture we're drawing on!
                    //    // Use whatever function the current brush is
                    //    current_brush(mouse_world_position);
                    //}

                    else
                    {
                        // We're not over our destination texture
                        previous_drag_position = Vector2.zero;
                        if (!mouse_was_previously_held_down)
                        {
                            // This is a new drag where the user is left clicking off the canvas
                            // Ensure no drawing happens until a new drag is started
                            no_drawing_on_current_drag = true;
                        }
                    }
                }
                // Mouse is released
                else if (!mouse_held_down && !mouseHold)
                {
                    previous_drag_position = Vector2.zero;
                    no_drawing_on_current_drag = false;
                }
                mouse_was_previously_held_down = mouse_held_down;
            */
        }

        public void drawRectangleNormalizedTopLeft(Vector2 pos1, Vector2 pos2, int width, Color color) {
            var imageWidth = drawable_texture.width;
            var imageHeight = drawable_texture.height;

            Vector2 pixelPosition1 = new Vector2(pos1.x * imageWidth, (imageHeight - pos1.y * imageHeight));
            Vector2 pixelPosition2 = new Vector2(pos2.x * imageWidth, (imageHeight - pos2.y * imageHeight));

            ColourBetween(pixelPosition1, new Vector2(pixelPosition1.x, pixelPosition2.y), width, color);
            ColourBetween(pixelPosition1, new Vector2(pixelPosition2.x, pixelPosition1.y), width, color);
            ColourBetween(pixelPosition2, new Vector2(pixelPosition1.x, pixelPosition2.y), width, color);
            ColourBetween(pixelPosition2, new Vector2(pixelPosition2.x, pixelPosition1.y), width, color);

            ApplyMarkedPixelChanges();
        }

        public void MarkCenter(Vector2 pos1, Vector2 pos2, int width, Color color)
        {
            var imageWidth = drawable_texture.width;
            var imageHeight = drawable_texture.height;

            Vector2 pixelPosition1 = new Vector2(pos1.x * imageWidth, (imageHeight - pos1.y * imageHeight));
            Vector2 pixelPosition2 = new Vector2(pos2.x * imageWidth, (imageHeight - pos2.y * imageHeight));

            var center = (pixelPosition1 + pixelPosition2) / 2;

            ColourBetween(center - new Vector2(5, 5), center + new Vector2(5, 5), width, color);
            ColourBetween(center - new Vector2(-5, 5), center + new Vector2(-5, 5), width, color);

            ApplyMarkedPixelChanges();
        }


        public Vector2 GetCenterinPixel(Vector2 pos1, Vector2 pos2)
        {
            var imageWidth = drawable_texture.width;
            var imageHeight = drawable_texture.height;

            Vector2 pixelPosition1 = new Vector2(pos1.x * imageWidth, (imageHeight - pos1.y * imageHeight));
            Vector2 pixelPosition2 = new Vector2(pos2.x * imageWidth, (imageHeight - pos2.y * imageHeight));

            return (pixelPosition1 + pixelPosition2) / 2;
        }


        //public Vector3 GetWorldCoordinateFromPixel(Vector2 pixelCoord) {

        //    var imageWidth = drawable_texture.width;
        //    var imageHeight = drawable_texture.height;

        //    Vector3 textureOrigin = transform.position;

        //    var imagecoordExtends = drawable_sprite.bounds.size;

        //    var xCord = imagecoordExtends.x * ( pixelCoord.x / imageWidth);
        //    var yCord = imagecoordExtends.y * ( pixelCoord.y / imageHeight);

        //    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    sphere.transform.parent = transform;

        //    sphere.transform.position = new Vector3(xCord, yCord, transform.position.z);

        //    return Vector3.zero;
        //}

        public Vector3 GetWorldCoordinateFromPixel(Vector2 pixelCoord)
        {
            // abstracted from https://answers.unity.com/questions/228486/an-easy-way-to-get-the-global-position-from-a-chil.html
            

            var position = new Vector2(transform.position.x, transform.position.y);
            var boundsSize  = drawable_sprite.bounds.size;
            Vector3 scale;
            if (transform.parent != null)
            {
                scale = new Vector3(transform.parent.localScale.x * transform.localScale.x, transform.parent.localScale.y * transform.localScale.y, transform.parent.localScale.z * transform.localScale.z);
            }
            else {
                scale = transform.localScale;
            }

            Debug.Log(drawable_sprite.pixelsPerUnit);
            Debug.Log(drawable_sprite.bounds.size);
     

            float pixelInWorldSpace = 1.0f / drawable_sprite.pixelsPerUnit;
            float startPosX = position.x - (boundsSize.x * 0.5f * scale.x);
            float startPosY = position.y - (boundsSize.y * 0.5f * scale.y);

            var worldX = startPosX + (pixelInWorldSpace * pixelCoord.x) * scale.x;
            var worldY =  startPosY + (pixelInWorldSpace * pixelCoord.y) * scale.y;

            var rot = transform.rotation;

            Vector3 defPosition = new Vector3(worldX, worldY, transform.position.z);

            //rotation * (vector - pivot) + pivot;

            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //sphere.transform.position = new Vector3(worldX, worldY, transform.position.z);


            //rotate points around the center so the real point is hit
            return rot * (defPosition - transform.position) +  transform.position;

        }





        public void ColourBetweenNormalized(Vector2 position1Normalized, Vector2 position2Normalized, int width, Color color)
        {
            var imageWidth = drawable_texture.width;
            var imageHeight = drawable_texture.height;

            Vector2 pixelPosition1 = new Vector2(position1Normalized.x * imageWidth, position1Normalized.y * imageHeight);
            Vector2 pixelPosition2 = new Vector2(position2Normalized.x * imageWidth, position2Normalized.y * imageHeight);

            ColourBetween(pixelPosition1, pixelPosition2, width, color);
            Debug.Log(pixelPosition1 + "" + pixelPosition2 + "" + imageWidth + "" + imageHeight);
            ApplyMarkedPixelChanges();
        }


        // Set the colour of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is coloured
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



  
        public void reLoadTexture ()
        {
            cur_colors = drawable_texture.GetPixels32();
            drawable_texture.SetPixels32(cur_colors);
            drawable_texture.Apply();
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
                if (x >= (int)drawable_texture.width || x < 0)
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
            int array_pos = y * (int)drawable_texture.width + x;

            // Check if this is a valid position
            if (array_pos >= cur_colors.Length || array_pos < 0)
                return;

            cur_colors[array_pos] = color;

        }

        public void ApplyMarkedPixelChanges()
        {
            drawable_texture.SetPixels32(cur_colors);
            drawable_texture.Apply();
        }


        // Directly colours pixels. This method is slower than using MarkPixelsToColour then using ApplyMarkedPixelChanges
        // SetPixels32 is far faster than SetPixel
        // Colours both the center pixel, and a number of pixels around the center pixel based on pen_thickness (pen radius)
        public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    drawable_texture.SetPixel(x, y, color_of_pen);
                }
            }

            drawable_texture.Apply();
        }


        public Vector2 WorldToPixelCoordinates(Vector3 world_position)
        {
            // Change coordinates to local coordinates of this image
            //Vector3 local_pos_projected = Vector3.ProjectOnPlane(transform.InverseTransformPoint(world_position),transform.position+transform.forward);
            Vector3 local_pos = transform.InverseTransformPoint(world_position);

           // Debug.Log("Local_pos: " + local_pos + " Orig " + world_position); //+ "Projected: "+ local_pos_projected);

            // Change these to coordinates of pixels
            float pixelWidth = drawable_texture.width;
            float pixelHeight = drawable_texture.height;
            float unitsToPixelsW = pixelWidth / drawable_sprite.bounds.size.x; //* transform.localScale.x;
            float unitsToPixelsH = pixelHeight / drawable_sprite.bounds.size.y; //* transform.localScale.x;

            // Need to center our coordinates
            float centered_x = local_pos.x * unitsToPixelsW + pixelWidth / 2;
            float centered_y = local_pos.y * unitsToPixelsH + pixelHeight / 2;

            // Round current mouse position to nearest pixel
            Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

            //Debug.Log("drawing on "+pixel_pos);
            return pixel_pos;
        }


        // Changes every pixel to be the reset colour
        public void ResetCanvas()
        {
            //drawable_texture.Resize(defaultWidth, defaultHeight);
            //drawable_texture.SetPixels(clean_colours_array);
            //drawable_texture.Apply();
            drawable_texture.SetPixels32(original_texture);
            reLoadTexture();

        }



        public void Start()
        {
            cur_colors = drawable_texture.GetPixels32();
            //drawRectangleNormalized(Vector2.zero, Vector2.one, 2, Color.red);
            Debug.Log(drawable_sprite.pixelsPerUnit);
            Debug.Log(drawable_sprite.bounds.size);

        }

        void Awake()
        {
            
            drawable = this;
            // DEFAULT BRUSH SET HERE
            current_brush = PenBrush;
            

            drawable_sprite = this.GetComponent<SpriteRenderer>().sprite;
            drawable_texture = drawable_sprite.texture;
            original_texture = drawable_texture.GetPixels32();
        



            // Initialize clean pixels to use
            clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
            for (int x = 0; x < clean_colours_array.Length; x++)
                clean_colours_array[x] = Reset_Colour;

            // Should we reset our canvas image when we hit play in the editor?
            if (Reset_Canvas_On_Play)
                ResetCanvas();
            
        }

        void OnDestroy()
        {
            drawable_texture.SetPixels32(original_texture); ;
            ResetCanvas();

        }

        public void changeTexture(Texture2D tex, bool overwriteOriginal = true){
            if (overwriteOriginal) {
                original_texture = tex.GetPixels32() ;
            }

            clean_colours_array = new Color[tex.width * tex.height];
            for (int x = 0; x < clean_colours_array.Length; x++)
                clean_colours_array[x] = Reset_Colour;

            drawable_texture.Resize(tex.width, tex.height);
            drawable_texture.SetPixels32(tex.GetPixels32());
            cur_colors = drawable_texture.GetPixels32();
            drawable_texture.Apply();

            
         }

        /*public void SaveToFile(string name) {
            var toSave = drawable_sprite.texture;
            byte[] itemBGBytes = toSave.EncodeToPNG();
            var folder = Directory.CreateDirectory("Drawables"); // returns a DirectoryInfo object
            string path;
            if (name == "")
            {
                path = "Drawables/" + FileName + ".png";
            }
            else { 
                path = "Drawables/" + name + ".png";
            }
            File.WriteAllBytes(path, itemBGBytes);
            Debug.Log("The file has been saved with the filename: " + path);
        }*/

        /*public void LoadFromFile(string name) {
            string path;
            if (name == "")
            {
                path = "Drawables/" + FileName + ".png";
            }
            else {
                path = "Drawables/" + name + ".png";
            }


            Debug.Log("Loading from" + path);
            //Load texture
            
            //Color32[] textureLoaded = drawable_texture.GetPixels32();
            //Fill in 
           

            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(path))
            {
                fileData = File.ReadAllBytes(path);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                Debug.Log("Size of texture" + tex.width+ " * " + tex.height);
                
                drawable_texture.Resize(tex.width, tex.height);
                drawable_texture.SetPixels32(tex.GetPixels32());

                

                reLoadTexture();
                drawable_texture.Apply();

                //if syncing every other client should also load this file
            }

        }*/

        /*void LoadImage(byte[] fileData)
        {
            Debug.Log("Received Load Command");
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            Debug.Log("Size of texture" + tex.width + " * " + tex.height);
            drawable_texture.Resize(tex.width, tex.height);
            drawable_texture.SetPixels32(tex.GetPixels32());

            reLoadTexture();
            drawable_texture.Apply();
        }*/
        




    }
}