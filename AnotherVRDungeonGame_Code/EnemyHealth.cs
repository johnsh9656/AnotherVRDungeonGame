using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem.Processors;
using UnityEngine.SocialPlatforms.Impl;

public class EnemyHealth : MonoBehaviour
{
    private class BoneTransform
    {
        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
    
    [SerializeField] float health = 100f;
    [SerializeField] bool invulnerable = false;
    [SerializeField] float invulnLength = .4f;
    [SerializeField] float invulnTimer = 0f;
    Animator anim;

    private Rigidbody[] _ragdollRigidbodies;
    private BoneTransform[] faceUpStandUpBoneTransforms;
    private BoneTransform[] faceDownStandUpBoneTransforms;
    private BoneTransform[] ragdollBoneTransforms;
    private Transform[] bones;
    private Transform hipsBone;
    //private bool ragdoll;
    private bool isFacingUp;

    [SerializeField] public float ragdollThreshold = 40;
    [SerializeField] float timeToGetUp;
    [SerializeField] float minTimeToGetUp = 2f;
    [SerializeField] float maxTimeToGetUp = 5;
    [SerializeField] private string faceUpStandUpStateName;
    [SerializeField] private string faceDownStandUpStateName;
    [SerializeField] private string faceUpStandUpClipName;
    [SerializeField] private string faceDownStandUpClipName;

    [SerializeField] private float timeToResetBones = 0.5f;
    private float elapsedResetBonesTime;
    private bool resettingBones = false;

    public bool isAlive = true;

    EnemyAI ai;

    public GameObject bloodFX;
    public List<GameObject> activeBloodFX = new List<GameObject>();
    RoomManager roomManager;

    // arena mode
    [SerializeField] int arenaScore;
    [SerializeField] bool arena;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        ai = GetComponent<EnemyAI>();
        if (!arena) roomManager = FindObjectOfType<RoomManager>();

        if (!anim) { isAlive = false; return; }

        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        hipsBone = anim.GetBoneTransform(HumanBodyBones.Hips);

        bones = hipsBone.GetComponentsInChildren<Transform>();
        faceUpStandUpBoneTransforms = new BoneTransform[bones.Length];
        faceDownStandUpBoneTransforms = new BoneTransform[bones.Length];
        ragdollBoneTransforms = new BoneTransform[bones.Length];
        for (int i = 0; i<bones.Length; i++)
        {
            faceUpStandUpBoneTransforms[i] = new BoneTransform();
            faceDownStandUpBoneTransforms[i] = new BoneTransform();
            ragdollBoneTransforms[i] = new BoneTransform();
        }

        PopulateAnimationStartBoneTransforms(faceUpStandUpClipName, faceUpStandUpBoneTransforms);
        PopulateAnimationStartBoneTransforms(faceDownStandUpClipName, faceDownStandUpBoneTransforms);
        resettingBones = false;
        DisableRagdoll();

        arena = FindObjectOfType<ArenaManager>();
    }

    private void Update()
    {
        if (!isAlive) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(45, true);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(20, true);
        }

        //if (!isAlive) return;

        if (resettingBones)
        {
            ResettingBonesBehaviour();
            return;
        }

        if (invulnTimer > 0)
        {
            invulnTimer -= Time.deltaTime;
            if (invulnTimer <= 0)
            {
                invulnerable = false;
                invulnTimer = 0;
            }
        }

        if (timeToGetUp > 0)
        {
            timeToGetUp -= Time.deltaTime;
            if (timeToGetUp <= 0)
            {
                isFacingUp = hipsBone.forward.y > 0;

                AlignRotationToHips();
                AlignPositionToHips();
                PopulateBoneTransforms(ragdollBoneTransforms);
                resettingBones = true;
                elapsedResetBonesTime = 0;
            }
        }
    }

    private void ResettingBonesBehaviour()
    {
        elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = elapsedResetBonesTime / timeToResetBones;

        BoneTransform[] standUpBoneTransforms = GetStandUpBoneTransforms();

        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].localPosition = Vector3.Lerp
                (ragdollBoneTransforms[i].Position, standUpBoneTransforms[i].Position, elapsedPercentage);
            bones[i].localRotation = Quaternion.Lerp
                (ragdollBoneTransforms[i].Rotation, standUpBoneTransforms[i].Rotation, elapsedPercentage);
        }

        if (elapsedPercentage >= 1)
        {
            resettingBones = false;
            //anim.SetTrigger("GetUp");
            anim.Play(GetStandUpStateName(), 0, 0);
            DisableRagdoll();
        }
    }

    public void TakeDamage(float damage, bool isBlunt)
    {
        if (invulnerable || !isAlive) return;

        health -= damage;
        if (health <= 0)
        {
            Die();
            EnableRagdoll();
            Destroy(gameObject, Random.Range(5, 10f));
        }

        // react to hit
        if (isBlunt && damage > ragdollThreshold)
        {
            EnableRagdoll();
            timeToGetUp = Random.Range(minTimeToGetUp, maxTimeToGetUp);
        } else if (damage > 10)
        {
            anim.SetTrigger("Hit");
        }

        roomManager.currentRoom.AlertAllEnemies();
        invulnerable = true;
        invulnTimer = invulnLength;
        ai.SetHealthOverride(true);
        
    }

    public void Die()
    {
        isAlive = false;
        foreach (GameObject fx in activeBloodFX)
        {
            Destroy(fx, 4f);
        }
        Destroy(anim);
        Destroy(GetComponentInChildren<MeleeDamage>());
        Destroy(ai);
        Destroy(GetComponent<NavMeshAgent>());

        if (arena) { FindObjectOfType<ArenaManager>().OnEnemyKilled(this, arenaScore); }
        else { roomManager.OnEnemyKilled(this); }
    }

    public void Slice()
    {
        isAlive = false;
        Destroy(this);
    }

    private void DisableRagdoll()
    {
        //ragdoll = false;
        anim.enabled = true;
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
        }
    }

    private void EnableRagdoll()
    {
        //ragdoll = true;
        ai.SetHealthOverride(true);
        anim.enabled = false;
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }
    }

    public void StrikeBloodFX(Vector3 contactPoint)
    {
        GameObject blood = Instantiate(bloodFX, contactPoint, Quaternion.identity);
        Destroy(blood, 3f);
    }

    private void AlignPositionToHips()
    {
        Vector3 originalHipsPos = hipsBone.position;
        transform.position = hipsBone.position;

        /*Vector3 positionOffset = GetStandUpBoneTransforms()[0].Position;
        positionOffset.y = 0;
        positionOffset = transform.rotation * positionOffset;
        transform.position -= positionOffset;
        */
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }

        hipsBone.position = originalHipsPos;
    }

    private void AlignRotationToHips()
    {
        Vector3 originalHipsPosition = hipsBone.position;
        Quaternion originalHipsRotation = hipsBone.rotation;

        Vector3 desiredDirection = hipsBone.up;
        if (isFacingUp) hipsBone.up *= -1;
        desiredDirection.y = 0;
        desiredDirection.Normalize();

        Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
        transform.rotation *= fromToRotation;

        hipsBone.position = originalHipsPosition;
        hipsBone.rotation = originalHipsRotation;
    }

    private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
    {
        for (int i = 0; i < bones.Length; i++)
        {
            boneTransforms[i].Position = bones[i].localPosition;
            boneTransforms[i].Rotation = bones[i].localRotation;
        }
    }

    private void PopulateAnimationStartBoneTransforms(string clipName, BoneTransform[] boneTransforms)
    {
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;

        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                clip.SampleAnimation(gameObject, 0);
                PopulateBoneTransforms(boneTransforms);
                break;
            }
        }

        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }

    private string GetStandUpStateName()
    {
        return isFacingUp ? faceUpStandUpStateName : faceDownStandUpStateName;
    }

    private BoneTransform[] GetStandUpBoneTransforms()
    {
        return isFacingUp ? faceUpStandUpBoneTransforms : faceDownStandUpBoneTransforms;
    }
}
