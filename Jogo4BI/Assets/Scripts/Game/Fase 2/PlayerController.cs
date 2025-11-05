using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;      // ponto de disparo (pos e rotação)
    public Transform weapon;         // sprite/transform da arma (opcional)
    public float fireRate = 0.2f;

    private float fireTimer;
    private Vector2 lastDir = Vector2.right; // direção atual/última direção usada para atirar

    void Update()
    {
        Move();
        Aim();
        Shoot();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, v, 0f);

        if (input != Vector3.zero)
        {
            // Move
            transform.position += input.normalized * speed * Time.deltaTime;

            // Escolhe direção dominante: horizontal ganha quando |h| >= |v|
            if (Mathf.Abs(h) >= Mathf.Abs(v))
            {
                lastDir = (h > 0) ? Vector2.right : Vector2.left;
            }
            else
            {
                lastDir = (v > 0) ? Vector2.up : Vector2.down;
            }
        }
    }

    void Aim()
    {
        float zAngle = 0f;

        // Mapeamento correto de ângulos para que transform.up do projétil aponte para a direção desejada
        if (lastDir == Vector2.right)
        {
            zAngle = -90f; // up -> right
            if (weapon != null) weapon.localScale = new Vector3(1, 1, 1);
        }
        else if (lastDir == Vector2.left)
        {
            zAngle = 90f;  // up -> left
            if (weapon != null) weapon.localScale = new Vector3(-1, 1, 1); // espelha horizontalmente
        }
        else if (lastDir == Vector2.up)
        {
            zAngle = 0f;   // up -> up
            if (weapon != null) weapon.localScale = new Vector3(1, 1, 1);
        }
        else // down
        {
            zAngle = 180f; // up -> down
            if (weapon != null) weapon.localScale = new Vector3(1, 1, 1);
        }

        firePoint.rotation = Quaternion.Euler(0f, 0f, zAngle);

        // se houver um objeto arma, faça com que acompanhe a rotação também
        if (weapon != null)
        {
            weapon.rotation = firePoint.rotation;
        }
    }

    void Shoot()
    {
        fireTimer += Time.deltaTime;
        if (Input.GetKey(KeyCode.Space) && fireTimer >= fireRate)
        {
            GameObject b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            // garante que o projétil não herde flips indesejados do pai/arma
            b.transform.localScale = Vector3.one;
            fireTimer = 0f;
        }
    }
}
