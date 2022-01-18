using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;

public class Camera_cube : MonoBehaviour
{
    // Start is called before the first frame update
    private WebCamTexture backCam;
    private bool camAvailable;
    private Texture defaulfImage;

    public RawImage Image;
    public AspectRatioFitter fit;

    CascadeClassifier cascade;

    public OpenCvSharp.Rect MyFace;

    player_movement player_Movement;

    void Start()
    {
        defaulfImage = Image.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            camAvailable = false;
            return;
        }

        for (int i=0; i<devices.Length; i++)
        {
            //if (!devices[i].isFrontFacing)
            //{
                backCam = new WebCamTexture(devices[0].name);
            //}
        }
        if (backCam==null)
        {
            Debug.Log("Unable to find back camera");
            return;

        }
        backCam.Play();
        Image.texture = backCam;

        cascade = new CascadeClassifier(Application.dataPath + @"\OpenCV+Unity\Demo\Face_Detector\haarcascade_frontalface_default.xml");

        camAvailable = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!camAvailable)
        //{
        //    return;
        //}
        //float ratio = (float)backCam.width / (float)backCam.height;
        //fit.aspectRatio = ratio;

        //float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        //Image.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        //int orient = -backCam.videoRotationAngle;
        //Image.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        GetComponent<Renderer>().material.mainTexture = backCam;//use Web cam texture as the render texture
        Mat frame = OpenCvSharp.Unity.TextureToMat(backCam);// store current frame
        FindNewFace(frame);
        Display(frame);
    }

    void FindNewFace(Mat frame)
    {
        //cascade to detect a matching image of a face and store it, detects a face and puts it in a rectangle
        var faces = cascade.DetectMultiScale(frame, 1.1, 2, HaarDetectionType.ScaleImage);
        if (faces.Length >= 1)
        {
            //Debug.Log(faces[0].Location);
            MyFace = faces[0];
            //player_Movement.GetComponent
            //p.MyFace = MyFace;
        }
    }

    void Display(Mat frame)
    {
        if (MyFace != null) //if a face is detected create a rectangle
        {

            frame.Rectangle(MyFace, new Scalar(0, 0, 255), 2);
            //Debug.Log(frame);
        }
        
        Texture newtexture = OpenCvSharp.Unity.MatToTexture(frame); //update renderer
        Image.texture = newtexture;
        GetComponent<Renderer>().material.mainTexture = newtexture;
    }
}
