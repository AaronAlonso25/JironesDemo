using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace NeonBlood
{
    /// <summary>
    /// Clase que realiza el movimiento del personaje
    /// </summary>
    public class CharacterController : MMSingleton<CharacterController>
    {
        [Header("Movement")]
        public float WalkSpeed = 1;
        public float RunSpeed = 1;
        public float DebugSpeed = 12;

        [Header("Permissions")]
        public bool CanMove = true;
        public bool CanRun = true;
        public bool CanDetective = true;

        [Header("Permissions Movement")]
        public bool CanMoveHorizontal = true;
        public bool CanMoveVertical = true;

        [Header("Detective Mode")]
        public bool IsDetectiveModeActive;
        public float DetectiveModeCoolDown = 3;
        public float DetectiveModeRadio = 5;
        public GameObject DetectiveVFX;
        public GameObject DetectivePulsar;
        public LayerMask DefaultLayerMask;
        public LayerMask DetectiveLayerMask;

        private Animator animator;
        private Rigidbody currentRigidbody;
        private SpriteRenderer sprite;

        private float currentSpeed;
        private Vector3 movement;

        private GameObject detectiveCamera;

        public int NumSteps { get; set; }

        void Start()
        {
            //Asocio la velocidad actual a la de andar
            this.currentSpeed = this.WalkSpeed;
            this.animator = this.GetComponent<Animator>();
            this.currentRigidbody = this.GetComponent<Rigidbody>();
            this.sprite = this.GetComponent<SpriteRenderer>();

           // this.detectiveCamera = Camera.main.transform.GetChild(0).gameObject;
        }

        void Update()
        {
            if (!this.CanMove)
            {
                animator.SetFloat("Horizontal", 0);
                animator.SetFloat("Vertical", 0);
                animator.SetFloat("Magnitude", 0);
                return;
            }

            if (Time.timeScale == 0)
                return;

            //Input
            this.movement = new Vector3(
                (Input.GetAxisRaw("Left Stick Horizontal") * this.currentSpeed),
                0.0f,
                Input.GetAxisRaw("Left Stick Vertical") * this.currentSpeed);

            //Permisos
            if (!this.CanMoveHorizontal)
                this.movement = new Vector3(0, 0, Input.GetAxisRaw("Left Stick Vertical") * this.currentSpeed);
            if (!this.CanMoveVertical)
                this.movement = new Vector3(Input.GetAxisRaw("Left Stick Horizontal") * this.currentSpeed, 0, 0);

            //Animaciones
            animator.SetFloat("Horizontal", this.movement.x);
            animator.SetFloat("Vertical", this.movement.z);
            animator.SetFloat("Magnitude", this.movement.magnitude);

            //Flip
            if (this.movement.x > 0)
                this.sprite.flipX = true;
            else if (this.movement.x < 0)
                this.sprite.flipX = false;

            //Walk, Run
            if (Input.GetButton("R1") && this.CanRun)
            {
                animator.SetFloat("Magnitude", 12f);
                this.currentSpeed = this.RunSpeed;
            }
            //else if (Input.GetButton("R2") && this.CanRun)
            //this.currentSpeed = this.DebugSpeed;
            else
                this.currentSpeed = this.WalkSpeed;

            //Modo detective
            // if (Input.GetButtonDown("L1") && this.CanDetective && !this.IsDetectiveModeActive)
            //     this.ActivateDetectiveMode();
        }

        void FixedUpdate()
        {
            if (!this.CanMove)
                return;

            this.currentRigidbody.velocity = new Vector3(
                this.currentSpeed * movement.x * Time.deltaTime,
                this.currentRigidbody.velocity.y,
                this.currentSpeed * movement.z * Time.deltaTime);
        }

        #region Player

        public void StopPlayer(bool stop)
        {
            this.CanMove = !stop;
            animator.SetFloat("Magnitude", 0);
        }

        public void AddStep()
        {
            this.NumSteps++;
        }

        #endregion

        #region Detective Mode

        public void ActivateDetectiveMode()
        {
            StartCoroutine(this.ActivateDetectiveModeCo());
        }

        public IEnumerator ActivateDetectiveModeCo()
        {
            if (PauseCanvas.Instance.MainPanel.activeSelf ||
                MapCanvas.Instance.ContainerPanel.activeSelf ||
                FastTravelCanvas.Instance.FastTravelPanel.activeSelf ||
                ShopCanvas.Instance.ShopPanel.activeSelf)
                yield return null;

            this.ActivateDetectiveModeCore(true);

            //Llamo a la animacion
            this.animator.SetTrigger("Detective");

            yield return new WaitForSeconds(0.5f);

            //VFX
            this.DetectiveVFX.SetActive(true);
            this.DetectivePulsar.SetActive(true);
            StartCoroutine(ProgrammerTools.ScaleFromTo(this.DetectiveVFX, 0, this.DetectiveModeRadio * 0.8f, 1f));           

            yield return new WaitForSeconds(this.DetectiveModeCoolDown);

            //VFX
            yield return StartCoroutine(ProgrammerTools.ScaleFromTo(this.DetectiveVFX, this.DetectiveModeRadio * 0.8f, 0, 0.25f));
            this.DetectiveVFX.SetActive(false);

            this.ActivateDetectiveModeCore(false);
        }

        private void ActivateDetectiveModeCore(bool active)
        {
            this.CanMove = !active;
            this.IsDetectiveModeActive = active;

            //Activamos el container
            GameplayCanvas.Instance.ToogleGameplayPanel();
            DetectiveCanvas.Instance.ToogleDetectiveContainer();

            //Activamos la camara
            this.detectiveCamera.SetActive(active);
        }

        void OnDrawGizmos()
        {
            if (this.IsDetectiveModeActive)
            {
                Gizmos.color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.35f);
                Gizmos.DrawSphere(transform.position, this.DetectiveModeRadio);
            }
        }

        #endregion
    }
}