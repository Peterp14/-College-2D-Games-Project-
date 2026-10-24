
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;
public class PlayerScript : MonoBehaviour
{
    private Vector2 moveDirection;
    private Rigidbody2D _rbody;
    public float speed;
    private SpriteRenderer _renderer;
    private Animator _animator;

    private InteractionScript interactable;
    private InteractionScript nearbyInteractable;
    public DialogueRunner dialogueRunner;
    public GameObject interactionIndicator;

    void Start()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        MeshRenderer textRenderer = interactionIndicator.GetComponent<MeshRenderer>();

        textRenderer.sortingLayerID = _renderer.sortingLayerID;
        textRenderer.sortingOrder = _renderer.sortingOrder + 100;
    }

    void Update()
    {
        bool isTalking = dialogueRunner != null && dialogueRunner.IsDialogueRunning;

        _animator.SetBool("isMoving", !isTalking && moveDirection.sqrMagnitude > 0.01f);

        interactionIndicator.SetActive(!isTalking && nearbyInteractable != null);
    }

    private void FixedUpdate()
    {
        if (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
        {
            _rbody.linearVelocity = Vector2.zero;
            return;
        }

        if (moveDirection.x != 0)
            _renderer.flipX = moveDirection.x < 0;

        _rbody.linearVelocity = moveDirection * speed;
    }

    public void OnMove(InputValue value)
    {
        if (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
        {
            moveDirection = Vector2.zero;
            return;
        }

        moveDirection = value.Get<Vector2>();
    }

    public void OnAction(InputValue value)
    {
        if (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
            return;

        if (value.isPressed && nearbyInteractable != null)
        {
            nearbyInteractable.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactable = collision.GetComponent<InteractionScript>();
        if (interactable != null)
        {
            interactionIndicator.SetActive(true);
            nearbyInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactable = collision.GetComponent<InteractionScript>();
        if (interactable != null && interactable == nearbyInteractable)
        {
            interactionIndicator.SetActive(false);
            nearbyInteractable = null;
        }
    }
}
