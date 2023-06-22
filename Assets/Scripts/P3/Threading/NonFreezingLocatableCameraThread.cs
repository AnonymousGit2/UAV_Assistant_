// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;
using UnityEngine.Windows.WebCam;

public class PositionInfo
{
    public string name;
    public Vector2 topleft;
    public Vector2 botright;

    public PositionInfo(string name, Vector2 topleft, Vector2 botright)
    {
        this.name = name;
        this.topleft = topleft;
        this.botright = botright;
    }
}

//NOT USED- Was for Testing a more perfomant scanning
public class NonFreezingLocatableCameraThread : MonoBehaviour
{
    [SerializeField]
    private Shader textureShader = null;

    [SerializeField]
    private Material matWithShader = null;

    [SerializeField]
    private TextMesh text = null;

    [SerializeField]
    [Range(1.3f, 1.4f)]
    public float scaleModifier = 1.32f;

    [SerializeField]
    private GameObject pictureMarker;


    [SerializeField]
    private GameObject connectorGO;

    private PhotoCapture photoCaptureObject = null;
    private Resolution cameraResolution = default(Resolution);
    private bool isCapturingPhoto, isReadyToCapturePhoto = false;
    private uint numPhotos = 0;

    [SerializeField]
    private InfoMapping speechCommandMapper;

    private string[] keywords;

    public UnityEvent onScanStarted;
    public UnityEvent onObjectFound;
    public UnityEvent onNoObjectFound;



    //----------------------WebStuff-----------------------------
    public FeatureType featureType = FeatureType.OBJECT_LOCALIZATION;
    public int maxResults = 10;
    public string url = "https://vision.googleapis.com/v1/images:annotate?key=";
    public string apiKey = "AIzaSyB6eWPhtVxXg304ZFAEamGl-j_FdjqU9Gk";
    Dictionary<string, string> headers;



    [System.Serializable]
    public class AnnotateImageRequests
    {
        public List<AnnotateImageRequest> requests;
    }

    [System.Serializable]
    public class AnnotateImageRequest
    {
        public Image image;
        public List<Feature> features;
    }
    [System.Serializable]
    public class Image
    {
        public string content;
    }

    [System.Serializable]
    public class Feature
    {
        public string type;
        public int maxResults;
    }
    public enum FeatureType
    {
        TYPE_UNSPECIFIED,
        FACE_DETECTION,
        LANDMARK_DETECTION,
        LOGO_DETECTION,
        LABEL_DETECTION,
        TEXT_DETECTION,
        SAFE_SEARCH_DETECTION,
        IMAGE_PROPERTIES,
        OBJECT_LOCALIZATION
    }


    //-------------------Threading------------------------------
    private Queue<ObjectData> objectDataQueue;
    //-----------------------------------------------------------
    // Optional Debugging Stuff
    //public MarkingHelper helper;
    GameObject blendedImage = null;

    [SerializeField]
    private bool optimizeQuads = false;

    private void Start()
    {
        if (onObjectFound == null) {
            onObjectFound = new UnityEvent();
        }
        if (onNoObjectFound== null)
        {
            onNoObjectFound = new UnityEvent();
        }
        if (onScanStarted == null)
        {
            onScanStarted = new UnityEvent();
        }
        objectDataQueue = new Queue<ObjectData>();
        keywords = speechCommandMapper.getAcceptedNames();
        headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json; charset=UTF-8");

        if (apiKey == null || apiKey == "")
            Debug.LogError("No API key. Please set your API key into the \"Web Cam Texture To Cloud Vision(Script)\" component.");


        var resolutions = PhotoCapture.SupportedResolutions;
        if (resolutions == null || resolutions.Count() == 0)
        {
            if (text != null)
            {
                text.text = "Resolutions not available. Did you provide web cam access?";
            }
            return;
        }

        cameraResolution = resolutions.OrderByDescending((res) => res.width * res.height).First();
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);

        if (text != null)
        {
            text.text = "Starting camera...";
        }
    }

    
    private void Update()
    {

      

        
    }


    private void OnDestroy()
    {
        
        isReadyToCapturePhoto = false;
        if (photoCaptureObject != null)
        {
            photoCaptureObject.StopPhotoModeAsync(OnPhotoCaptureStopped);

            if (text != null)
            {
                text.text = "Stopping camera...";
            }
        }
    }

    private void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        if (text != null)
        {
            text.text += "\nPhotoCapture created...";
        }

        photoCaptureObject = captureObject;

        CameraParameters cameraParameters = new CameraParameters(WebCamMode.PhotoMode)
        {
            hologramOpacity = 0.0f,
            cameraResolutionWidth = cameraResolution.width,
            cameraResolutionHeight = cameraResolution.height,
            pixelFormat = CapturePixelFormat.BGRA32
        };
        captureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            isReadyToCapturePhoto = true;

            if (text != null)
            {
                text.text = "Ready!\nPress the above button to take a picture.";
            }
        }
        else
        {
            isReadyToCapturePhoto = false;

            if (text != null)
            {
                text.text = "Unable to start photo mode!";
            }
        }
    }

    /// <summary>
    /// Takes a photo and attempts to load it into the scene using its location data.
    /// </summary>
    public void TakePhoto()
    {
        if (!isReadyToCapturePhoto || isCapturingPhoto)
        {

            text.text = "Camera is not ready";
            return;
        }

        isCapturingPhoto = true;

        if (text != null)
        {
            text.text = "Taking picture...";
        }

        photoCaptureObject.TakePhotoAsync(OnPhotoCaptured);
    }


    Texture2D targetTexture;
    private void OnPhotoCaptured(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            
            HandlePhotoAsync(result, photoCaptureFrame);

        }
        else
        {
            if (text != null)
            {
                text.text += "\nPicture taking failed: " + result.hResult;
            }
        }

        isCapturingPhoto = false;
    }

    public Vector3 GetWorldCoordinateFromPixel(GameObject quad, Vector2 topLeft, Vector2 botRight)
    {
        Vector2 pixelCenter = ((topLeft + botRight) / 2.0f);


        Debug.Log("calculatingWorldCoordinate for quadpos = " + quad.transform.position + " and Coordinate +" + pixelCenter);
        // abstracted from https://answers.unity.com/questions/228486/an-easy-way-to-get-the-global-position-from-a-chil.html

        var position = new Vector2(quad.transform.position.x, quad.transform.position.y);

        Vector3 scale;
        if (transform.parent != null)
        {
            scale = new Vector3(quad.transform.parent.localScale.x * quad.transform.localScale.x, quad.transform.parent.localScale.y * quad.transform.localScale.y, quad.transform.parent.localScale.z * quad.transform.localScale.z);
        }
        else
        {
            scale = quad.transform.localScale;
        }

        Debug.Log("Scale " + scale);

        float startPosX = position.x - (0.5f * scale.x);
        float startPosY = -position.y - (0.5f * scale.y);

        var worldX = startPosX + pixelCenter.x * scale.x;
        var worldY = -(startPosY + pixelCenter.y * scale.y);

        var rot = quad.transform.rotation;

        Vector3 defPosition = new Vector3(worldX, worldY, quad.transform.position.z);

        //rotate points around the center so the real point is hit

        var finalPos = rot * (defPosition - quad.transform.position) + quad.transform.position;
        //markPosition(finalPos);
        //Debug.Log("Final Pos =" + finalPos);
        return finalPos;

    }

    public Vector3 GetWorldCoordinateFromPixel(Transform quad, Vector2 topLeft, Vector2 botRight)
    {
        Vector2 pixelCenter = ((topLeft + botRight) / 2.0f);


        Debug.Log("calculatingWorldCoordinate for quadpos = " + quad.position + " and Coordinate +" + pixelCenter);
        // abstracted from https://answers.unity.com/questions/228486/an-easy-way-to-get-the-global-position-from-a-chil.html

        var position = new Vector2(quad.position.x, quad.position.y);

        Vector3 scale;
        if (transform.parent != null)
        {
            scale = new Vector3(quad.parent.localScale.x * quad.localScale.x, quad.parent.localScale.y * quad.localScale.y, quad.parent.localScale.z * quad.localScale.z);
        }
        else
        {
            scale = quad.localScale;
        }

        Debug.Log("Scale " + scale);

        float startPosX = position.x - (0.5f * scale.x);
        float startPosY = -position.y - (0.5f * scale.y);

        var worldX = startPosX + pixelCenter.x * scale.x;
        var worldY = -(startPosY + pixelCenter.y * scale.y);

        var rot = quad.rotation;

        Vector3 defPosition = new Vector3(worldX, worldY, quad.position.z);

        //rotate points around the center so the real point is hit

        var finalPos = rot * (defPosition - quad.position) + quad.position;
        //markPosition(finalPos);
        //Debug.Log("Final Pos =" + finalPos);
        return finalPos;

    }



    #region Experimentyl
    //TESTING-------------------------------------
    public Vector3 GetWorldCoordinateFromPixelTransformed(GameObject quad, Vector2 pixelCoord, Matrix4x4 worldToCamera, Matrix4x4 projection)
    {

        Debug.Log("calculatingWorldCoordinate for quadpos = " + quad.transform.position + " and Coordinate +" + pixelCoord);
        // abstracted from https://answers.unity.com/questions/228486/an-easy-way-to-get-the-global-position-from-a-chil.html

        var position = new Vector2(quad.transform.position.x, quad.transform.position.y);

        Vector3 scale;
        if (transform.parent != null)
        {
            scale = new Vector3(quad.transform.parent.localScale.x * quad.transform.localScale.x, quad.transform.parent.localScale.y * quad.transform.localScale.y, quad.transform.parent.localScale.z * quad.transform.localScale.z);
        }
        else
        {
            scale = quad.transform.localScale;
        }

        Debug.Log("Scale " + scale);

        float startPosX = position.x - (0.5f * scale.x);
        float startPosY = -position.y - (0.5f * scale.y);

        var worldX = startPosX + pixelCoord.x * scale.x;
        var worldY = -(startPosY + pixelCoord.y * scale.y);

        var rot = quad.transform.rotation;

        Vector3 defPosition = new Vector3(worldX, worldY, quad.transform.position.z);

        //rotate points around the center so the real point is hit
        var finalPos = rot * (defPosition - quad.transform.position) + quad.transform.position;

        //Debug.Log("Final Pos =" + finalPos);

        //Got World coordinate
        //Calculate to CameraCoordinate 
        var cameraCord = worldToCamera * new Vector4(finalPos.x, finalPos.y, finalPos.z, 1f);

        //calculate the clipspace? 
        var clipSpace = projection * new Vector4(cameraCord.x, cameraCord.y, cameraCord.z, 1f);

        //transform in 3 dimenstions
        finalPos = (new Vector3(clipSpace.x / clipSpace.w, clipSpace.y / clipSpace.w, clipSpace.z / clipSpace.w));

        var finalPos2 = (new Vector3(clipSpace.x, clipSpace.y, clipSpace.z)) / 2f;
        var finalPos3 = (new Vector3(clipSpace.x, clipSpace.y, clipSpace.z));
        //now mark position
        markPosition(finalPos);
        markPosition(finalPos2, Color.red);
        markPosition(finalPos3, Color.blue);


        return finalPos;


    }

    //TESTING-------------------------------------
    #endregion

    private void markPosition(Vector3 coord)
    {
        //var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var sphere = Instantiate(pictureMarker);
        sphere.transform.localScale = Vector3.one / 20;
        sphere.transform.parent = null;
        sphere.transform.position = coord;
        Debug.Log("Spawned sphere at" + coord);

    }
    private void markPosition(Vector3 coord, Color col)
    {
        //var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var sphere = Instantiate(pictureMarker);
        sphere.transform.localScale = Vector3.one / 20;
        sphere.transform.parent = null;
        sphere.transform.position = coord;
        Debug.Log("Spawned sphere at" + coord);
        sphere.GetComponent<Renderer>().material.SetColor("_Color", col);
    }



    //[SerializeField]
    //private GameObject objectMarker;
    /*private void markObject(Vector3 cood,string name, Vector3 direction) { 

        var marker = Instantiate(objectMarker);
        marker.transform.localScale = Vector3.one / 10;
        marker.transform.parent = null;
        marker.transform.position = cood;
        marker.transform.LookAt(new Vector3(direction.x, marker.transform.position.y, direction.z),Vector3.up);
        marker.GetComponent<FoundObjectMarker>().setObjectName(name);
        Debug.Log("Spawned object marker at" + cood);
    }*/



    private bool getHitWithSpatialMesh(Vector3 cameraPos, Vector3 direction, out Vector3 target)
    {
        RaycastHit hit;
        var layer = 31;
        var layermask = 1 << layer;
        if (Physics.Raycast(cameraPos, direction, out hit, 2.0f, layermask))
        {
            target = hit.point;

            //var temp = Instantiate(connectorGO, transform.position, Quaternion.identity);
            //temp.GetComponent<Connect>().setLooseConnection(cameraPos,hit.point);

            return true;
        }
        else
        {
            target = Vector3.zero;
            return false;
        }
    }

    public void RaycastForward()
    {
        var sp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sp.transform.position = new Vector3(0, 0, 0);
        sp.transform.localScale = Vector3.one / 20f;
        RaycastHit hit;
        var layer = 31;
        var layermask = 1 << layer;
        if (Physics.Raycast(Camera.main.transform.position, sp.transform.position - Camera.main.transform.position, out hit, 2.0f, layermask))
        {
            markPosition(hit.point);
            //return true;
        }
        else
        {
           
            Debug.Log("nothing Hit");
            //return false;
        }
    }



    private void OnPhotoCaptureStopped(PhotoCapture.PhotoCaptureResult result)
    {
        if (text != null)
        {
            text.text = result.success ? "Photo mode stopped." : "Unable to stop photo mode.";
        }

        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }


    #region WebStuff

    

    
    private async void HandlePhotoAsync(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        onScanStarted.Invoke();
        Debug.Log("Took picture");
        if (text != null)
        {
            text.text += "\nTook picture!";
        }
        Destroy(blendedImage);
        blendedImage = GameObject.CreatePrimitive(PrimitiveType.Quad);
        blendedImage.name = $"Photo{numPhotos++}";
        blendedImage.transform.parent = transform;



        float ratio = cameraResolution.height / (float)cameraResolution.width;
        blendedImage.transform.localScale = new Vector3(blendedImage.transform.localScale.x, blendedImage.transform.localScale.x * ratio, blendedImage.transform.localScale.z);
        //Debug.Log("scaleMod:" + scaleModifier);
        blendedImage.transform.localScale = blendedImage.transform.localScale * scaleModifier;
        //scaleModifier += 0.02f;

        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height)
        {
            wrapMode = TextureWrapMode.Clamp
        };

        photoCaptureFrame.TryGetCameraToWorldMatrix(out Matrix4x4 cameraToWorldMatrix);

        photoCaptureFrame.UploadImageDataToTexture(targetTexture);
        // this enables the picture view
        if (!optimizeQuads)
        {
            
            photoCaptureFrame.TryGetProjectionMatrix(Camera.main.nearClipPlane, Camera.main.farClipPlane, out Matrix4x4 projectionMatrix);
            Renderer quadRenderer = blendedImage.GetComponent<Renderer>();

            //quadRenderer.material = new Material(textureShader);
            quadRenderer.material = new Material(matWithShader.shader);

            quadRenderer.sharedMaterial.SetTexture("_MainTex", targetTexture);
            quadRenderer.sharedMaterial.SetMatrix("_WorldToCameraMatrix", cameraToWorldMatrix.inverse);
            quadRenderer.sharedMaterial.SetMatrix("_CameraProjectionMatrix", projectionMatrix);

        }
        else
        {
            blendedImage.GetComponent<Renderer>().enabled = false;
            blendedImage.GetComponent<MeshCollider>().enabled = false;
        }

        Vector3 cameraPosition;

        //placePhotoInFrontOfCamera(locationMatrix,texture)
        //places the blendedImage with the texture so in front of the camera that it blends with the background

        Debug.Log("Calculate positions");
        if (photoCaptureFrame.hasLocationData)
        {
            Vector4 camToWorldCol1 = cameraToWorldMatrix.GetColumn(1);
            Vector4 camToWorldCol2 = cameraToWorldMatrix.GetColumn(2);
            Vector4 camToWorldCol3 = cameraToWorldMatrix.GetColumn(3);

            Vector3 position = camToWorldCol3 - camToWorldCol2;
            Quaternion rotation = Quaternion.LookRotation(-camToWorldCol2, camToWorldCol1);

            cameraPosition = new Vector3(camToWorldCol3.x, camToWorldCol3.y, camToWorldCol3.z) + blendedImage.transform.up * 0.02f + blendedImage.transform.right * (-0.02f);
            Debug.Log("Cam Position is: " + cameraPosition);

            blendedImage.transform.position = position;
            blendedImage.transform.rotation = rotation;
        }
        else
        {
            if (text != null)
            {
                text.text += "\nNo location data :(";
            }
            blendedImage.transform.position = Vector3.one * 5;
            blendedImage.transform.rotation = UnityEngine.Random.rotation;
            cameraPosition = new Vector3(0, 2, 0);

        }
        
        
        Debug.Log("Starting WebPart");


        // test
        //byte[] byteArray = targetTexture.EncodeToJPG();

        byte[] bytes = null;
        byte[] rawTexture = targetTexture.GetRawTextureData();
        GraphicsFormat format = targetTexture.graphicsFormat;
        Vector2 res = new Vector2(cameraResolution.width, cameraResolution.height);

        await Task.Run(() =>
        {
            try
            {
                bytes = ImageConversion.EncodeArrayToJPG(rawTexture, format,(uint)res.x, (uint)res.y);
            }
            catch (Exception e)
            {
                AsyncToConsole.addException(e);
            }
        });
        

        string base64 = Convert.ToBase64String(bytes);


        //string base64 =Convert.ToBase64String(targetTexture.EncodeToJPG()); 


        bool hit = false;
        AnnotateImageRequests requests = new AnnotateImageRequests();
        requests.requests = new List<AnnotateImageRequest>();
        AnnotateImageRequest request = new AnnotateImageRequest();
        request.image = new Image();
        request.image.content = base64;
        request.features = new List<Feature>();
        Feature feature = new Feature();
        feature.type = featureType.ToString();
        feature.maxResults = maxResults;
        request.features.Add(feature);
        requests.requests.Add(request);


        string jsonData = "";
        await Task.Run(() =>
        {
            jsonData = JsonUtility.ToJson(requests, false);
        });
            
        if (jsonData != string.Empty)
        {
            string url = this.url + this.apiKey;


            byte[] postData = null;

            await Task.Run(() =>
            {
                try
                {
                    postData = System.Text.Encoding.Default.GetBytes(jsonData);
                }
                catch (Exception e)
                {
                    AsyncToConsole.addException(e);
                }
            });

           

            WWWForm form = new WWWForm();
            form.AddField("myField", "myData");

            UnityWebRequest www = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
            Debug.Log("SendindRequest;");
            www.uploadHandler = new UploadHandlerRaw(postData);
            www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            //headers.Add("Content-Type", "application/json; charset=UTF-8");
            await www.SendWebRequest();
            
            Debug.Log("Got answer");
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else // lets do the computational intensive calculations in a thread
            {
                string responses = www.downloadHandler.text;
                // Debug.Log(responses);
                List<PositionInfo> pInformation = null;
                await Task.Run(() =>
                {
                    try
                    {

                        pInformation = HandleWWWAnswer(ref responses, keywords.ToList<string>(), blendedImage, cameraPosition, ref objectDataQueue);

                    }catch (Exception e) {
                        AsyncToConsole.addException(e);
                    }
                });


                if (pInformation != null) {
                    
                    foreach (PositionInfo pInfo in pInformation)
                    {
                        //calculate WorldPosition of the rectangle
                        var centerPixel = GetWorldCoordinateFromPixel(blendedImage, pInfo.topleft, pInfo.botright);//getCenter(pos1, pos2));

                        // calculate direction for the ray
                        var pixelDir = centerPixel - cameraPosition;
                        //now calculate hitting position for the ray
                        Vector3 hitpoint;
                        if (getHitWithSpatialMesh(cameraPosition, pixelDir, out hitpoint))
                        {
                            Debug.Log("Raycast through image hit marking Position: " + pInfo.name);
                            //markObject(hitpoint, name, cameraposition);
                            CloudVisionResultManager.addObject(pInfo.name, hitpoint, cameraPosition);
                            hit = true;
                        }
                        else
                        {
                            Debug.Log("No hit with the mesh");
                        }
                    }
              

                }
               


            }
        }
        else
        {
            Debug.Log("EmptyJsOn");
        }
        //Finally destroy the blendedImage since it is not needed any more and it consumes RAM space
        if (hit)
        {
            onObjectFound.Invoke();
        }
        else {
            onNoObjectFound.Invoke();
        }

        Destroy(blendedImage);
    }



    //NICE

    public async void notBlockingFunc() {
        int test = 0;
        await Task.Run(() =>
        {
            test = AsyncTest();
            AsyncToConsole.addMessage(test.ToString());
        });
        Debug.Log("Wieder Synced: " + test);


    }

    public int AsyncTest() {
        Thread.Sleep(1000);
        return 1;
    
    }

  

    private List<PositionInfo> HandleWWWAnswer(ref string responses, List<string> keywords, GameObject quad, Vector3 cameraposition, ref Queue<ObjectData> objDataList)
    {
        // Debug.Log(responses);
        JSONNode res = JSON.Parse(responses.Replace("\n", "").Replace(" ", ""));

        var itemcount = res["responses"][0]["localizedObjectAnnotations"].Count;
        Vector2 topLeft;
        Vector2 botRight;
        int j = 0;

        List<PositionInfo> posInfo = new List<PositionInfo>();
        for (int i = 0; i < itemcount; i++)
        {
            string name;
            try
            {
                j = i;
                name = res["responses"][0]["localizedObjectAnnotations"][i]["name"].ToString().Trim('"');
                if (!keywords.Contains(name))
                {
                    AsyncToConsole.addMessage("Found Object: " + name + " , is not in Keyword list. Skipping.");
                    continue;
                }
                if (CloudVisionResultManager.isValidated(name))
                {
                    AsyncToConsole.addMessage("Found Object: " + name + " , is already validated.");
                    continue;
                }
                AsyncToConsole.addMessage("Found Object: " + name + " , is in Keyword list. Continue tracking.");

                string first, second;

                first = res["responses"][0]["localizedObjectAnnotations"][i]["boundingPoly"][0][0]["x"].ToString().Trim('"');//.Replace(".", ",");
                second = res["responses"][0]["localizedObjectAnnotations"][i]["boundingPoly"][0][0]["y"].ToString().Trim('"');//.Replace(".", ",");
                AsyncToConsole.addMessage("First: " + first + " Second: " + second);

                //Google omits zero coordinates so we have to create them by our own
                if (first == "")
                {
                    first = "0.0";
                }
                if (second == "")
                {
                    second = "0.0";
                }
                topLeft = new Vector2(float.Parse(first, CultureInfo.InvariantCulture), float.Parse(second, CultureInfo.InvariantCulture));

                first = res["responses"][0]["localizedObjectAnnotations"][i]["boundingPoly"][0][2]["x"].ToString().Trim('"');//.Replace(".", ",");
                second = res["responses"][0]["localizedObjectAnnotations"][i]["boundingPoly"][0][2]["y"].ToString().Trim('"');//.Replace(".", ",");
                AsyncToConsole.addMessage("First: " + first + " Second: " + second);

                //Google omits zero coordinates so we have to create them by our own
                if (first == "")
                {
                    first = "0.0";
                }
                if (second == "")
                {
                    second = "0.0";
                }

                botRight = new Vector2(float.Parse(first, CultureInfo.InvariantCulture), float.Parse(second, CultureInfo.InvariantCulture));
                AsyncToConsole.addMessage("Pos1: " + topLeft + " Pos2: " + botRight);
            }
            catch (FormatException fex)
            {
                AsyncToConsole.addMessage("j = " + j);
                AsyncToConsole.addMessage(fex.ToString());
                break;
            }

            AsyncToConsole.addMessage("Calculating Coordinates;");


            posInfo.Add(new PositionInfo(name, topLeft, botRight));
            //calculate WorldPosition of the rectangle
           

            //Debug.DrawRay(currentPos, pixelDir, Color.red, 5000);

        }
        AsyncToConsole.addMessage("FinishedCoordinate calculation");
        return posInfo;
        
      
    }



    #endregion



    /*
    * Sources: 
    * https://docs.unity3d.com/ScriptReference/Windows.WebCam.PhotoCapture.html
    * https://docs.microsoft.com/en-us/windows/mixed-reality/develop/unity/locatable-camera-in-unity
    * https://forum.unity.com/threads/holographic-photo-blending-with-photocapture.416023/?_ga=2.57872105.210548785.1614215615-862490274.1597860099
    * Example Project 
    */

}
