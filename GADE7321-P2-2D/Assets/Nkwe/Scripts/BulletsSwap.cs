using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [System.Serializable]
    public class BulletType
    {
        public GameObject bulletPrefab; // The bullet prefab
        public int maxBullets; // Maximum number of bullets the player can have for this type
        [HideInInspector]
        public int currentBullets; // Current number of bullets the player has for this type
    }

    public BulletType[] bulletTypes; // Array to hold different bullet types

    private int currentBulletIndex = 0; // Index to keep track of the current bullet

    void Start()
    {
        // Initialize bullet counts for each type
        foreach (BulletType type in bulletTypes)
        {
            type.currentBullets = type.maxBullets;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchBullet(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchBullet(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchBullet(2);
        }
    }

    void SwitchBullet(int bulletIndex)
    {
        if (bulletIndex >= 0 && bulletIndex < bulletTypes.Length)
        {
            // Check if the player has bullets left for the selected type
            if (bulletTypes[bulletIndex].currentBullets > 0)
            {
                currentBulletIndex = bulletIndex;
                Debug.Log("Switched to bullet: " + bulletTypes[bulletIndex].bulletPrefab.name);
            }
            else
            {
                Debug.LogWarning("Out of bullets for this type!");
            }
        }
        else
        {
            Debug.LogWarning("Invalid bullet index!");
        }
    }

    public GameObject GetCurrentBullet()
    {
        return bulletTypes[currentBulletIndex].bulletPrefab;
    }

    public void ReduceBulletCount()
    {
        bulletTypes[currentBulletIndex].currentBullets--;
    }

    public void ReloadCurrentBulletType()
    {
        bulletTypes[currentBulletIndex].currentBullets = bulletTypes[currentBulletIndex].maxBullets;
    }
}

}

