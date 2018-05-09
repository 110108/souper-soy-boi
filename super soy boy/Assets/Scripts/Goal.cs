using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour {

    public AudioClip goalClip;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            var audioSource = GetComponent<AudioSource>();
            if (audioSource != null && goalClip != null)
            {
                audioSource.PlayOneShot(goalClip);
            }
            //GameManager.instance.RestartLevel(0.5f);
            //Find the timer script and refrence it
            //var timer = FindObjectOfType<Timer>();
            //Call the save time method in the game manager script and pass through the time from the timer script
            //GameManager.instance.RestartLevel(0.5f);
			System.Threading.Thread.Sleep(1000);
			SceneManager.LoadScene("Menu");
        }
    }
}
