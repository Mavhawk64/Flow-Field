using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public Vector2 pos;
    public Vector2 vel;
    public Vector2 acc;

    public Particle(Vector2 pos)
    {
        this.pos = pos;
        this.vel = Vector2.zero;
        this.acc = Vector2.zero;
    }

    public void SetRandPos(Vector2 rangeW, Vector2 rangeH)
    {
        this.pos = new Vector2(Random.Range(rangeW.x, rangeW.y), Random.Range(rangeH.x, rangeH.y));
    }

    // Update is called once per frame
    public void ApplyForce(Vector2 force)
    {
        this.acc += force;
    }

    public void Move(float screenWidth, float screenHeight)
    {
        this.vel += this.acc;
        this.pos += this.vel;
        this.acc *= 0;

        this.CheckOutOfBounds(screenWidth, screenHeight);
    }

    public void CheckOutOfBounds(float screenWidth, float screenHeight)
    {
        float maxX = screenWidth / 2;
        float maxY = screenHeight / 2;

        if (this.pos.x > maxX) this.pos.x = -maxX;
        if (this.pos.x < -maxX) this.pos.x = maxX;
        if (this.pos.y > maxY) this.pos.y = -maxY;
        if (this.pos.y < -maxY) this.pos.y = maxY;
    }

}