using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace Sayollo {
    public class VideoAd : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro textMesh;
        [SerializeField]
        private GameObject videoHolder;
        [SerializeField]
        private GameObject progressIndicator;

        //Urls
        [TextArea]
        [SerializeField]
        private string adURL;
        private string videoUrl;

        //Retry
        [Tooltip("Maximum number of retry on error")]
        [SerializeField]
        private int maxRetryCount;
        [SerializeField]
        private float retryThrottle = 15f;
        [SerializeField]
        private bool needRetry;

        private string videoFilePath;
        private VideoPlayer videoPlayer;
        private int retryCounter;

        private void Start()
        {
            GetAd();

        }

        private void GetAd()
        {
            ApiRequest.Get(adURL,
                (string success) => {
                    Debug.Log("Received: " + success);
                    VAST vast = ParseXML(success, typeof(VAST)) as VAST;

                    retryCounter = 0;


                    videoUrl = vast.Ad.InLine.Creatives.Creative.Linear.MediaFiles.MediaFile;
                    DownloadVideoAd();


                },
                (string error) => {
                    Debug.Log("Ad Get Error: " + error);

                    if (needRetry && retryCounter < maxRetryCount)
                    {
                        Invoke("GetAd", retryThrottle);
                        retryCounter++;
                    }
                    else
                    {
                        textMesh?.SetText("Something went wrong");
                        progressIndicator.SetActive(false);
                    }

                }
            );
        }

        private object ParseXML(string xml, Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            using (TextReader reader = new StringReader(xml))
            {
                return serializer.Deserialize(reader);
            }
        }

        private void DownloadVideoAd()
        {
            textMesh?.gameObject.SetActive(true);

            ApiRequest.GetVideoProgress(videoUrl,
                (string success) => {
                    videoPlayer = videoHolder.AddComponent<VideoPlayer>();
                    videoPlayer.errorReceived += VideoPlayer_errorListener;

                //TODO: Change for different render pipeline
                videoPlayer.targetMaterialProperty = "_BaseMap";
                    videoPlayer.isLooping = true;

                    Debug.Log("Video download success: " + success);
                    videoFilePath = success;
                    videoPlayer.url = success;
                    textMesh?.gameObject.SetActive(false);
                    retryCounter = 0;
                    progressIndicator.SetActive(false);
                },
                (string error) => {
                    Debug.Log("Video download Error: " + error);
                    textMesh?.SetText("Something went wrong");
                    if (needRetry && retryCounter < maxRetryCount)
                    {
                        Invoke("DownloadVideoAd", retryThrottle);
                        retryCounter++;
                    }
                    else
                    {
                        textMesh?.SetText("Something went wrong");
                        progressIndicator.SetActive(false);
                    }
                },
                (int progress) => {
                    Debug.Log("Progress: " + progress);
                    textMesh?.SetText(progress + "%");
                }
            );
        }

        private void VideoPlayer_errorListener(VideoPlayer source, string message)
        {
            Debug.LogError("VideoPlayer_errorReceived: message: " + message);
            source.Stop();
            textMesh?.gameObject.SetActive(true);
            textMesh?.SetText("Something went wrong");
            videoPlayer.errorReceived -= VideoPlayer_errorListener;//Unregister to avoid memory leaks
        }

        private void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit: delete video ad: videoFilePath: " + videoFilePath);
            if (videoFilePath != null && File.Exists(videoFilePath))
            {
                File.Delete(videoFilePath);
            }

        }
    }
}