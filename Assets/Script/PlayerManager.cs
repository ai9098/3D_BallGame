using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;
    private float jumpBuffer = 0f;
    public float fadeDuration = 0.5f;  // 何秒でフェードアウトするか

    public GameObject ClearFlag;
    private bool isClearFlagSpawned = false;
    private bool isGameClear = false;
    private int totalCoins;
    private int getCoin;
    private float timer;
    private AudioSource audioSource;

    [SerializeField] private TextMeshProUGUI BigUI;
    [SerializeField] private TextMeshProUGUI NextUI;
    [SerializeField] private TextMeshProUGUI CoinCountUI;
    [SerializeField] private TextMeshProUGUI TimerUI;
    [SerializeField] private CanvasGroup BlackOutUI;

    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip getCoinSound;
    [SerializeField] private AudioClip retrySound;
    [SerializeField] private AudioClip UIStageSound;
    [SerializeField] private AudioClip nextStageSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        getCoin = 0;
        timer = 0f;
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;

        // 初めはフラグ非表示
        ClearFlag.gameObject.SetActive(false);

        // 次のステージを促す文章を非表示に
        NextUI.gameObject.SetActive(false);

        // 初めに表示されるUI
        StartCoroutine(UICoroutine("Game Start!!"));
    }

    void Update()
    {
        // ジャンプ処理(Space)が発生したら記録
        if (Input.GetButtonDown("Jump"))
        {
            // 0.2秒予約することで、滑らかに動く
            jumpBuffer = 0.2f;
        }
        jumpBuffer -= Time.deltaTime;

        // 全てのコインを獲得したらフラグが出現
        if (getCoin == totalCoins && !isClearFlagSpawned)
        {
            ClearFlag.gameObject.SetActive(true);
            isClearFlagSpawned = true;
        }

        // クリアするまでタイムを追加し続ける
        if (!isGameClear)
        {
            timer += Time.deltaTime;
            TimerUI.text = "Timer: " + timer.ToString("F2") + "s";
        }

        // ステージクリア後、Eキーで次のステージへ
        if (isGameClear && Input.GetKeyDown(KeyCode.E))
        {
            LoadNextScene();
        }

        // Escapeでゲーム画面を閉じる
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Finish Game");
        }

        // 獲得したコイン数 / ステージ上のコイン数 表示
        CoinCountUI.text = getCoin.ToString() + "/" + totalCoins.ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 入力の取得
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 移動方向の計算
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        move *= moveSpeed;

        // 力を加える(移動)
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        // 地面に着いたらジャンプ処理を実行
        if (jumpBuffer > 0 && IsGrounded())
        {
            jumpBuffer = 0;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            audioSource.PlayOneShot(jumpSound);
        }
    }

    // 地面にいるかどうかを判定
    bool IsGrounded()
    { 
        // プレイヤーの足元から1.1m以内に何かあるかを判定し、返す
        return Physics.Raycast(transform.position, Vector3.down, 0.5f); 
    }

    // プレイヤーが何かに触れたら
    private void OnTriggerEnter(Collider other)
    {
        // フラグに触れたら
        if (other.gameObject == ClearFlag)
        {
            // 触れたフラグを消す
            ClearFlag.gameObject.SetActive(false);
            // クリア時に表示されるUI
            StartCoroutine(UICoroutine("Stage Clear!!"));
            NextUI.gameObject.SetActive(true);
            isGameClear = true;
        }

        // コインに触れたら
        if (other.gameObject.CompareTag("Coin"))
        {
            // 触れたコインを消す
            other.gameObject.SetActive(false);
            getCoin++;
            audioSource.PlayOneShot(getCoinSound);
        }

        // ステージから落下し、RetryBoardに触れたら
        if (other.gameObject.CompareTag("RetryBoard"))
        {
            // 画面をフェードアウトさせるコールーチンを呼び出す
            StartCoroutine(RetryCoroutine());
        }
    }

    private IEnumerator UICoroutine(string text)
    {
        BigUI.text = text;
        BigUI.gameObject.SetActive(true);
        audioSource.PlayOneShot(UIStageSound);

        // 徐々に大きくする
        float scale = 1f;
        while (scale < 1.3f)
        {
            scale += Time.deltaTime * 2f;
            BigUI.rectTransform.localScale = Vector3.one * scale;
            yield return null;
        }
        // 少し小さくする
        while (scale > 1f)
        {
            scale -= Time.deltaTime * 2f;
            BigUI.rectTransform.localScale = Vector3.one * scale;
            yield return null;
        }

        // UI表示後、3秒待つ
        yield return new WaitForSeconds(1);

        BigUI.gameObject.SetActive(false);
    }

    // フェードアウトを待つコールーチン
    private IEnumerator RetryCoroutine()
    {
        //BlackOut()を呼び出してから
        yield return StartCoroutine(BlackOut());

        // ステージ再読み込み
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    // 画面をフェードアウトさせるコールーチン
    private IEnumerator BlackOut()
    {
        BlackOutUI.gameObject.SetActive(true);
        audioSource.PlayOneShot(retrySound);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            BlackOutUI.alpha = t;
            yield return null;
        }

        BlackOutUI.alpha = 1f;
    }

    public void LoadNextScene()
    {
        // 現在のシーンと、シーンの合計数を取得
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        // まだシーンがあれば
        if (currentSceneIndex + 1 < totalScenes)
        {
            // 次のシーンへ移動
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            // なければログで通知
            Debug.Log("No more Stages!");
        }
    }
}
