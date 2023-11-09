using System.Net.Http;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
public class CoordinateConverter
{   
    private string requestData;
    static Dictionary<int,string> GetHttpsContentAsString(string url) {
        Dictionary<int, string> res = new Dictionary<int, string>();
        HttpClient httpClient = new HttpClient();
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        HttpResponseMessage responseMessage = httpClient.SendAsync(requestMessage).Result;
        res.Add(((int)responseMessage.StatusCode), responseMessage.Content.ReadAsStringAsync().Result);
        return res;
    }

    public static Dictionary<int,string> LongLa2Pos(float longitude, float latitude) {
        string home = "https://epsg.io/srs/transform/";
        string tail = ".json?key=default&s_srs=4326&t_srs=25832";
        string url = home + longitude.ToString() + "," + latitude.ToString() + tail;
        return GetHttpsContentAsString(url);
    }

    public static Dictionary<int,string> Pos2LongLa(float x, float y) {
        string home = "https://epsg.io/srs/transform/";
        string tail = ".json?key=default&s_srs=25832&t_srs=4326";
        string url = home + x.ToString() + "," + y.ToString() + tail;
        return GetHttpsContentAsString(url);
    }

    // IEnumerator begin(){
    //     yield return StartCoroutine(getData("123"));
    //     Debug.Log("* CoordinateConverter -- -- -- "+requestData);
    // }
    // IEnumerator getData(string url){
    //     float longitude = 11.632854763147883f;
    //     float latitude = 48.09726382322415f;
    //     string home = "https://epsg.io/srs/transform/";
    //     string tail = ".json?key=default&s_srs=4326&t_srs=25832";
    //     url = home + longitude.ToString() + "," + latitude.ToString() + tail;
    //     using (UnityWebRequest request = UnityWebRequest.Get(url))
    //     {
    //         yield return request.SendWebRequest();
    //         if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    //         {
    //             Debug.Log("Network or HTTP Error!");
    //         }
    //         else
    //         {
    //             requestData = request.downloadHandler.text.ToString();
    //         }
    //     }
    // }
}
