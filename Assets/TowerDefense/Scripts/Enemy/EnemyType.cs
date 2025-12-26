using UnityEngine;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Düşman türlerinin temel özelliklerini tanımlar
    /// </summary>
    [System.Serializable]
    public class EnemyType
    {
        public string type;           // Düşman tipi adı
        public int hp;                // Can
        public int damage;            // Hasar
        public int speed;             // Hız
        public int money;             // Öldürüldüğünde kazanılan para
        public bool isFlying;         // Havada uçuyor mu?
        public bool isBoss;           // Boss mu?

        public EnemyType(string type, int hp, int damage, int speed, int money, bool isFlying = false, bool isBoss = false)
        {
            this.type = type;
            this.hp = hp;
            this.damage = damage;
            this.speed = speed;
            this.money = money;
            this.isFlying = isFlying;
            this.isBoss = isBoss;
        }
    }
}
