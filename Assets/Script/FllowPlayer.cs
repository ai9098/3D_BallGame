using UnityEngine;

public class FllowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // カメラとプレイヤーの距離を記録
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 自分自身(カメラ)の位置を調整
        transform.position = player.position + offset;
    }
}
