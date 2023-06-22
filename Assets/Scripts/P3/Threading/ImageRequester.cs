using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using static NonFreezingLocatableCamera;



// DOES NOT WORK YET, Maybe Late
//NOT USED- Was for Testing a more perfomant scanning
public class ObjectData
{
    public string name;
    public Vector3 pixeldir;
    public Vector3 camPos;

    public ObjectData(string name, Vector3 pixeldir, Vector3 camPos)
    {
        this.name = name;
        this.pixeldir = pixeldir;
        this.camPos = camPos;
    }
}
public class ImageRequester : ImageRequestThread
{
  
    string responses;

    List<string> keywords;
    GameObject quad;
    Vector3 cameraposition;
    Queue<ObjectData> objDataList;

    public ImageRequester(ref string responses, List<string> keywords, GameObject quad, Vector3 cameraposition, ref Queue<ObjectData> objDataList)
    {
        this.responses = responses;
        this.keywords = keywords;
        this.quad = quad;
        this.cameraposition = cameraposition;
        this.objDataList = objDataList;
    }



    protected override void Run()
    {


        // Debug.Log(responses);
        JSONNode res = JSON.Parse(responses.Replace("\n", "").Replace(" ", ""));

        var itemcount = res["responses"][0]["localizedObjectAnnotations"].Count;
        Vector2 topLeft;
        Vector2 botRight;
        int j = 0;

        for (int i = 0; i < itemcount; i++)
        {

            string name;
            try
            {
                j = i;
                name = res["responses"][0]["localizedObjectAnnotations"][i]["name"].ToString().Trim('"');
                if (!keywords.Contains(name))
                {
                    Debug.Log("Found Object: " + name + " , is not in Keyword list. Skipping.");
                    continue;
                }
                if (CloudVisionResultManager.isValidated(name))
                {
                    Debug.Log("Found Object: " + name + " , is already validated.");
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


            //calculate WorldPosition of the rectangle
            var centerPixel = GetWorldCoordinateFromPixel(quad, topLeft, botRight);//getCenter(pos1, pos2));

            // calculate direction for the ray
            var pixelDir = centerPixel - cameraposition;

            objDataList.Enqueue(new ObjectData(name, pixelDir, cameraposition));




            //Debug.DrawRay(currentPos, pixelDir, Color.red, 5000);

        }
        AsyncToConsole.addMessage("FinishedCoordinate calculation");
        Stop();

    }










    private Vector3 GetWorldCoordinateFromPixel(GameObject quad, Vector2 topLeft, Vector2 botRight)
    {
        Vector2 pixelCenter = ((topLeft + botRight) / 2.0f);


        Debug.Log("calculatingWorldCoordinate for quadpos = " + quad.transform.position + " and Coordinate +" + pixelCenter);
        // abstracted from https://answers.unity.com/questions/228486/an-easy-way-to-get-the-global-position-from-a-chil.html

        var position = new Vector2(quad.transform.position.x, quad.transform.position.y);

        Vector3 scale= quad.transform.localScale;

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




}
