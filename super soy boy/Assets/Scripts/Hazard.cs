using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour {

    public GameObject playerDeathPrefab;
    public AudioClip deathClip;
    public Sprite hitSprite;
	public Sprite bladeSprite;
    private SpriteRenderer sr;

    void Awake()
    {
		sr = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start () {
	
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.tag == "Player")
        {
            var audioSource = GetComponent<AudioSource>();
            if (audioSource != null && deathClip != null)
            {
                audioSource.PlayOneShot(deathClip);
            }
            Instantiate(playerDeathPrefab, coll.contacts[0].point,
              Quaternion.identity);
           	sr.sprite = hitSprite;
			SoyBoyController obj = new SoyBoyController();
			coll.transform.position = obj.startPos;
			GameObject.Find("SoyBoy").GetComponent <SoyBoyController>().ded = true;
			sr.sprite = bladeSprite;
//            Destroy(coll.gameObject);
//            GameManager.instance.RestartLevel(1.25f);
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
