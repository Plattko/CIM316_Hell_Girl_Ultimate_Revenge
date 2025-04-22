using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenAngelWingAnimation : MonoBehaviour
{
    public GameObject foldedWingLeft;
    public GameObject unfoldedWingLeft;
    public Animator leftWingAnimator;

    public GameObject foldedWingRight;
    public GameObject unfoldedWingRight;
    public Animator rightWingAnimator;

    public string unfoldAnimation = "WingUnfold";
    public string foldAnimation = "WingFold";

    public string playerTag = "Player";

    private Transform playerTransform;
    private string currentArc = "";
    private string previousArc = "";


    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player not found with tag: " + playerTag);
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.y = 0;
        float angleToPlayer = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);
        string newArc = GetArc(angleToPlayer);

        if (newArc != currentArc)
        {
            if (!string.IsNullOrEmpty(currentArc))
            {
                HandleExitQuadrant(currentArc);
            }

            if (!string.IsNullOrEmpty(newArc))
            {
                HandleEnterQuadrant(newArc);
            }

            previousArc = currentArc;
            currentArc = newArc;
        }
    }

    //void Update()
    //{
    //    if (playerTransform == null) return;

    //    Vector3 directionToPlayer = playerTransform.position - transform.position;
    //    directionToPlayer.y = 0;  // Flatten to XZ plane
    //    float angleToPlayer = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);


    //    // Determine the arc based on the angle
    //    string arc = GetArc(angleToPlayer);

    //    // Trigger animation based on the arc the player is in
    //    if (arc == "Right")
    //    {
    //        // Trigger Right animation (Unfold/Fold wings for Right arc)
    //        // Add your code to play the right-side animation here
    //    }
    //    else if (arc == "Front")
    //    {
    //        hasEnteredFront= true;

    //        PlayUnfoldAnimation(leftWingAnimator, foldedWingLeft, unfoldedWingLeft); // Both wings unfold

    //        //// Trigger Front animation (Unfold/Fold wings for Front arc)

    //        //// Unfold both right and left wings when entering the front arc
    //        //if (!hasEnteredFront)
    //        //{
    //        //    hasEnteredRight = true;
    //        //    PlayUnfoldAnimation(rightWingAnimator, foldedWingRight, unfoldedWingRight);
    //        //    PlayFoldAnimation(leftWingAnimator, unfoldedWingLeft, foldedWingLeft); // Ensure left wing is folded
    //        //}
    //        //// Fold both right and left wings when exiting the front arc
    //        //else if (hasEnteredFront)
    //        //{
    //        //    hasEnteredFront = false;
    //        //    // Play fold animation for both right and left wings
    //        //    PlayFoldAnimation(rightWingAnimator, foldedWingRight, unfoldedWingRight); // Right wing
    //        //    PlayFoldAnimation(leftWingAnimator, foldedWingLeft, unfoldedWingLeft); // Left wing
    //        //}
    //    }

    //    else if (arc == "Left")
    //    {
    //        hasEnteredLeft = true;
    //        PlayUnfoldAnimation(leftWingAnimator, foldedWingLeft, unfoldedWingLeft);
    //        PlayFoldAnimation(rightWingAnimator, unfoldedWingRight, foldedWingRight); // Ensure right wing is folded
    //    }
    //    else if (arc == "Back")
    //    {
    //        // Trigger Back animation (Unfold/Fold wings for Back arc)
    //        // Add your code to play the back-side animation here
    //    }
    //}

    string GetArc(float angle)
    {
        // Normalize the angle between 0 and 360
        angle = (angle + 360f) % 360f;

        if (angle >= 315f || angle < 45f) return "Front";       // Forward-facing arc (centered at 0°)
        if (angle >= 45f && angle < 135f) return "Right";       // Right arc (centered at 90°)
        if (angle >= 135f && angle < 225f) return "Back";       // Back arc (centered at 180°)
        if (angle >= 225f && angle < 315f) return "Left";       // Left arc (centered at 270°)

        return "";
    }

    //string GetArc(float angle)
    //{
    //    if (angle >= -45f && angle <= 45f) return "Right";
    //    if (angle > 45f && angle <= 135f) return "Front";
    //    if (angle > 135f || angle <= -135f) return "Left";
    //    if (angle > -45f && angle <= -135f) return "Back";
    //    return "";
    //}

    string GetQuadrant(float angle)
    {
        if (angle >= -45f && angle <= 45f) return "Forward";      // 270°–0°–315°
        if (angle > 45f && angle <= 135f) return "Right";         // 45°–135°
        if (angle > 135f || angle < -135f) return "Back";         // 135°–180° & -135°–(-180°)
        if (angle < -45f && angle >= -135f) return "Left";        // -45°–(-135°)
        return "";
    }

    void HandleEnterQuadrant(string arc)
    {
        switch (arc)
        {
            case "Right":
                PlayUnfoldAnimation(rightWingAnimator, foldedWingRight, unfoldedWingRight);
                PlayFoldAnimation(leftWingAnimator, unfoldedWingLeft, foldedWingLeft);
                break;

            case "Left":
                PlayUnfoldAnimation(leftWingAnimator, foldedWingLeft, unfoldedWingLeft);
                PlayFoldAnimation(rightWingAnimator, unfoldedWingRight, foldedWingRight);
                break;

            case "Front":
            case "Back":
                PlayUnfoldAnimation(leftWingAnimator, foldedWingLeft, unfoldedWingLeft);
                PlayUnfoldAnimation(rightWingAnimator, foldedWingRight, unfoldedWingRight);
                break;
        }
    }

    void HandleExitQuadrant(string arc)
    {
        switch (arc)
        {
            case "Right":
                PlayFoldAnimation(rightWingAnimator, unfoldedWingRight, foldedWingRight);
                break;

            case "Left":
                PlayFoldAnimation(leftWingAnimator, unfoldedWingLeft, foldedWingLeft);
                break;

            case "Front":
            case "Back":
                PlayFoldAnimation(leftWingAnimator, unfoldedWingLeft, foldedWingLeft);
                PlayFoldAnimation(rightWingAnimator, unfoldedWingRight, foldedWingRight);
                break;
        }
    }

    void PlayUnfoldAnimation(Animator animator, GameObject folded, GameObject unfolded)
    {
        if (animator == null || folded == null || unfolded == null) return;

        folded.SetActive(false);
        unfolded.SetActive(true);
        animator.Play(unfoldAnimation, 0, 0f);

        StartCoroutine(SwapAfter(animator, unfolded, folded, true));
    }

    void PlayFoldAnimation(Animator animator, GameObject unfolded, GameObject folded)
    {
        if (animator == null || unfolded == null || folded == null) return;

        animator.Play(foldAnimation, 0, 0f);

        StartCoroutine(SwapAfter(animator, unfolded, folded, false));
    }

    IEnumerator SwapAfter(Animator animator, GameObject current, GameObject toSwap, bool isUnfolding)
    {
        yield return null; // Wait 1 frame

        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        if (animLength <= 0f) animLength = 1f;

        yield return new WaitForSeconds(animLength);

        if (isUnfolding)
        {
            current.SetActive(true);   // keep unfolded on
            toSwap.SetActive(false);   // folded stays off
        }
        else
        {
            current.SetActive(false);  // disable unfolded
            toSwap.SetActive(true);    // re-enable folded
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);  // Orange fill

        Vector3 origin = transform.position;
        Vector3 baseDirection = transform.forward; // This is now considered "forward" direction
        float radius = 3f;
        int segments = 30;

        // Define arc angles
        float[] arcStartAngles = { -45f, 45f, 135f, 225f };  // Starting angles for each arc
        float[] arcEndAngles = { 45f, 135f, 225f, 315f };    // Ending angles for each arc

        for (int i = 0; i < 4; i++)
        {
            Vector3 prevPoint = origin + Quaternion.Euler(0, arcStartAngles[i], 0) * baseDirection * radius;

            for (int j = 1; j <= segments; j++)
            {
                float angle = Mathf.Lerp(arcStartAngles[i], arcEndAngles[i], j / (float)segments) % 360f;
                Vector3 nextPoint = origin + Quaternion.Euler(0, angle, 0) * baseDirection * radius;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
    }
}
    
