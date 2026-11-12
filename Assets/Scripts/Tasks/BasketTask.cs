using System;
using UnityEngine;
using static UnityEditor.Progress;
using TMPro;
using System.Collections;

public class BasketTask : MonoBehaviour
{

    public event Action<Item> OnFishCollected;
    [SerializeField] private ParticleSystem basketParticle;
    [SerializeField] private TMP_Text textPrefab;
    [SerializeField] private Canvas canvas;

    [Header("Pop Text Settings")]
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private float popScale = 3f;
    [SerializeField] private float popDuration = 0.15f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 1f, 0);
    [SerializeField] private float launchSpeedY = 900f;    
    [SerializeField] private float launchSpeedX = 300f;     
    [SerializeField] private float gravity = 2250f; 


    private int weight;



    void Awake()
    {


    }


    void Update()
    {
       

    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Item item)) return;

        if(item.GetItemType() == typeof(FishData))
        {
            Debug.Log("score");
            OnFishCollected?.Invoke(item);
            Destroy(other.gameObject);
            basketParticle.Play();
           // weight = (int)other.GetComponent<Item>().GetWeight();
            //StartCoroutine(ShowText());
            SpawnScoreText((int)item.GetWeight());
        }
        else
        {
            Debug.Log("No");
        }
    }


    private void SpawnScoreText(int weight)
    {
        TMP_Text text = Instantiate(textPrefab, canvas.transform);
        text.text = weight.ToString();
        text.color = weight > 5f ? Color.green : Color.red;

        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);

        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.NoWrap; 
        rt.sizeDelta = new Vector2(100f, 50f);
        
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + worldOffset);
        Camera uiCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : (canvas.worldCamera != null ? canvas.worldCamera : Camera.main);
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, screenPosition, uiCam, out Vector2 localPoint);
        rt.anchoredPosition = localPoint;
        StartCoroutine(AnimateText(text));
    }



    private IEnumerator AnimateText(TMP_Text text)
    {
       
        RectTransform t = text.rectTransform;
        Color baseColor = text.color;

        float dir = UnityEngine.Random.value < 0.5f ? -1f : 1f;
        Vector2 velocity = new Vector2(dir * UnityEngine.Random.Range(launchSpeedX * 0.5f, launchSpeedX), launchSpeedY);
        
;
        float time = 0f;

        while(time < lifetime)
        {
            float dt = Time.deltaTime;
            time += dt;

            velocity.y -= gravity * dt;
            t.anchoredPosition += velocity * dt;

            // float y = launchSpeedY * time - 0.5f * gravity * time * time;
            // if(time > peakTime)
            // {
            //     float ease = Mathf.Clamp01((time - peakTime) / 0.15f);
            //     xOffset += dir * launchSpeedX * ease * dt;
            // }
           // float easeOut = 1f - Mathf.Pow(1f - p, 3f);
            //t.position = Vector3.Lerp(startPos, endPos, easeOut);
            // velocity.y -= gravity * dt;
            // t.position += (Vector3)(velocity * dt);

           // t.anchoredPosition = startPos + new Vector2(xOffset, y);

            float scale;
            if(time < popDuration)
            {
                scale = Mathf.Lerp(0f, popScale, time / popDuration);
            }
            else if(time < popDuration * 2f)
            {
                scale = Mathf.Lerp(popScale, 1f, (time - popDuration) / popDuration);
            }
            else
            {
                scale = 1f;
            }
            t.localScale = Vector3.one * scale;

            float fadeStart = lifetime - fadeDuration;
            if(time > fadeStart)
            {
                baseColor.a = 1f - (time - fadeStart) / fadeDuration;
                text.color = baseColor;
            }

            yield return null;
        }

        Destroy(text.gameObject);

        // basketText.text = weight.ToString();
        // TMP_Text text = Instantiate(basketText, canvas.transform);

        // if(weight > 5f)
        // {
        //     text.color = Color.green;
        // }
        // else
        // {
        //     text.color = Color.red;
        // }
        
        // text.gameObject.SetActive(true);
        // yield return new WaitForSeconds(2f);
        // Destroy(text.gameObject);
       


    }
   
}
