using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Sayollo {
    public static class ApiRequest
    {
        private class ApiRequestMonoBehaviour : MonoBehaviour { }

        private static ApiRequestMonoBehaviour apiRequestMonoBehaviour;

        //Initialize MonoBehaviour object for starting coroutine
        private static void Init()
        {
            if (apiRequestMonoBehaviour == null)
            {
                GameObject gameObject = new GameObject("ApiRequests");
                apiRequestMonoBehaviour = gameObject.AddComponent<ApiRequestMonoBehaviour>();
            }
        }

        //GET request
        public static void Get(string url, Action<string> onSuccess, Action<string> onError)
        {
            Init();
            apiRequestMonoBehaviour.StartCoroutine(GetCoroutine(url, onSuccess, onError));
        }

        private static IEnumerator GetCoroutine(string url, Action<string> onSuccess, Action<string> onError)
        {
            using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(url))
            {

                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
                {
                    onError(unityWebRequest.error);
                }
                else
                {
                    onSuccess(unityWebRequest.downloadHandler.text);
                }
            }
        }

        //POST request with json
        public static void PostJson(string url, object data, Action<string> onSuccess, Action<string> onError)
        {
            Init();
            apiRequestMonoBehaviour.StartCoroutine(PostJsonCoroutine(url, data, onSuccess, onError));
        }

        private static IEnumerator PostJsonCoroutine(string url, object data, Action<string> onSuccess, Action<string> onError)
        {
            string json = JsonUtility.ToJson(data);

            using (UnityWebRequest unityWebRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                byte[] jsonRaw = Encoding.UTF8.GetBytes(json);
                unityWebRequest.uploadHandler = new UploadHandlerRaw(jsonRaw);
                unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

                unityWebRequest.SetRequestHeader("Content-Type", "application/json");

                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
                {
                    onError(unityWebRequest.error);
                }
                else
                {
                    onSuccess(unityWebRequest.downloadHandler.text.Replace("'", "\""));
                }
            }
        }

        //GET request Texture2D
        public static void GetTexture(string url, Action<Texture2D> onSuccess, Action<string> onError)
        {
            Init();
            apiRequestMonoBehaviour.StartCoroutine(GetTextureCoroutine(url, onSuccess, onError));
        }

        private static IEnumerator GetTextureCoroutine(string url, Action<Texture2D> onSuccess, Action<string> onError)
        {
            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
                {
                    onError(unityWebRequest.error);
                }
                else
                {
                    DownloadHandlerTexture downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                    onSuccess(downloadHandlerTexture.texture);
                }
            }
        }

        //GET file with progress
        public static void GetVideoProgress(string url, Action<string> onSuccess, Action<string> onError, Action<int> onProgress)
        {
            Init();
            apiRequestMonoBehaviour.StartCoroutine(GetVideoProgressCoroutine(url, onSuccess, onError, onProgress));
        }

        private static IEnumerator GetVideoProgressCoroutine(string url, Action<string> onSuccess, Action<string> onError, Action<int> onProgress)
        {
            using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(url))
            {

                unityWebRequest.SendWebRequest();

                string fileName = GetFileNameFromUrl(url);

                string path;
                if (fileName == null)
                {
                    //TODO: We don't know expected file extension
                    path = Path.Combine(Application.dataPath, "Video", "Video.webm");
                }
                else
                {
                    path = Path.Combine(Application.dataPath, "Video", fileName);
                }



                Debug.Log("ApiRequest: fileName: " + fileName + ", path: " + path);

                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }

                while (!unityWebRequest.isDone)
                {
                    int downloadPercent = (int)(unityWebRequest.downloadProgress * 100f);
                    onProgress(downloadPercent);
                    yield return null;
                }

                if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
                {
                    onError(unityWebRequest.error);
                }
                else
                {
                    File.WriteAllBytes(path, unityWebRequest.downloadHandler.data);
                    onSuccess(path);
                }
            }
        }

        //Convert URL to valid URI, then get fileName
        public static string GetFileNameFromUrl(string url)
        {
            Uri uri;

            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                try
                {
                    uri = new Uri(url);
                }
                catch (UriFormatException e)
                {
                    Debug.LogError("Error: " + e);
                }
            }

            return Path.GetFileName(uri?.LocalPath);
        }
    }

}