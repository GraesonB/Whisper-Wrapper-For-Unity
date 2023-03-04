using UnityEngine;
using System.Collections.Generic;
using System.IO;
using OpenAIRequests;

namespace Whisper {

    public class Whisper : MonoBehaviour
    {
        [SerializeField]
        private string _apiKey = null;
        private string _uri = "https://api.openai.com/v1/audio/transcriptions";
        private List<(string, string)> _reqHeaders;
        private string _text;


        private Requests requests = new Requests();
        private void OnEnable() {
          _reqHeaders = new List<(string, string)>
            { 
                ("Authorization", $"Bearer {_apiKey}")
            };
        }

        public void SendToWhisper(string path)
        {
          string fileName = Path.GetFileName(path);
          string extention = Path.GetExtension(path);
          string mimeType;
          switch(extention) {
            case ".mp3":
              mimeType = "audio/mpeg";
              break;
            case ".m4a": case ".mp4":
              mimeType = "audio/mp4";
              break;
            case ".wav":
              mimeType = "audio/x-wav";
              break;
            default:
              Debug.LogError("Invalid audio file type.");
              return;
          }
          WWWForm form = BuildWhisperForm(path, fileName, mimeType);
          StartCoroutine(requests.PostFormReq<WhisperRes>(_uri, form, Resolve, _reqHeaders));    
        }

        private WWWForm BuildWhisperForm(string path, string fileName, string mimeType) {
          WWWForm form = new WWWForm();
          form.AddField("model", "whisper-1");
          byte[] file = File.ReadAllBytes(Application.dataPath +  path);
          form.AddBinaryData("file", file, fileName, mimeType);
          return form;
        }

        private void Resolve(WhisperRes res)
        {
            _text = res.text;
            Debug.Log(_text);
        }
    }
}