////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
////////////////////////////////////////////////////////////////////////

﻿using UnityEngine;
using System.Collections;

public class AdventuressAnimationEventHandler : MonoBehaviour
{
    public AudioClip[] normalFoodStep;
    public AudioClip[] runFoodStep;


    [Header("Object Links")]
    [SerializeField]
    private Animator playerAnimator;

    [SerializeField]
    private GameObject runParticles;

    [Header("Combat Audios")]

    public uint weaponState = 0;

    public AudioClip miss1;
    public AudioClip miss2;
    public AudioClip miss3;

    public AudioClip sword;
    public AudioClip dagger;
    public AudioClip axe;

    private PlayerFoot foot_L;
    private PlayerFoot foot_R;

    AudioSource audioSource;
    PlayerMovement movement;

    #region private variables
    private bool hasPausedMovement;
    private readonly int canShootMagicHash = Animator.StringToHash("CanShootMagic");
    private readonly int isAttackingHash = Animator.StringToHash("IsAttacking");
    #endregion

    private void Awake()
    {
        GameObject L = GameObject.Find("toe_left");
        GameObject R = GameObject.Find("toe_right");
        if (L != null)
        {
            foot_L = L.GetComponent<PlayerFoot>();
        }
        else {
            print("Left foot missing");
        }
        if (R != null)
        {
            foot_R = R.GetComponent<PlayerFoot>();
        }
        else
        {
            print("Right foot missing");
        }

        audioSource = GetComponent<AudioSource>();
        movement = GetComponent<PlayerMovement>();

    }


    void enableWeaponCollider()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.equippedWeaponInfo != null)
        {
            PlayerManager.Instance.equippedWeaponInfo.EnableHitbox();
        }
    }

    void disableWeaponCollider()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.equippedWeaponInfo != null)
        {
            PlayerManager.Instance.equippedWeaponInfo.DisableHitbox();
        }

    }

    void ScreenShake()
    {
        PlayerManager.Instance.cameraScript.CamShake(new PlayerCamera.CameraShake(0.4f, 0.7f));
    }

    bool onCooldown = false;
    public enum FootSide { left, right };
    public void TakeFootstep(FootSide side)
    {
        if (foot_L != null && foot_R != null) {
            if (!PlayerManager.Instance.inAir && !onCooldown)
            {
                Vector3 particlePosition;

                if (side == FootSide.left )
                {
                    //if (foot_L.FootstepSound.Validate())
                    { 
                        // HINT: Play left footstep sound
                        particlePosition = foot_L.transform.position;
                        FootstepParticles(particlePosition);

                        if (movement.currentSpeed > 5)
                        {
                            audioSource.PlayOneShot(normalFoodStep[Random.Range(0, runFoodStep.Length)], 7 * movement.currentSpeed / movement.maxSpeed);
                        }
                        else
                        {
                            audioSource.PlayOneShot(normalFoodStep[Random.Range(0, normalFoodStep.Length)], 7 * movement.currentSpeed / movement.maxSpeed);
                        }
                    }
                }
                else
                {
                    //if (foot_R.FootstepSound.Validate())
                    {
                        // HINT: Play right footstep sound
                        particlePosition = foot_R.transform.position;
                        FootstepParticles(particlePosition);

                        if (movement.currentSpeed > 5)
                        {
                            audioSource.PlayOneShot(normalFoodStep[Random.Range(0, runFoodStep.Length)], 7 * movement.currentSpeed/movement.maxSpeed);
                        }
                        else
                        {
                            audioSource.PlayOneShot(normalFoodStep[Random.Range(0, runFoodStep.Length)], 7 * movement.currentSpeed / movement.maxSpeed);
                        }
                       
                    }
                }
            }
        }
    }

    void FootstepParticles(Vector3 particlePosition) {
        GameObject p = Instantiate(runParticles, particlePosition + Vector3.up * 0.1f, Quaternion.identity) as GameObject;
        p.transform.parent = SceneStructure.Instance.TemporaryInstantiations.transform;
        Destroy(p, 5f);
        StartCoroutine(FootstepCooldown());
    }

    IEnumerator FootstepCooldown()
    {
        onCooldown = true;
        yield return new WaitForSecondsRealtime(0.2f);
        onCooldown = false;
    }

    void ReadyToShootMagic()
    {
        PlayerManager.Instance.playerAnimator.SetBool(canShootMagicHash, true);
    }

    public enum AttackState { NotAttacking, Attacking };
    void SetIsAttacking(AttackState state)
    {
        if (state == AttackState.NotAttacking)
        {
            playerAnimator.SetBool(isAttackingHash, false);
        }
        else
        {
            playerAnimator.SetBool(isAttackingHash, true);
        }
    }

    public void Weapon_SwingEvent()
    {
        // PLAY SOUND
        Weapon W = PlayerManager.Instance.equippedWeaponInfo;
        // HINT: PlayerManager.Instance.weaponSlot contains the selected weapon;
        // HINT: This is a good place to play the weapon swing sounds


        AnimatorStateInfo currentAnimation = PlayerManager.Instance.playerAnimator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimation.IsName("Player_RightSwing"))
        {
            audioSource.PlayOneShot(miss1);
        }
        else if (currentAnimation.IsName("Player_LeftSwing"))
        {
            audioSource.PlayOneShot(miss2);
        }
        else if (currentAnimation.IsName("Player_TopSwing"))
        {
             audioSource.PlayOneShot(miss3);
        }
          
        
    }

    public void PauseMovement()
    {
        if (!hasPausedMovement)
        {
            hasPausedMovement = true;
            PlayerManager.Instance.motor.SlowMovement();
        }
    }

    public void ResumeMovement()
    {
        if (hasPausedMovement)
        {
            hasPausedMovement = false;
            PlayerManager.Instance.motor.UnslowMovement();
        }
    }

    public void FreezeMotor()
    {
        StartCoroutine(PickupEvent());
    }

    private IEnumerator PickupEvent()
    {
        PlayerManager.Instance.PauseMovement(gameObject);
        yield return new WaitForSeconds(2f);
        PlayerManager.Instance.ResumeMovement(gameObject);
    }

    public void PickUpItem()
    {
        PlayerManager.Instance.PickUpEvent();
        // HINT: This is a good place to play the Get item sound and stinger

        Weapon W = PlayerManager.Instance.equippedWeaponInfo;
        switch (W.weaponType)
        {
            case WeaponTypes.Axe:
                audioSource.PlayOneShot(axe);
                break;
            case WeaponTypes.Dagger:
                audioSource.PlayOneShot(dagger);
                break;
            case WeaponTypes.Sword:
                audioSource.PlayOneShot(sword);
                break;
        }

    }

    public void WeaponSound()
    {
        Weapon EquippedWeapon = PlayerManager.Instance.equippedWeaponInfo;
        // HINT: This is a good place to play equipped weapon impact sound
        Debug.Log("Weapon");
        switch (EquippedWeapon.weaponType)
        {
            case WeaponTypes.Axe:
                audioSource.PlayOneShot(axe);
                break;
            case WeaponTypes.Dagger:
                audioSource.PlayOneShot(dagger);
                break;
            case WeaponTypes.Sword:
                audioSource.PlayOneShot(sword);
                break;
        }
    }
}
