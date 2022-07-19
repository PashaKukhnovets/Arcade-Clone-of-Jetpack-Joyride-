using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseController : MonoBehaviour
{
    public float jetpackForce = 75.0f;
    public float forwardMovementSpeed = 3.0f;
    private Rigidbody2D _rigidbody2D;
    public Transform groundCheckTransform;
    private bool grounded;
    public LayerMask groundCheckLayerMask;
    Animator animator;
    public ParticleSystem jetpack;
    private bool dead = false;
    private uint coins = 0;
    public Text coinsCollectedLabel;
    public Button restartButton;
    public AudioClip coinCollectSound;
    public AudioSource jetpackAudio;
    public AudioSource footstepsAudio;
    public ParallaxScroll parallax;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    
    void FixedUpdate()
    {
        bool jetpackActive = Input.GetButton("Fire1");

        jetpackActive = jetpackActive && !dead;

        if (jetpackActive) {
            _rigidbody2D.AddForce(new Vector2(0, jetpackForce));
        }

        if (!dead)
        {
            Vector2 newVelocity = _rigidbody2D.velocity;
            newVelocity.x = forwardMovementSpeed;
            _rigidbody2D.velocity = newVelocity;
        }

        UpdateGroundedStatus();
        AdjustJetpack(jetpackActive);
        if (dead && grounded) {
            restartButton.gameObject.SetActive(true);
        }

        AdjustFootstepsAndJetpackSound(jetpackActive);
        parallax.offset = transform.position.x;
    }

    public void RestartGame() {
        SceneManager.LoadScene(0);
    }

    void UpdateGroundedStatus() {
        grounded = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundCheckLayerMask);
        animator.SetBool("grounded", grounded);
    }

    void AdjustJetpack(bool jetpackActive) {
        jetpack.enableEmission = !grounded;
        jetpack.emissionRate = jetpackActive ? 300.0f : 75.0f;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Coins")) {
            CollectCoin(collider);
        }
        else
            HitByLaser(collider);
    }

    void HitByLaser(Collider2D laserCollider) {
        if (!dead) {
            AudioSource laserZap = laserCollider.gameObject.GetComponent<AudioSource>();
            laserZap.Play();
        }
        dead = true;
        animator.SetBool("dead", true);
    }

    void CollectCoin(Collider2D coinCollider) {
        coins++;
        coinsCollectedLabel.text = coins.ToString();
        Destroy(coinCollider.gameObject);
        AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);
    }

    void AdjustFootstepsAndJetpackSound(bool jetpackActive) {
        footstepsAudio.enabled = !dead && grounded;
        jetpackAudio.enabled = !dead && !grounded;

        if (jetpackActive) {
            jetpackAudio.volume = 1.0f;
        }

        else {
            jetpackAudio.volume = 0.5f;
        }
    }
}
