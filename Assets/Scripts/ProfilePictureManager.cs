using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using PlayFab;
using PlayFab.ClientModels;
using ImageCropperNamespace;

public class ProfilePictureManager : MonoBehaviour
{
   // public static ProfilePictureManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private RawImage profilePictureDisplay;
    [SerializeField] private Button changeProfileButton;
    [SerializeField] private GameObject loadingIndicator;

    private const string PROFILE_PIC_KEY = "ProfilePicture"; // PlayFab user data key
    private Texture2D croppedTexture;

    private void Awake()
    {
     //   if (Instance != null && Instance != this)
      //  {
        //    Destroy(gameObject);
          //  return;
  //      }

    //    Instance = this;
      //  DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        changeProfileButton.onClick.AddListener(OnChangeProfileButtonClicked);
        // LoadProfilePictureFromPlayFab();
    }

    IEnumerator GetplayfabID(float del) 
    {
        yield return new WaitForSeconds(del);
    }

    private void OnChangeProfileButtonClicked()
    {
#if UNITY_ANDROID || UNITY_IOS
        //NativeFilePicker.PickImage(OnImageSelected);
        NativeFilePicker.PickFile(OnImageSelected);
#else
        string path = UnityEditor.EditorUtility.OpenFilePanel("Select Profile Picture", "", "png,jpg,jpeg");
        if (!string.IsNullOrEmpty(path))
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            OpenCropper(texture);
        }
#endif
    }

    private void OnImageSelected(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        byte[] imageBytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);
        OpenCropper(texture);
    }

    private void OpenCropper(Texture2D texture)
    {
        ImageCropper.Instance.Show(texture, (bool result, Texture originalImage, Texture2D croppedImage) =>
        {
            if (result)
            {
                croppedTexture = croppedImage;
                profilePictureDisplay.texture = croppedTexture;
                StartCoroutine(UploadProfilePictureToPlayFab(croppedTexture));
            }
        },
        new ImageCropper.Settings()
        {
            ovalSelection = true,
            selectionMinAspectRatio = 1f,
            selectionMaxAspectRatio = 1f,
            imageBackground = Color.black
        });
    }

    private IEnumerator UploadProfilePictureToPlayFab(Texture2D texture)
    {
        //ShowLoading(true);

        // ✅ Make sure texture is readable before encoding
        Texture2D readableTexture = MakeTextureReadable(texture);

        // ✅ Now safely encode the readable copy to PNG
        byte[] imageBytes = readableTexture.EncodeToPNG();
        string base64String = System.Convert.ToBase64String(imageBytes);

        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>()
        {
            { PROFILE_PIC_KEY, base64String }
        },
            Permission = UserDataPermission.Public // 👈 THIS is the important line
        };

        bool isDone = false;

        PlayFabClientAPI.UpdateUserData(request,
        result =>
        {
            Debug.Log("✅ Profile picture uploaded successfully!");

            // ✅ Save locally too, so we can instantly display it next time without re-downloading
            PlayerPrefs.SetString("ProfilePic", base64String);
            PlayerPrefs.Save();

            isDone = true;
        },
        error =>
        {
            Debug.LogError("❌ Failed to upload profile picture: " + error.GenerateErrorReport());
            isDone = true;
        });

        yield return new WaitUntil(() => isDone);
        ShowLoading(false);
    }


    public void LoadProfilePictureFromPlayFab(string playFabId = null)
{
    //ShowLoading(true);

    var request = new GetUserDataRequest();

    // 👇 If playFabId is provided, load that player's data. Otherwise, load your own.
    if (!string.IsNullOrEmpty(playFabId))
        request.PlayFabId = playFabId;

    PlayFabClientAPI.GetUserData(request, result =>
    {
        ShowLoading(false);

        if (result.Data != null && result.Data.ContainsKey(PROFILE_PIC_KEY))
        {
            string base64String = result.Data[PROFILE_PIC_KEY].Value;
            byte[] imageBytes = System.Convert.FromBase64String(base64String);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            profilePictureDisplay.texture = texture;

            Debug.Log("✅ Loaded profile picture for PlayFabId: " + (playFabId ?? "Local Player"));
        }
        else
        {
            Debug.LogWarning("⚠️ No ProfilePic found for PlayFabId: " + (playFabId ?? "Local Player"));
        }
    },
    error =>
    {
        ShowLoading(false);
        Debug.LogError("❌ Failed to load profile picture: " + error.GenerateErrorReport());
    });
}


    private void ShowLoading(bool show)
    {
        if (loadingIndicator)
            loadingIndicator.SetActive(show);
    }

    private Texture2D MakeTextureReadable(Texture2D source)
    {
        RenderTexture tmp = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(source, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;

        Texture2D readableTexture = new Texture2D(source.width, source.height);
        readableTexture.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        return readableTexture;
    }

}
