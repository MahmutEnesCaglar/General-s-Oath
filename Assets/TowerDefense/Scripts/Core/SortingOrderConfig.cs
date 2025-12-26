namespace TowerDefense.Core
{
    /// <summary>
    /// Tüm projede kullanılan Sorting Order sabitlerini tanımlar
    /// Tek bir yerden merkezi yönetim sağlar
    ///
    /// SORTING ORDER MİMARİSİ:
    /// 0-9:   Background (arka plan, zemin efektleri)
    /// 10-19: Tilemap (harita, yapılar) - Şu an kullanılan: 10-13
    /// 20-99: Characters (düşmanlar, kuleler, projectile'lar) - Dinamik Y-based
    /// 100+:  UI Effects (can barları, damage text, partiküller)
    ///
    /// KULLANIM:
    /// - Enemy, Tower gibi karakterler base sorting'i kullanır
    /// - Y pozisyonuna göre otomatik offset eklenir
    /// - Böylece izometrik depth illüzyonu sağlanır
    /// </summary>
    public static class SortingOrderConfig
    {
        // ===== LAYER RANGE'LERİ =====

        /// <summary>Background layer - Zemin, arka plan sprite'ları</summary>
        public const int BACKGROUND_MIN = 0;
        public const int BACKGROUND_MAX = 9;

        /// <summary>Tilemap layer - Harita, yollar, yapılar</summary>
        public const int TILEMAP_MIN = 10;
        public const int TILEMAP_MAX = 19;

        /// <summary>Character layer - Düşmanlar, kuleler, mermiler</summary>
        public const int CHARACTER_MIN = 20;
        public const int CHARACTER_MAX = 99;

        /// <summary>UI Effects layer - Can barları, damage text</summary>
        public const int UI_EFFECTS_MIN = 100;
        public const int UI_EFFECTS_MAX = 999;

        // ===== CHARACTER SORTING (Dinamik Y-based) =====

        /// <summary>
        /// Karakterler için base sorting order
        /// Tilemap'in üstünde, UI effect'lerin altında
        /// </summary>
        public const int CHARACTER_BASE_SORTING = 50;

        /// <summary>
        /// Y pozisyonunun sorting order'a etkisi
        /// Daha yüksek değer = daha fazla derinlik etkisi
        /// Örnek: 10 ile çarpılırsa, Y=2 ile Y=0 arasında 20 sorting farkı olur
        /// </summary>
        public const int Y_POSITION_MULTIPLIER = 10;

        // ===== SPESİFİK LAYER'LAR =====

        /// <summary>Projectile (mermi) base sorting</summary>
        public const int PROJECTILE_BASE = 60;

        /// <summary>Health bar base sorting (karakterlerin üstünde)</summary>
        public const int HEALTH_BAR_BASE = 100;

        /// <summary>Damage text base sorting (en üstte)</summary>
        public const int DAMAGE_TEXT_BASE = 200;

        // ===== HELPER METHODS =====

        /// <summary>
        /// Y pozisyonuna göre karakter sorting order hesaplar
        /// </summary>
        /// <param name="yPosition">Karakterin Y pozisyonu</param>
        /// <returns>Hesaplanan sorting order</returns>
        public static int GetCharacterSortingOrder(float yPosition)
        {
            int yOffset = -(int)(yPosition * Y_POSITION_MULTIPLIER);
            int sortingOrder = CHARACTER_BASE_SORTING + yOffset;

            // Karakterler her zaman kendi range'inde kalmalı
            if (sortingOrder < CHARACTER_MIN) sortingOrder = CHARACTER_MIN;
            if (sortingOrder > CHARACTER_MAX) sortingOrder = CHARACTER_MAX;

            return sortingOrder;
        }

        /// <summary>
        /// Y pozisyonuna göre projectile sorting order hesaplar
        /// </summary>
        public static int GetProjectileSortingOrder(float yPosition)
        {
            int yOffset = -(int)(yPosition * Y_POSITION_MULTIPLIER);
            return PROJECTILE_BASE + yOffset;
        }
    }
}
