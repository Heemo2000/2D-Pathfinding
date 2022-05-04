using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField]private MovingUnit unit;
    [SerializeField]private Transform target;

    [SerializeField]private float targetMoveSpeed = 5f;
    
    [SerializeField]private float scanInterval = 1f;
    
    void Awake() {
        
    }

    // Start is called before the first frame update
    void Start()
    {
                
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            unit.MoveToDestination(target.position);
        }
        MoveTarget();
    }

    
    private void MoveTarget()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 moveInput = new Vector2(horizontalInput,verticalInput).normalized;
        target.Translate(moveInput * targetMoveSpeed * Time.fixedDeltaTime);
        
    }
    
}
