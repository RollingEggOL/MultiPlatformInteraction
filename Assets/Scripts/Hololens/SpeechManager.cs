using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;

public class SpeechManager : Singleton<SpeechManager>
{
    private KeywordRecognizer keywordRecognizer;
    private delegate void KeywordAction(PhraseRecognizedEventArgs args);
    private Dictionary<string, KeywordAction> keywordCollection;

    private MultiNetworkManager networkManager;

    public bool IsNetworkScene
    {
        get
        {
            return SceneManager.GetActiveScene().name == "Main_Network";
        }
    }

    private void Start()
    {

        keywordCollection = new Dictionary<string, KeywordAction>();

        if (IsNetworkScene)
        {
            networkManager = GameObject.Find("NetworkManager").GetComponent<MultiNetworkManager>();
            keywordCollection.Add("Start Host", ((PhraseRecognizedEventArgs args) =>
            {
                networkManager.StartHost();
            }));

            keywordCollection.Add("Stop Host", ((PhraseRecognizedEventArgs args) =>
            {
                networkManager.StopHost();
            }));
        }

        keywordRecognizer = new KeywordRecognizer(keywordCollection.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognizer;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognizer(PhraseRecognizedEventArgs args)
    {
        KeywordAction keywordAction;
        if (keywordCollection.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke(args);
        }
    }

    private void OnDestroy()
    {
        keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognizer;
        keywordRecognizer.Dispose();
    }
}