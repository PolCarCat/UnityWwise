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
    public AudioClip sword1;
    public AudioClip sword2;
    public AudioClip sword3;

    public AudioClip dagger1;
    public AudioClip dagger2;
    public AudioClip dagger3;

    public AudioClip hammer1;
    public AudioClip hammer2;
    public AudioClip hammer3;

    public AudioClip pickaxe1;
    public AudioClip pickaxe2;
    public AudioClip pickaxe3;

    public AudioClip axe1;
    public AudioClip axe2;
    public AudioClip axe3;

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

        int state = 0;

        AnimatorStateInfo currentAnimation = PlayerManager.Instance.playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (currentAnimation.IsName("Player_RightSwing")) state = 1;
        else if (currentAnimation.IsName("Player_LeftSwing")) state = 2;
        else if (currentAnimation.IsName("Player_TopSwing")) state = 3;

        switch (W.weaponType) 
        {
            case WeaponTypes.Axe:
                switch (state)
                {
                    case 1:
                        audioSource.PlayOneShot(axe1);
                        break;
                    case 2:
                        audioSource.PlayOneShot(axe2);
                        break;
                    case 3:
                        audioSource.PlayOneShot(axe3);
                        break;
                }
            break;
            case WeaponTypes.Dagger:
                switch (state)
                {
                    case 1:
                        audioSource.PlayOneShot(dagger1);
                        break;
                    case 2:
                        audioSource.PlayOneShot(dagger2);
                        break;
                    case 3:
                        audioSource.PlayOneShot(dagger3);
                        break;
                }
                break;
            case WeaponTypes.Sword:
                switch (state)
                {
                    case 1:
                        audioSource.PlayOneShot(sword1);
                        break;
                    case 2:
                        audioSource.PlayOneShot(sword2);
                        break;
                    case 3:
                        audioSource.PlayOneShot(sword3);
                        break;
                }
                break;
            case WeaponTypes.PickAxe:
                switch (state)
                {
                    case 1:
                        audioSource.PlayOneShot(pickaxe1);
                        break;
                    case 2:
                        audioSource.PlayOneShot(pickaxe2);
                        break;
                    case 3:
                        audioSource.PlayOneShot(pickaxe3);
                        break;
                }
                break;
            case WeaponTypes.Hammer:
                switch (state)
                {
                    case 1:
                        audioSource.PlayOneShot(hammer1);
                        break;
                    case 2:
                        audioSource.PlayOneShot(hammer2);
                        break;
                    case 3:
                        audioSource.PlayOneShot(hammer3);
                        break;
                }
                break;
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
    }

    public void WeaponSound()
    {
        Weapon EquippedWeapon = PlayerManager.Instance.equippedWeaponInfo;
        // HINT: This is a good place to play equipped weapon impact sound
    }
}
