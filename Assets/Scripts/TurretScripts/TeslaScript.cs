using UnityEngine;
using System.Collections;

public class TeslaScript : TurretScript {

    private GameObject target;
    private Vector3 direction;

    // Use this for initialization
    protected override void Start()
    {
        //onGrid; 
        base.Start();
        minAttackDamage = 5;
        maxAttackDamage = 8;
        attackSpeed = 0.25f;
        proximity = 2f;
        direction = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (target)
        {
            if (!transform.GetChild(0).gameObject.activeInHierarchy)
                transform.GetChild(0).gameObject.SetActive(true);

            direction =  new Vector3(target.transform.position.x, 0, target.transform.position.z) 
                - new Vector3(transform.position.x, 0, transform.position.z);
            transform.rotation = Quaternion.LookRotation(direction.normalized);
        }
        else
        {
            if (transform.GetChild(0).gameObject.activeInHierarchy)
                transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    protected override Collider[] EnemiesInAttackRadius()
    {
        if (base.EnemiesInAttackRadius() == null)
            target = null;
        return base.EnemiesInAttackRadius();
    }

    protected override void Attacking(Collider[] enemies)
    {
        switch (attackStyle)
        {
            case ATTSTYLE.NEAREST_FIRST:
                {
                    float nearestDistance = proximity;
                    foreach (Collider enemy in enemies)
                    {
                        if ((transform.position - enemy.transform.position).magnitude < nearestDistance)
                        {
                            nearestDistance = (enemy.transform.position - transform.position).magnitude;
                            target = enemy.transform.gameObject;
                        }
                    }
                    break;
                }

            case ATTSTYLE.FURTHEST_FIRST:
                {
                    float longestDistance = 0f;
                    foreach (Collider enemy in enemies)
                    {
                        if ((transform.position - enemy.transform.position).magnitude >= longestDistance)
                        {
                            longestDistance = (enemy.transform.position - transform.position).magnitude;
                            target = enemy.transform.gameObject;
                        }
                    }
                    break;
                }
        }

        //Debug.DrawRay(transform.position, direction, new Color(1, 0, 1), 10);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, proximity))
        {
           
        }
    }

    public void LevelUp()
    {
        LevelUpgrades(1, 2, 0.05f, 0.5f);
    }
}
