using UnityEngine;
using System.Collections;
using SgLib;
using MidiJack;

public class PlayerController : MonoBehaviour
{
    public static event System.Action PlayerDied;

    public PianoKey pianoKey; 

    [Header("Gameplay Config")]
    public float jumpForce = 13f;
    //How high player jump
    public float rotateAngle = 50f;
    //Rotate angle of player
    [HideInInspector]
    public bool hasStarted = false;
    [HideInInspector]
    public bool hitObstacle;
    [HideInInspector]
    public bool playerFallDown = false;
    [HideInInspector]
    public bool hasHitGround = false;

    [Header("Gameplay Preferences")]
    public GameManager gameManager;
    public GameObject player;
    public ParticleSystem goldParticlePrefab;
    public ParticleSystem hitGroundParticlePrefab;
    public AnimationClip jump;
    public AnimationClip rotate;
    public Color gray;
      
    private Rigidbody rigid;
    private Animator anim;
    private int turn = 1;
    private bool isFinishRotate = false;
    private CameraController cameraController;

    void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;

        PianoKey.OnPianoKeyDown += MidiFlap;
    }

    public void MidiFlap(int note)
    {
        Flap();
    }

    void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }

    void GameManager_GameStateChanged(GameState newState, GameState oldState)
    {
        if (newState == GameState.Playing && oldState == GameState.Prepare)
        {
            if (!hasStarted)
            {
                hasStarted = true;
            }

            Flap();
        }
    }

    // Use this for initialization
    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();

        // Change the character to the selected one
        GameObject currentCharacter = CharacterManager.Instance.characters[CharacterManager.Instance.CurrentCharacterIndex];
        if (currentCharacter.transform.childCount > 0)
        {
            Mesh charMainMesh = currentCharacter.transform.Find("Main").GetComponent<MeshFilter>().sharedMesh;
            Mesh charRightWingMesh = currentCharacter.transform.Find("RightWing").GetComponent<MeshFilter>().sharedMesh;
            Mesh charLeftWingMesh = currentCharacter.transform.Find("LeftWing").GetComponent<MeshFilter>().sharedMesh;
            Material charMaterial = currentCharacter.transform.Find("Main").GetComponent<Renderer>().sharedMaterial;

            GameObject playerMain = player.transform.Find("Main").gameObject;
            GameObject playerRightWing = player.transform.Find("RightWing").gameObject;
            GameObject playerLeftWing = player.transform.Find("LeftWing").gameObject;

            playerMain.GetComponent<MeshFilter>().mesh = charMainMesh;
            playerMain.GetComponent<Renderer>().material = charMaterial;

            playerRightWing.GetComponent<MeshFilter>().mesh = charRightWingMesh;
            playerRightWing.GetComponent<Renderer>().material = charMaterial;

            playerLeftWing.GetComponent<MeshFilter>().mesh = charLeftWingMesh;
            playerLeftWing.GetComponent<Renderer>().material = charMaterial;
        }
        else
        {
            Mesh charMesh = currentCharacter.GetComponent<MeshFilter>().sharedMesh;
            Material charMaterial = currentCharacter.GetComponent<Renderer>().sharedMaterial;
            GameObject main = player.transform.Find("Main").gameObject;

            main.GetComponent<MeshFilter>().mesh = charMesh;
            main.GetComponent<Renderer>().material = charMaterial;
        }


        anim = player.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }
	
    // Update is called once per frame
    void Update()
    {
        //Check player fall out of camera
        if (Camera.main.WorldToScreenPoint(transform.position).y < -30 && GameManager.Instance.GameState != GameState.GameOver)
        {
            playerFallDown = true;
            anim.Play(rotate.name);
            Die();
        }

        //if (Input.GetMouseButtonDown(0) && GameManager.Instance.GameState == GameState.Playing)
        //{
        //    Flap();
        //}

       if( MidiDriver.Instance.GetKeyDown(MidiChannel.All,60) && GameManager.Instance.GameState == GameState.Playing)
       {
            Flap();
        }

        // Fix position
        transform.position = new Vector3(0, transform.position.y, 0);
    }

    void Flap()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.flap);
        StartCoroutine(AddVelocityForPlayer()); //add velocity for player
        if (!isFinishRotate)
        {
            isFinishRotate = true;
            StartCoroutine(RotateParentPlayer()); //rotate player
        }
    }

    void Die()
    {
        // Fire event
        if (PlayerDied != null)
        {
            PlayerDied();
        }
    }

    IEnumerator AddVelocityForPlayer()
    {
        yield return new WaitForFixedUpdate();
        rigid.velocity = new Vector3(0, jumpForce, 0);
        anim.SetTrigger(jump.name);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gold") //Hit gold
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.item);
            CoinManager.Instance.AddCoins(1);
            CreateParticle(goldParticlePrefab, other.transform.position);
            Destroy(other.gameObject);
        }
        else
        {
            // Hit obstacles
            if (!hitObstacle)
                cameraController.ShakeCamera();
        }

        if (other.tag == "NormalObstacle") //hit obstacle
        {
            if (!hitObstacle)
            {
                hitObstacle = true;

                Die();
                rigid.velocity = new Vector3(0, 0, 0);
                transform.position = new Vector3(0, transform.position.y, 0);

                //Create particle base on obstacle
                rigid.isKinematic = true;
                StartCoroutine(WaitToDisableKinematic());

                SoundManager.Instance.PlaySound(SoundManager.Instance.hit);
            }

        }
        else if (other.tag == "FireObstacle")
        {
            if (!hitObstacle)
            {
                hitObstacle = true;

                Die();
                rigid.velocity = new Vector3(0, 0, 0);
                transform.position = new Vector3(0, transform.position.y, 0);

                //Create particle base on obstacle
                rigid.isKinematic = true;
                CreateParticle(gameManager.fireParticle, transform.position);
                player.transform.Find("Main").GetComponent<Renderer>().material.SetColor("_Color", gray);
                player.transform.Find("LeftWing").GetComponent<Renderer>().material.SetColor("_Color", gray);
                player.transform.Find("RightWing").GetComponent<Renderer>().material.SetColor("_Color", gray);
                StartCoroutine(WaitToDisableKinematic());

                SoundManager.Instance.PlaySound(SoundManager.Instance.hit);
            }
        }
        else if (other.tag == "IceObstacle")
        {
            if (!hitObstacle)
            {
                hitObstacle = true;

                Die();
                rigid.velocity = new Vector3(0, 0, 0);
                transform.position = new Vector3(0, transform.position.y, 0);

                //Create particle base on obstacle
                rigid.isKinematic = true;
                GameObject icePrefab = Instantiate(gameManager.iceParticle, player.transform.position, Quaternion.identity) as GameObject;
                icePrefab.transform.parent = player.transform;
                icePrefab.transform.localPosition += new Vector3(-0.05f, 0, -0.2f);
                icePrefab.transform.localRotation = Quaternion.Euler(0, 0, 0);
                StartCoroutine(WaitToDisableKinematic());

                SoundManager.Instance.PlaySound(SoundManager.Instance.hit);
            }
        }
        else if (other.tag == "ElectricObstacle")
        {
            if (!hitObstacle)
            {
                hitObstacle = true;

                Die();
                rigid.velocity = new Vector3(0, 0, 0);  //stop player
                transform.position = new Vector3(0, transform.position.y, 0);

                //Create particle base on obstacle
                rigid.isKinematic = true;
                CreateParticle(gameManager.electricParticle, transform.position, true);
                StartCoroutine(WaitToDisableKinematic());

                SoundManager.Instance.PlaySound(SoundManager.Instance.hit);
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (GameManager.Instance.GameState == GameState.GameOver && col.collider.tag.Equals("TheGround") && !hasHitGround)
        {
            hasHitGround = true;
            SoundManager.Instance.PlaySound(SoundManager.Instance.hit);
            CreateParticle(hitGroundParticlePrefab, transform.position + new Vector3(0, -1f, 0));
        }
    }

    void CreateParticle(ParticleSystem particle, Vector3 position, bool attachToParent = false, float existTime = 5f)
    {
        ParticleSystem particleTemp = Instantiate(particle, position, particle.transform.rotation) as ParticleSystem;
        if (attachToParent)
        {
            particleTemp.transform.SetParent(player.transform);
        }
        particleTemp.Play();
        Destroy(particleTemp.gameObject, existTime);
    }

    IEnumerator RotateParentPlayer()
    {
        turn = turn * (-1);

        float firstCurrentAngle = transform.eulerAngles.y; //Y rotation = 0
        while (firstCurrentAngle < rotateAngle && GameManager.Instance.GameState == GameState.Playing) //Rotate 
        {
            float rotateAmount = 200 * Time.deltaTime;
            firstCurrentAngle += rotateAmount;
            if (turn < 0)
            {
                transform.Rotate(Vector3.up * rotateAmount);
            }
            else
            {
                transform.Rotate(Vector3.down * rotateAmount);
            }
            yield return null;
        }
        if (turn < 0)
        {
            transform.eulerAngles = new Vector3(0, rotateAngle, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, -rotateAngle, 0);
        }


        float secondCurrentAngle = transform.eulerAngles.y; //Y rotation = rotateAngle
        if (turn < 0)
        {
            while (secondCurrentAngle > 0 && GameManager.Instance.GameState == GameState.Playing)
            {
                float rotateAmount = 200 * Time.deltaTime;
                secondCurrentAngle -= rotateAmount;
                transform.Rotate(Vector3.down * rotateAmount);
                yield return null;
            }
        }
        else
        {
            while (secondCurrentAngle < 360 && GameManager.Instance.GameState == GameState.Playing)
            {
                float rotateAmount = 200 * Time.deltaTime;
                secondCurrentAngle += rotateAmount;
                transform.Rotate(Vector3.up * rotateAmount);
                yield return null;
            }
        }

        transform.eulerAngles = new Vector3(0, 0, 0);

        isFinishRotate = false;
    }

    IEnumerator WaitToDisableKinematic()
    {
        yield return new WaitForSeconds(0.5f);
        anim.Play(rotate.name);
        rigid.isKinematic = false;
    }

}
