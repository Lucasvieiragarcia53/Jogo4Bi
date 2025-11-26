using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float vidasMaximas = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform weapon;

    public Transform playerSprite;

    public float fireRate = 0.2f;

    private float fireTimer;
    private Vector2 lastDir = Vector2.right;

    private Bullet bullet;
    public Placar placar;
    private Projectile proj;

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
            transform.position += input.normalized * speed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            lastDir = Vector2.up;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            lastDir = Vector2.down;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            lastDir = Vector2.right;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            lastDir = Vector2.left;

        UpdatePlayerRotation();
    }

    void UpdatePlayerRotation()
    {
        if (lastDir == Vector2.up)
            playerSprite.rotation = Quaternion.Euler(0, 0, 0);

        else if (lastDir == Vector2.right)
            playerSprite.rotation = Quaternion.Euler(0, 0, -90);

        else if (lastDir == Vector2.left)
            playerSprite.rotation = Quaternion.Euler(0, 0, 90);

        else if (lastDir == Vector2.down)
            playerSprite.rotation = Quaternion.Euler(0, 0, 180);
    }

    void Aim()
    {
        float zAngle = 0f;

        if (lastDir == Vector2.right)
            zAngle = -90f;

        else if (lastDir == Vector2.left)
            zAngle = 90f;

        else if (lastDir == Vector2.up)
            zAngle = 0f;

        else
            zAngle = 180f;

        firePoint.rotation = Quaternion.Euler(0f, 0f, zAngle);
        if (weapon != null) weapon.rotation = firePoint.rotation;
    }

    void Shoot()
    {
        fireTimer += Time.deltaTime;
        int oneshot = 1;

        int bulletmax = GameObject.FindGameObjectsWithTag("Bullet").Length;

        if (Input.GetKeyDown(KeyCode.Space) && fireTimer >= fireRate)
        {
            GameObject b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            if(bulletmax < oneshot)
            {
                Destroy(bulletPrefab);
            }


            // Envia a direção da última tecla apertada
            // b.GetComponent<Projectile>().SetDirection(lastDir);
            if(proj != null)
            {
            proj.GetComponent<Rigidbody2D>().velocity = lastDir.normalized * 10f;
            }
            fireTimer = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("BulletEnemy"))
        {

            if (placar != null)
            {
                placar.PerderVida();
            }
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Special"))
        {
            if (placar != null)
            {
                placar.PerderVidaEspecial();
            }
            
        }
    }    
}
