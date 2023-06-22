using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;
using FreeDraw;
using System;

public class WebCamTextureToCloudVision : MonoBehaviour {

	public string url = "https://vision.googleapis.com/v1/images:annotate?key=";
	public string apiKey = "AIzaSyB6eWPhtVxXg304ZFAEamGl-j_FdjqU9Gk"; //Put your google cloud vision api key here
	public float captureIntervalSeconds = 5.0f;
	//public int requestedWidth = 640;
	public int requestedHeight = 480;
	public FeatureType featureType = FeatureType.OBJECT_LOCALIZATION;
	public int maxResults = 10;
	//public Text responseText, responseArray;
	private string responseText, responseArray;

	public Sprite imageToSend;
	public Drawable imageToDraw;


	WebCamTexture webcamTexture;
	Texture2D texture2D;
	Dictionary<string, string> headers;

	[System.Serializable]
	public class AnnotateImageRequests {
		public List<AnnotateImageRequest> requests;
	}

	[System.Serializable]
	public class AnnotateImageRequest {
		public Image image;
		public List<Feature> features;
	}

	[System.Serializable]
	public class Image {
		public string content;
	}

	[System.Serializable]
	public class Feature {
		public string type;
		public int maxResults;
	}

	public enum FeatureType {
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

	// Use this for initialization
	void Start () {
		headers = new Dictionary<string, string>();
		headers.Add("Content-Type", "application/json; charset=UTF-8");

		if (apiKey == null || apiKey == "")
			Debug.LogError("No API key. Please set your API key into the \"Web Cam Texture To Cloud Vision(Script)\" component.");
		
	}

	public void CaptureOnePicture() {
		StartCoroutine(CaptureOnce());
	}

	public void CaptureGivenPicture(Texture2D pic) {

		StartCoroutine(CaptureOnce(pic));
	}



	private IEnumerator CaptureOnce(Texture2D pictureToSend = null)
	{

		byte[] jpg;
		if (pictureToSend == null)
		{
			jpg = imageToSend.texture.EncodeToJPG();
		}
		else {
			Debug.Log("Replacing Pic");
			jpg = pictureToSend.EncodeToJPG();
			imageToDraw.changeTexture(pictureToSend,true);
		}

			string base64 = System.Convert.ToBase64String(jpg);

			AnnotateImageRequests requests = new AnnotateImageRequests();
			requests.requests = new List<AnnotateImageRequest>();

			AnnotateImageRequest request = new AnnotateImageRequest();
			request.image = new Image();
			request.image.content = base64;
			request.features = new List<Feature>();
			Feature feature = new Feature();
			feature.type = this.featureType.ToString();
			feature.maxResults = this.maxResults;
			request.features.Add(feature);
			requests.requests.Add(request);

			string jsonData = JsonUtility.ToJson(requests, false);
		if (jsonData != string.Empty)
		{
			string url = this.url + this.apiKey;
			byte[] postData = System.Text.Encoding.Default.GetBytes(jsonData);

			using (WWW www = new WWW(url, postData, headers))
			//using (UnityWebRequest www = UnityWebRequest.Post(url, jsonData))
			{
				yield return www;
				if (string.IsNullOrEmpty(www.error))
				{
					string responses = www.text.Replace("\n", "").Replace(" ", "");
					// Debug.Log(responses);
					JSONNode res = JSON.Parse(responses);

					var itemcount = res["responses"][0]["localizedObjectAnnotations"].Count;
					Vector2 pos1 = Vector2.zero;
					Vector2 pos2 = Vector2.zero;
					int j = 0;
					
						
						for (int i = 0; i < itemcount; i++)
						{
						try
						{
							j = i;
							pos1 = new Vector2(
								float.Parse(res["responses"][0]["localizedObjectAnnotations"][i]["boundingPoly"][0][0]["x"].ToString().Trim('"').Replace(".", ",")),
								float.Parse(res["responses"][0]["localizedObjectAnnotations"][i]["boundingPoly"][0][0]["y"].ToString().Trim('"').Replace(".", ",")));
							pos2 = new Vector2(
								float.Parse(res["responses"][0]["localizedObjectAnnotations"][i]["boundingPoly"][0][2]["x"].ToString().Trim('"').Replace(".", ",")),
								float.Parse(res["responses"][0]["localizedObjectAnnotations"][i]["boundingPoly"][0][2]["y"].ToString().Trim('"').Replace(".", ",")));

						}
						catch (FormatException fex)
						{
							Debug.LogError("j = " + j);
							Debug.LogError(fex);
							break;
						}


							//Image manipulations
							imageToDraw.drawRectangleNormalizedTopLeft(pos1, pos2, 2, Color.red);
							imageToDraw.MarkCenter(pos1, pos2, 2, Color.red);

							var centerPixel = imageToDraw.GetWorldCoordinateFromPixel(imageToDraw.GetCenterinPixel(pos1, pos2));
							var pixelDir = centerPixel - Camera.main.transform.position;
							var currentPos = Camera.main.transform.position;

							Debug.DrawRay(currentPos, pixelDir, Color.red, 5000);

						}
					

					

					string fullText = "Name: " + res["responses"][0]["localizedObjectAnnotations"][0]["name"].ToString().Trim('"') + " " +
										"Score: " + res["responses"][0]["localizedObjectAnnotations"][0]["score"].ToString().Trim('"') + " ";
										//"FirstPos: " + "x: " +pos1.x +" y: "+pos1.y+
										//"SecondPos: " + "x: " + pos2.x + " y: " + pos2.y;
					
					
				

					//TODO insert full Text modifications
					var tempString = "";
					string fullString = "";
					int tempi = 0;
					//while (tempString == "") { 
					//fullString = res["responses"][0]["labelAnnotations"][tempi]["description"].ToString().Trim('"');
					//}

					if (fullText != "")
					{

						responseText = fullText.Replace("\\n", " ");
						fullText = fullText.Replace("\\n", ";");
						string[] texts = fullText.Split(';');
						responseArray = "";
						for (int i = 0; i < texts.Length; i++)
						{
							responseArray += texts[i];
							if (i != texts.Length - 1)
								responseArray += ", ";
						}
						Debug.Log("OCR Response: " + fullText);
						Debug.Log("Full Response: " + res.ToString());
					}
					else {
						Debug.Log("Empty Response "+ res.ToString());
					}
				}
				else
				{
					Debug.Log("Error: " + www.error);
				}
			}
		}
		else
		{
			Debug.Log("EmptyJsOn");
		}
	}


#if UNITY_WEBGL
	void OnSuccessFromBrowser(string jsonString) {
		Debug.Log(jsonString);	
	}

	void OnErrorFromBrowser(string jsonString) {
		Debug.Log(jsonString);	
	}
#endif

}
