using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameTools;
using Events;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private Image hitEffectPanel;
    [SerializeField] private IntPayloadEvent healthChangedEvent;

    private float flashLength = .2f;
    private float fadeTime = .3f;
    private float maxTransparency = .6f;
    private Coroutine hitEffectCoroutine;
    private GameManagerSingleton gm;

    void Awake()
    {
        if(hitEffectPanel != null)
        {
            Color panelColor = hitEffectPanel.color;
            panelColor.a = 0f;
            hitEffectPanel.color = panelColor;
        }
    }
    
    private void Start() => gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
    
    private void OnEnable()
    {
        if (healthChangedEvent != null)
            healthChangedEvent.OnEventTriggered += OnHealthChanged;
    }
    
    private void Disable()
    {
        if (healthChangedEvent != null)
            healthChangedEvent.OnEventTriggered -= OnHealthChanged;
    }

    private void OnHealthChanged(int health)
    {
        if (health < 0)
        {
            PlayHitEffect();
        }
    }

    public void PlayHitEffect()
    {
        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
        }

        hitEffectCoroutine = StartCoroutine(DamageEffect());
    }

    private IEnumerator DamageEffect()
    {
        float time = 0f;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0, maxTransparency, time / flashLength));
            yield return null;
        }
        
        time = 0f;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            SetAlpha(Mathf.Lerp(maxTransparency, 0, time / flashLength));
            yield return null;
        }
        
        SetAlpha(0);
    }

    private void SetAlpha(float alpha)
    {
        if (hitEffectPanel == null) return;
        
        Color colour = hitEffectPanel.color;
        colour.a = alpha;
        hitEffectPanel.color = colour;
    }
    
    
}
