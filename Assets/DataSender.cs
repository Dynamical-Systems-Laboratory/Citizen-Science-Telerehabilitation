using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;
using System.Text;
using System.Linq;


public class DataSender : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        //GoogleDriveFiles.Create(file).Send();

        //var file = new UnityGoogleDrive.Data.File { Name = "user-0", MimeType = "application/vnd.google-apps.folder"};
        //var parentID = UnityGoogleDrive.Helpers.CreateOrUpdateFileAtPathAsync(file, "/Test/user-0");

        var bytes = System.IO.File.ReadAllBytes("Builds/Data/User-5/20190716_IMU_Data.csv");
        var file = new UnityGoogleDrive.Data.File { Name = "20190716_IMU_Data.csv", Content = bytes };
        UnityGoogleDrive.Helpers.CreateOrUpdateFileAtPathAsync(file, "/Test/user-0/20190716_IMU_Data.csv");

        //bytes = System.IO.File.ReadAllBytes("Builds/Data/User-5/20190716_Kinect_Data.csv");
        //file = new UnityGoogleDrive.Data.File { Name = "20190716_Kinect_Data.csv", Content = bytes };
        //UnityGoogleDrive.Helpers.CreateOrUpdateFileAtPathAsync(file, "/Test/user-0/20190716_Kinect_Data.csv");

        //UnityGoogleDrive.Helpers.CreateOrUpdateFileAtPathAsync(file, "/Test/20190716_IMU_Data.csv");





    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
