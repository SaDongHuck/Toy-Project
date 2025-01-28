using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class LaunchController : MonoBehaviour
{
    public Transform LauchStick;
    public GameObject projectileprefab;
    public Transform firePoint;
    private Vector3 startLaunchStick;
    private Vector3 animLaunchStickPos;
    private ActionBasedController ABC;

    public Text scoreText; // Canvas의 Score Text
    private int score = 0; // 현재 점수

    private void Awake()
    {
        startLaunchStick = LauchStick.localPosition;
        animLaunchStickPos = LauchStick.localPosition;
        animLaunchStickPos.x = -0.1f;
        ABC = GetComponentInParent<ActionBasedController>();
        ABC.activateAction.reference.action.performed += LaunchStickAction;
    }

    private void OnEnable()
    {
        ABC.selectAction.reference.action.performed += LaunchStickAction;
        ABC.selectAction.reference.action.canceled += LaunchStickAction;
    }

    private void OnDisable()
    {
        ABC.selectAction.reference.action.performed -= LaunchStickAction;
        ABC.selectAction.reference.action.canceled -= LaunchStickAction;
    }

    private void LaunchStickAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LauchStick.localPosition = animLaunchStickPos;
            Fire();
        }
        else
        {
            LauchStick.localPosition = startLaunchStick;
        }
    }

    private void Fire()
    {
        if (projectileprefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectileprefab, firePoint.position, firePoint.rotation);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(firePoint.forward * 20f, ForceMode.Impulse);
            }

            Destroy(projectile, 5f);
        }
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
