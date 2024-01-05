using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth;
    [SerializeField] private Slider healthBar;
    [SerializeField] public Image damageUI;
    [SerializeField] private Color damageUIColour;
    [SerializeField] private Color healColour;
    [SerializeField] private float damageColourLength = 1f;
    [SerializeField] public GameObject deathCanvas;
    [SerializeField] private GameObject l_hand;
    [SerializeField] private GameObject r_hand;
    [SerializeField] private TMP_Text scoreText;
    
    private bool isAlive = true;

    private void Awake()
    {
        SetMaxHealth(maxHealth);
        healthBar.gameObject.SetActive(true);
        deathCanvas.SetActive(false);
    }

    private void Update()
    {
        if (!isAlive) return;

        if (damageUI.color.a > 0)
        {
            damageUI.color -= new Color(0, 0, 0, damageColourLength * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(5);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }

        SetHealth(currentHealth);
        damageUI.color = damageUIColour;
    }

    void Die()
    {
        isAlive = false;
        deathCanvas.SetActive(true);
        if (FindObjectOfType<ArenaManager>())
        {
            scoreText.enabled = true;
            scoreText.text = "Score: " + FindObjectOfType<ArenaManager>().GetScore();
        }
        Destroy(GetComponent<LocomotionSystem>());
        Destroy(GetComponent<ActionBasedSnapTurnProvider>());
        Destroy(GetComponent<CharacterControllerGravity>());
        Destroy(GetComponent<HandleHookshot>());
    }

    public void SetHealth(float health)
    {
        healthBar.value = health;
    }

    public void SetMaxHealth(float max)
    {
        maxHealth = max;
        currentHealth = max;
        healthBar.maxValue = max;
        healthBar.value = max;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            SetHealth(currentHealth);
            damageUI.color = healColour;
            SetHealth(currentHealth);
        }
    }
}
