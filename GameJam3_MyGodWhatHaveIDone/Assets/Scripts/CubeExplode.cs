﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeExplode : MonoBehaviour
{
    public Material tinyCubeMat;
    public int EnviromentLayer = 9;
    public int cubesInRow = 5;

    float cubesPivotDistance;
    Vector3 cubesPivot;

    public float explosionForce = 50f;
    public float explosionRadius = 4f;
    public float explosionUpward = 0.4f;

    private float cubeSize = 0.2f;
    private bool active = false;

    public void ShootCube() {
        cubeSize = transform.localScale.x / cubesInRow;
        //calculate pivot distance
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        //use this value to create pivot vector)
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);

        active = true;
    }

    void OnCollisionEnter(Collision collision)
    {   
        if(active){
            if (collision.gameObject.layer == EnviromentLayer) {
                explode();
            }
        }
    }

    public void explode() {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        //loop 3 times to create 5x5x5 pieces in x,y,z coordinates
        for (int x = 0; x < cubesInRow; x++) {
            for (int y = 0; y < cubesInRow; y++) {
                for (int z = 0; z < cubesInRow; z++) {
                    createPiece(x, y, z);
                }
            }
        }

        //get explosion position
        Vector3 explosionPos = transform.position;
        //get colliders in that position and radius
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        //add explosion force to all colliders in that overlap sphere
        foreach (Collider hit in colliders) {
            //get rigidbody from collider object
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null) {
                //add explosion force to this body with given parameters
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }

        Destroy(this.gameObject, 5);

    }

    void createPiece(int x, int y, int z) {
        //create piece
        GameObject piece;
        piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        piece.GetComponent<Renderer>().material = tinyCubeMat;
        piece.layer = 4;

        //set piece position and scale
        piece.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
        piece.transform.parent = this.transform;

        //add rigidbody and set mass
        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = cubeSize;

        Destroy(piece, Random.Range(1.0f,4.0f));
    }

}
