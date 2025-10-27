using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cutscenes
{
    public class WinCutscenePlayer : MonoBehaviour
//     {
//         [Header("References")]
//         public Animator animator;      // assign your Animator
//         public AnimationClip clip;     // assign your Win animation clip
//
//         [Header("After playback")]
//         public string nextSceneName = "MenuScene";  // menu to return to
//         public float holdOnEnd = 0.3f;              // small pause after animation
//         public float inputWarmup = 0.25f;           // ignore first click
//
//         void Reset() { animator = GetComponent<Animator>(); }
//
//         
//         void Start()
//         {
//             if (!animator) animator = GetComponent<Animator>();
//             StartCoroutine(PlayThenExit());
//         }
//
//         IEnumerator PlayThenExit()
//         {
//             // Duration fallback if no clip assigned
//             float duration = clip ? clip.length : 2.5f;
//             float elapsed = 0f;
//
//             while (elapsed < duration + holdOnEnd)
//             {
//                 elapsed += Time.unscaledDeltaTime;
//
//                 // Allow skipping after a short warm-up
//                 if (elapsed > inputWarmup &&
//                     (Input.GetKeyDown(KeyCode.Space) ||
//                      Input.GetKeyDown(KeyCode.Return) ||
//                      Input.GetMouseButtonDown(0)))
//                 {
//                     break;
//                 }
//                 yield return null;
//             }
//
//             SceneManager.LoadScene(nextSceneName);
//         }
//     }
// }
    {

        [Header("References")] public Animator animator; // assign your Animator
        public AnimationClip clip; // assign the Win clip

        [Header("Playback")] [Tooltip("Animator playback speed. 0.5 = half speed.")]
        public float playbackSpeed = 0.5f; // <-- half speed

        [Header("After playback")] public string nextSceneName = "MenuScene";
        public float holdOnEnd = 0.3f;
        public float inputWarmup = 0.25f;

        void Reset()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            // ensure normal time and deterministic start
            Time.timeScale = 1f;
            if (!animator) animator = GetComponent<Animator>();

            // drive animator on unscaled time so our waits match inputWarmup exactly
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.speed = Mathf.Max(0.001f, playbackSpeed);

            if (clip) {
                // make sure we start from frame 0 of the intended state
                animator.Play(clip.name, 0, 0f);
            }

            StartCoroutine(PlayThenExit());
        }

        IEnumerator PlayThenExit()
        {
            // wait until the current state finishes (normalizedTime >= 1)
            // works for any speed and avoids hard-coding clip.length
            float warm = inputWarmup;

            while (true) {
                // allow user to skip after a short warm-up
                if (warm <= 0f &&
                    (Input.GetKeyDown(KeyCode.Space) ||
                     Input.GetKeyDown(KeyCode.Return) ||
                     Input.GetMouseButtonDown(0)))
                    break;

                warm -= Time.unscaledDeltaTime;

                if (animator && clip) {
                    var st = animator.GetCurrentAnimatorStateInfo(0);
                    // finished when not transitioning and we've reached the end once
                    if (!animator.IsInTransition(0) && st.normalizedTime >= 1f)
                        break;
                }
                else {
                    // fallback safety in case references are missing
                    break;
                }

                yield return null;
            }

            if (holdOnEnd > 0f)
                yield return new WaitForSecondsRealtime(holdOnEnd);

            SceneManager.LoadScene(nextSceneName);
        }
    }
}