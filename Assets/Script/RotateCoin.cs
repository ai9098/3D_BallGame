using UnityEngine;

public class RotateCoin : MonoBehaviour
{
    public float rotationSpeed;

    // Update is called once per frame
    void FixedUpdate()
    {
        // ƒRƒCƒ“‚ð‰ñ“]‚³‚¹‚é
        transform.Rotate(new Vector3(0f, 0f, 1f) * rotationSpeed);
    }
}
