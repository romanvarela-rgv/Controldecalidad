using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine;

public class VideoEndSceneChanger : MonoBehaviour
{
    public VideoPlayer videoPlayer; 

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(8);
    }
}
