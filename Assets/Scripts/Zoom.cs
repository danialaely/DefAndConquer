using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Zoom : MonoBehaviour
{
    //public Camera mainCamera; // Reference to your main camera
    //private float originalOrthographicSize;
    //private Vector3 originalCamPos;

    //public float maxDoubleTapTime = 0.3f; // Maximum time allowed between taps
    //private float lastTapTime; // Time of the last tap
    //private static bool isBoolVariableTrue = false; // Initial value of the boolean variable

   // public Image dice1;
   // public Image dice2;
    //Vector3 originaldice1Pos;
    //Vector3 originaldice2Pos;

   // Vector3 zoomPos;
    // Vector3 zoomOut;

    public AudioSource src;
    public AudioClip shuffleClip;
    public AudioClip errorClip;

    public GameManager gm;

    public GameObject pausePanel;
    private bool isPaused;

    private void Start()
    {
        isPaused = false;
        pausePanel.SetActive(false);
    }

    //private Vector3 Origin;
    // private Vector3 Difference;
    //private Vector3 ResetCamera;

    // private bool drag = false;

    // private bool stickToPos;

    /*
    private void Start()
    {
        originaldice1Pos = dice1.transform.position;
        originaldice2Pos = dice2.transform.position;

        stickToPos = false;

        if (mainCamera == null)
        {
            // If the mainCamera reference is not set, try to find the main camera in the scene
            mainCamera = Camera.main;
        }

        // Store the original orthographic size for resetting
        if (mainCamera != null)
        {
            originalOrthographicSize = mainCamera.orthographicSize;
            originalCamPos = mainCamera.transform.position;
        }
    }

    private IEnumerator Fluctuate(float delay)
    {
        yield return new WaitForSeconds(delay);
        stickToPos = !stickToPos;
    }

    void Update()
    {

        // mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 400f, 0.0123f);
        // Check for a tap or click
        if (Input.GetMouseButtonDown(0)) // Assuming left mouse button for simplicity
        {
            // Check if it's a double tap
            if (Time.time - lastTapTime < maxDoubleTapTime && mainCamera != null)
            {
                // Double tap detected
                Debug.Log("Double Tap!");

                // Toggle the boolean variable
                isBoolVariableTrue = !isBoolVariableTrue;
                // stickToPos = !stickToPos;
                StartCoroutine(Fluctuate(2f));
                Debug.Log("SticToPos:" + stickToPos);

                if (isBoolVariableTrue)
                {
                    Debug.Log("Boolean variable is true. Performing zoom in.");

                    // Get the position of the touch/click on the screen
                    Vector3 touchPos = Input.mousePosition;
                    touchPos.z = -10f; // Set a fixed distance from the camera


                    Vector3 touchPoz = Input.mousePosition;
                    touchPoz.z = 400f;
                    touchPoz.x += 90f;

                    // Convert screen space to world space
                    Vector3 worldPos = mainCamera.ScreenToWorldPoint(touchPos);


                    Vector3 offt1 = new Vector3(-400f, 0, 0);
                    dice1.transform.position = Input.mousePosition + offt1;
                    dice2.transform.position = Input.mousePosition - offt1;

                    // Set the target orthographic size
                    // float targetOrthographicSize = 411f;

                    // Smoothly interpolate to the target orthographic size and position
                    float transitionSpeed = 0.5f;

                    //mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetOrthographicSize, transitionSpeed);
                    zoomPos = Vector3.Lerp(mainCamera.transform.position, worldPos, transitionSpeed);

                }
            }

            // Update the last tap time
            lastTapTime = Time.time;
        }

        // Perform actions based on the boolean variable
        if (isBoolVariableTrue)
        {
            Debug.Log("Boolean variable is true. Performing zoom in.");

            // Set the target orthographic size
            float targetOrthographicSize = 411f;

            // Smoothly interpolate to the target orthographic size and position
            float transitionSpeed = 0.0925f;

            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetOrthographicSize, transitionSpeed);

            if (Input.GetMouseButton(0) == false && stickToPos == false)
            {
                //IF I HAVE NOT DRAGGED UP TILL NOW THEN ONLY IT WILL HAPPEN OTHERWISE IT WON'T EXECUTE 
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, zoomPos, 8f);
            }

            //HERE
            if (Input.GetMouseButton(0))
            {
                Difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - mainCamera.transform.position;
                if (drag == false)
                {
                    drag = true;
                    Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                drag = false;
                if (mainCamera.transform.position.x < -200f)
                {
                    mainCamera.transform.position += new Vector3(5f, 0, 0);
                }
                if (mainCamera.transform.position.x > 1230f)
                {
                    mainCamera.transform.position += new Vector3(-5f, 0, 0);
                }

                if (mainCamera.transform.position.y > 690f)
                {
                    mainCamera.transform.position += new Vector3(0, -5f, 0);
                }
                if (mainCamera.transform.position.y < -170f)
                {
                    mainCamera.transform.position += new Vector3(0, 5f, 0);
                }
            }

            if (drag == true)
            {
                if (mainCamera.transform.position.x > -200f && mainCamera.transform.position.x < 1230f && mainCamera.transform.position.y < 690f && mainCamera.transform.position.y > -170f)
                {
                    Camera.main.transform.position = Origin - Difference;
                }

                //stickToPos = true;
            }

        }
        else
        {
            Debug.Log("Boolean variable is false. Performing reset.");

            // Reset the camera to its original state
            if (mainCamera.orthographicSize <= 765f)
            {
                float targetOrthographicSize = 765.3f;
                float transitionSpeed = 0.0925f;
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetOrthographicSize, transitionSpeed);
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, originalCamPos, 8f);
                dice1.transform.position = originaldice1Pos;
                dice2.transform.position = originaldice2Pos;
            }
            else
            {
                mainCamera.orthographicSize = originalOrthographicSize;
                mainCamera.transform.position = originalCamPos;
                dice1.transform.position = originaldice1Pos;
                dice2.transform.position = originaldice2Pos;
            }
        }
    }


    public static bool GetBool()
    {
        return isBoolVariableTrue;
    }
    */

    public void DeckSound()
    {
        if (gm.currentPhase == GamePhase.Draw)
        {
            //src.clip = shuffleClip;
            //src.Play();
            AudioManager.instance.PlaySFX("Shuffle_fast");
        }
        if (gm.currentPhase == GamePhase.Play || gm.currentPhase == GamePhase.Move)
        {
            //src.clip = errorClip;
            //src.Play();
            AudioManager.instance.PlaySFX("Error");
        }
    }

    public void PauseResume() 
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0.0f;
        }
        else 
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }

    public void MainMenu() 
    {
        AudioManager.instance.continuedFromGame = true;
        SceneManager.LoadScene("Welcome");
    }

}
