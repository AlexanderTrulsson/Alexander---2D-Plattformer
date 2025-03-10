using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class VideoSequenceManager : MonoBehaviour
{
    public VideoClip firstClip;       // The first video clip.
    public VideoClip secondClip;      // The second video clip.
    public AudioClip customAudioClip; // Custom audio for the second clip.

    public VideoPlayer firstVideoPlayer;  // Assigned via Inspector.
    public VideoPlayer secondVideoPlayer; // Assigned via Inspector.
    public AudioSource audioSource;       // For custom audio.

    // UI elements for crossfade:
    public CanvasGroup firstCanvasGroup;  // CanvasGroup on the first RawImage.
    public CanvasGroup secondCanvasGroup; // CanvasGroup on the second RawImage.
    public float crossfadeDuration = 1.0f;  // Duration of the crossfade in seconds.

    void Start()
    {
        // Set up the first VideoPlayer to play the first clip normally.
        firstVideoPlayer.clip = firstClip;
        firstVideoPlayer.isLooping = false;
        firstVideoPlayer.loopPointReached += OnFirstClipEnd;
        firstVideoPlayer.Play();

        // Set up the second VideoPlayer to preload the second clip.
        secondVideoPlayer.clip = secondClip;
        secondVideoPlayer.isLooping = true;
        secondVideoPlayer.audioOutputMode = VideoAudioOutputMode.None;  // We'll use custom audio.
        secondVideoPlayer.Prepare();
        secondVideoPlayer.gameObject.SetActive(false); // Hide until needed.

        // Ensure UI RawImages are at the correct starting alpha.
        if (firstCanvasGroup != null)
            firstCanvasGroup.alpha = 1f;
        if (secondCanvasGroup != null)
            secondCanvasGroup.alpha = 0f;
    }

    void OnFirstClipEnd(VideoPlayer vp)
    {
        // Disable the first VideoPlayer's GameObject and enable the second.
        firstVideoPlayer.gameObject.SetActive(false);
        secondVideoPlayer.gameObject.SetActive(true);
        secondVideoPlayer.Play();

        // Start playing the custom audio.
        if (customAudioClip != null)
        {
            audioSource.clip = customAudioClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        // Begin the crossfade between the two UI displays.
        StartCoroutine(Crossfade());
    }

    IEnumerator Crossfade()
    {
        float elapsed = 0f;
        while (elapsed < crossfadeDuration)
        {
            float t = elapsed / crossfadeDuration;
            if (firstCanvasGroup != null)
                firstCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t); // Fade out first video.
            if (secondCanvasGroup != null)
                secondCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t); // Fade in second video.
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Ensure final values.
        if (firstCanvasGroup != null)
            firstCanvasGroup.alpha = 0f;
        if (secondCanvasGroup != null)
            secondCanvasGroup.alpha = 1f;
    }
}

