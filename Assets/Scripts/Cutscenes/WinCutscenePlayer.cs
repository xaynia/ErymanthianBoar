using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cutscenes
{
    public class WinCutscenePlayer : MonoBehaviour
    {
        [Header("References")]
        public Animator animator;      // assign your Animator
        public AnimationClip clip;     // assign your Win animation clip

        [Header("After playback")]
        public string nextSceneName = "MenuScene";  // menu to return to
        public float holdOnEnd = 0.3f;              // small pause after animation
        public float inputWarmup = 0.25f;           // ignore first click

        void Reset() { animator = GetComponent<Animator>(); }

        void Start()
        {
            if (!animator) animator = GetComponent<Animator>();
            StartCoroutine(PlayThenExit());
        }

        IEnumerator PlayThenExit()
        {
            // Duration fallback if no clip assigned
            float duration = clip ? clip.length : 2.5f;
            float elapsed = 0f;

            while (elapsed < duration + holdOnEnd)
            {
                elapsed += Time.unscaledDeltaTime;

                // Allow skipping after a short warm-up
                if (elapsed > inputWarmup &&
                    (Input.GetKeyDown(KeyCode.Space) ||
                     Input.GetKeyDown(KeyCode.Return) ||
                     Input.GetMouseButtonDown(0)))
                {
                    break;
                }
                yield return null;
            }

            SceneManager.LoadScene(nextSceneName);
        }
    }
}