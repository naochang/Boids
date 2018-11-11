using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Boid.
/// </summary>
public class Boid : MonoBehaviour
{
    /// <summary>
    /// 壁を設ける際のサイズ。
    /// </summary>
    public static float WallSize = -1f;
    public static float WallAvoidPercentage = 0.75f;
    public static float WallAvoidForceMultiplier = 5f;

    /// <summary>
    /// 各要素の重み。
    /// </summary>
    public static float SeparationWeight = 5f;
    public static float AlignmentWeight = 0.08f;
    public static float ConhesionWeight = 0.1f;

    /// <summary>
    /// 各要素の考慮する半径。
    /// </summary>
    public static float SeparationRadiusThreshold = float.MaxValue;
    public static float AlignmentRadiusThreshold = float.MaxValue;
    public static float ConhensionRadiusThreshold = float.MaxValue;

    /// <summary>
    /// 分離の際の最小/最大の力。
    /// </summary>
    public static float MinSeparationForce = 10f;
    public static float MaxSeparationForce = 30f;

    /// <summary>
    /// 最大の速度。
    /// </summary>
    public static float MaxVelocity = 100f;

    /// <summary>
    /// 全Boid。
    /// </summary>
    public static IList<Boid> boids = new List<Boid>();

    /// <summary>
    /// 群衆の中心点を取得する。
    /// </summary>
    public static Vector3 GetCenterPosition()
    {
        Vector3 totalPos = Vector3.zero;
        foreach (Boid boid in Boid.boids)
        {
            totalPos += boid.Position;
        }

        return totalPos / Boid.boids.Count;
    }

    /// <summary>
    /// 座標の設定
    /// </summary>
    public Vector3 Position
    {
        get { return this.trans.position; }
        set { this.trans.position = value; }
    }

    /// <summary>
    /// 速度。
    /// </summary>
    private Vector3 velocity;
    public Vector3 Velocity
    {
        get { return this.velocity; }
        set
        {
            this.velocity = value;

            if (this.velocity != Vector3.zero)
                this.trans.forward = this.velocity.normalized;
        }
    }

    /// <summary>
    /// Transform.
    /// </summary>
    private Transform trans;

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        this.trans = this.GetComponent<Transform>();
        Boid.boids.Add(this);
    }

    /// <summary>
    /// Update.
    /// </summary>
    private void Update()
    {
        Vector3 force = Vector3.zero;

        Vector3 totalSeparationForce = Vector3.zero;
        int totalSeparationCount = 0;

        Vector3 alignmentVelocity = Vector3.zero;
        int totalAlignmentCount = 0;

        Vector3 totalConhensionPos = Vector3.zero;
        int totalConhensionCount = 0;

        for (int i = 0; i < Boid.boids.Count; ++i)
        {
            Boid otherBoid = Boid.boids[i];
            if (otherBoid == this) continue;

            Vector3 diff = this.Position - otherBoid.Position;
            float distance = diff.magnitude;

            // 分離 (Separation)
            if (distance <= Boid.SeparationRadiusThreshold)
            {
                Vector3 repulse = diff.normalized * Boid.MaxSeparationForce / distance;
                totalSeparationForce += repulse;
                ++totalSeparationCount;
            }

            // 整列 (Alignment)
            if (distance <= Boid.AlignmentRadiusThreshold)
            {
                alignmentVelocity += otherBoid.velocity;
                ++totalAlignmentCount;
            }

            // 結合 (Conhension)
            if (distance <= Boid.ConhensionRadiusThreshold)
            {
                totalConhensionPos += otherBoid.Position;
                ++totalConhensionCount;
            }

        }

        float dt = Time.deltaTime;

        // 分離 (Separation)
        Vector3 separateForce = Vector3.zero;
        if (totalSeparationCount > 0)
        {
            separateForce = totalSeparationForce / (float)totalSeparationCount;
        }

        // 整列 (Alignment)
        Vector3 alignmentForce = Vector3.zero;
        if (totalAlignmentCount > 0)
        {
            alignmentForce = alignmentVelocity / (float)totalAlignmentCount - this.Velocity;
        }

        // 結合 (Conhension)
        Vector3 conhensionForce = Vector3.zero;
        if (totalConhensionCount > 0)
        {
            conhensionForce = totalConhensionPos / (float) totalConhensionCount - this.Position;
        }

        force += Boid.SeparationWeight * separateForce + Boid.AlignmentWeight * alignmentForce + Boid.ConhesionWeight * conhensionForce;

        // 壁の設定があれば
        if (Boid.WallSize > 0f)
        {
            float wallAvoidDistance = (Boid.WallSize * Boid.WallAvoidPercentage);

            if (Mathf.Abs(this.Position.x) > 0.5f * wallAvoidDistance)
            {
                if (Mathf.Abs(this.Position.x) > 0.5f * Boid.WallSize)
                    this.Position = new Vector3(0.5f * Mathf.Sign(this.Position.x) * Boid.WallSize, this.Position.y, this.Position.z);

                force.x += -Mathf.Sign(this.Position.x) * Boid.WallAvoidForceMultiplier;
            }

            if (Mathf.Abs(this.Position.y) > 0.5f * wallAvoidDistance)
            {
                if (Mathf.Abs(this.Position.y) > 0.5f * Boid.WallSize)
                    this.Position = new Vector3(this.Position.x, 0.5f * Mathf.Sign(this.Position.y) * Boid.WallSize, this.Position.z);

                force.y += -Mathf.Sign(this.Position.y) * Boid.WallAvoidForceMultiplier;
            }

            if (Mathf.Abs(this.Position.z) > 0.5f * wallAvoidDistance)
            {
                if (Mathf.Abs(this.Position.z) > 0.5f * Boid.WallSize)
                    this.Position = new Vector3(this.Position.x, this.Position.y, 0.5f * Mathf.Sign(this.Position.z) * Boid.WallSize);

                force.z += -Mathf.Sign(this.Position.z) * Boid.WallAvoidForceMultiplier;
            }
        }

        this.Velocity += force * dt;
        this.Velocity = this.Velocity.normalized * Mathf.Clamp(this.Velocity.magnitude, Boid.MinSeparationForce, Boid.MaxVelocity);
        this.Position += this.Velocity * dt;
    }
}
