using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TitleController : MonoBehaviour
{
    [SerializeField] public string nextSceneName;

    private float flashTimer = 0;
    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        this.flashTimer -= Time.deltaTime;
        if (this.flashTimer <= 0) this.flashTimer += 2;
        var active = this.flashTimer < 1;
        text.enabled = active;

        if (Input.anyKey) SceneManager.LoadScene(this.nextSceneName);
    }
}
