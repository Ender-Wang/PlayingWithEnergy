using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

struct Point {
    public float pixX;
    public float pixY;

    public float longtitude;
    public float latitude;

    public float worldPosX;
    public float worldPosY;

    public string toString() {
        string content = string.Format("{0} | {1}; {2} | {3}; {4} | {5}", pixX, pixY, longtitude, latitude, worldPosX, worldPosY);
        return content;
    }
}

public class PicDataPreprocessor
{
    // drop your file here in inspector
    public Sprite img;
    public TextAsset pointInfo;

    private Point keyPoint1;
    private Point keyPoint2;

    // distance in the real world represented by the picture
    private float picWidth;
    private float picHeight;

    // real world position of these 2 corners of the image
    private Point leftTop;
    private Point rightBottom;
    
    private float meterPerPixel;

    private string semanticName;

    public PicDataPreprocessor(string semanticName, TextAsset csv, Sprite img) {
        this.semanticName = semanticName;
        this.pointInfo = csv;
        this.img = img;
        LoadData();
    }

    void LoadData()
    {   
        loadPicFile();
        PictureInfo();
    }

    void loadPicFile()
    {
        Stream filePath = GenerateStreamFromString(pointInfo.text);
        StreamReader reader = new StreamReader(filePath);
        reader.ReadLine();//skip the header (first line)

        string line = reader.ReadLine();
        ParseContent(ref keyPoint1, line);
        line = reader.ReadLine();
        ParseContent(ref keyPoint2, line);
        reader.Close();
    }

    /// <summary>
    /// return a color of the calculated pixel
    /// </summary>
    /// <param name="x">the real world position on x axis</param>
    /// <param name="y">the real world position on y axis</param>
    /// <returns>RGB color</returns>
    public Color getColor(float x, float y) {
        int horizontal = Mathf.FloorToInt((x - leftTop.worldPosX) / meterPerPixel);
        int vertical = Mathf.FloorToInt((leftTop.worldPosY - y) / meterPerPixel);
        // unity read image from bottom to top, from left to right
        return img.texture.GetPixel(horizontal, img.texture.height - vertical);
    }

    public void GetPixel(float x, float y) {
        int horizontal = Mathf.FloorToInt((x - leftTop.worldPosX) / meterPerPixel);
        int vertical = Mathf.FloorToInt((leftTop.worldPosY - y) / meterPerPixel);
    }
    /// <summary>
    /// check if the given real world position is in the range represented by the img
    /// </summary>
    /// <param name="x">building real world x pos</param> 
    /// <param name="y">building real world y pos</param>
    /// <returns></returns>
    public bool isInrange(float x, float y) {
        if (x < leftTop.worldPosX || x > rightBottom.worldPosX)
            return false;
        if (y < rightBottom.worldPosY || y > leftTop.worldPosY)
            return false;
        return true;
    }
    
    /// <summary>
    /// calculate related information of the <param name="img"></param>
    /// </summary>
    private void PictureInfo() {
        float width = img.bounds.size.x * img.pixelsPerUnit;
        float height = img.bounds.size.y * img.pixelsPerUnit;
        picWidth = Mathf.Abs(keyPoint1.worldPosX - keyPoint2.worldPosX) * width / Mathf.Abs(keyPoint1.pixX - keyPoint2.pixX);
        picHeight = Mathf.Abs(keyPoint1.worldPosY - keyPoint2.worldPosY) * height / Mathf.Abs(keyPoint1.pixY - keyPoint2.pixY);
        
        meterPerPixel = picWidth / width;
        leftTop.pixX = leftTop.pixY = 0;
        leftTop.worldPosX = keyPoint1.worldPosX - keyPoint1.pixX * meterPerPixel;
        leftTop.worldPosY = keyPoint1.worldPosY + keyPoint1.pixY * meterPerPixel;

        rightBottom.worldPosX = keyPoint1.worldPosX + (width - keyPoint1.pixX) * meterPerPixel;
        rightBottom.worldPosY = keyPoint1.worldPosY - (height - keyPoint1.pixY) * meterPerPixel;
    }

    /// <summary>
    /// parse csv file content, save in Point
    /// </summary>
    /// <param name="point"></param>
    /// <param name="line">content line of csv file</param>
    private void ParseContent(ref Point point, string line) {
        char delimiter = '|';
        line = Regex.Replace(line, "[\", °]", string.Empty);
        string[] content = line.Split(';');
        string[] values = content[0].Split(delimiter);
        // pixel point in the image
        point.pixX = int.Parse(values[0]);
        point.pixY = int.Parse(values[1]);

        // longtitude and latitude of that pixel point
        values = content[1].Split(delimiter);
        point.longtitude = float.Parse(values[0]);
        point.latitude = float.Parse(values[1]);

        // world position of that pixel point
        values = content[2].Split(delimiter);
        point.worldPosX = float.Parse(values[0]);
        point.worldPosY = float.Parse(values[1]);
    }

    private Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
