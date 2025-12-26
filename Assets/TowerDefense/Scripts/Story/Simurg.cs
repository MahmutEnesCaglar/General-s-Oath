using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Story
{
    /// <summary>
    /// Bir diyalog cümlesini temsil eder
    /// </summary>
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;      // Konuşan (Simurg, General, vs.)
        public string text;         // Konuşma metni
        public float displayTime;   // Ekranda kalma süresi (saniye)

        public DialogueLine(string speaker, string text, float displayTime = 3f)
        {
            this.speaker = speaker;
            this.text = text;
            this.displayTime = displayTime;
        }
    }

    /// <summary>
    /// Bir hikaye bölümünü temsil eder
    /// </summary>
    [System.Serializable]
    public class StorySegment
    {
        public string segmentName;              // Bölüm adı
        public List<DialogueLine> dialogues;    // Diyaloglar

        public StorySegment(string name)
        {
            this.segmentName = name;
            this.dialogues = new List<DialogueLine>();
        }

        public void AddDialogue(string speaker, string text, float displayTime = 3f)
        {
            dialogues.Add(new DialogueLine(speaker, text, displayTime));
        }
    }

    /// <summary>
    /// Simurg - Efsanevi hikaye anlatıcısı
    /// Oyunun hikayesini ve geçişlerini yönetir
    /// 
    /// Simurg, Türk-Fars mitolojisindeki tüm bilgeliğe sahip efsanevi kuştur.
    /// Oyunda rehber rolündedir ve oyuncuya haritalar arası geçişte hikaye anlatır.
    /// </summary>
    public class Simurg : MonoBehaviour
    {
        [Header("Simurg Bilgileri")]
        public string narratorName = "Simurg";
        public string description = "Bilgelik ve güç sahibi efsanevi kuş";

        [Header("Hikaye Bölümleri")]
        private Dictionary<string, StorySegment> storySegments;

        private void Awake()
        {
            InitializeStory();
        }

        /// <summary>
        /// Tüm hikaye bölümlerini oluşturur
        /// </summary>
        private void InitializeStory()
        {
            storySegments = new Dictionary<string, StorySegment>();

            CreateIntroduction();
            CreateGrifonStory();
            CreateKirinStory();
            CreateEjderhaStory();
            CreateEnding();
        }

        #region HİKAYE BÖLÜMLER

        /// <summary>
        /// Oyun başlangıcı - Simurg'un tanıtımı
        /// </summary>
        private void CreateIntroduction()
        {
            StorySegment intro = new StorySegment("Başlangıç");

            intro.AddDialogue("Simurg", 
                "Merhaba genç savaşçı. Ben Simurg, tüm zamanların tanığı ve bilgelik koruyucusuyum.", 4f);
            
            intro.AddDialogue("Simurg", 
                "Karanlık güçler dünyamızı tehdit ediyor. Üç efsanevi yaratık, kötü ruhlar tarafından kontrol ediliyor.", 5f);
            
            intro.AddDialogue("Simurg", 
                "Sen, onları özgür bırakmalı ve dünyamızı kurtarmalısın. Her birinin koruyucusu seni bekliyor.", 5f);
            
            intro.AddDialogue("Simurg", 
                "İlk yolculuğun Grifon'un Dağlarına olacak. Hazır mısın?", 3f);

            storySegments.Add("intro", intro);
        }

        /// <summary>
        /// Harita 1 - Grifon'un hikayesi
        /// </summary>
        private void CreateGrifonStory()
        {
            // Harita öncesi
            StorySegment preGrifon = new StorySegment("Grifon Öncesi");
            
            preGrifon.AddDialogue("Simurg", 
                "Grifon'un Dağları... Burası bir zamanlar huzur doluydu.", 4f);
            
            preGrifon.AddDialogue("Simurg", 
                "General Altay, Grifon'un gücüyle bu toprakları koruyordu.", 4f);
            
            preGrifon.AddDialogue("Simurg", 
                "Ancak karanlık güçler Grifon'u esir aldı. Şimdi dağlar düşman akınlarıyla dolu.", 5f);
            
            preGrifon.AddDialogue("General Altay", 
                "Yabancı! Yardımına ihtiyacım var. Grifon'u özgür bırakmadan bu saldırılar durmayacak!", 5f);
            
            preGrifon.AddDialogue("Simurg", 
                "Kulelerini yerleştir, savunmanı kur. 10 dalga düşman gelecek. Son dalgada Grifon'la yüzleşeceksin.", 5f);

            storySegments.Add("pre_grifon", preGrifon);

            // Harita sonrası
            StorySegment postGrifon = new StorySegment("Grifon Sonrası");
            
            postGrifon.AddDialogue("General Altay", 
                "Başardık! Grifon özgür! Dağlar yeniden güvende!", 4f);
            
            postGrifon.AddDialogue("Simurg", 
                "İyi iş çıkardın genç savaşçı. Ancak yolculuk daha yeni başlıyor.", 4f);
            
            postGrifon.AddDialogue("Simurg", 
                "Doğuda, Kirin'in Bahçeleri seni bekliyor. Hazırlan, çünkü zorluk artıyor...", 5f);

            storySegments.Add("post_grifon", postGrifon);
        }

        /// <summary>
        /// Harita 2 - Kirin'in hikayesi
        /// </summary>
        private void CreateKirinStory()
        {
            // Harita öncesi
            StorySegment preKirin = new StorySegment("Kirin Öncesi");
            
            preKirin.AddDialogue("Simurg", 
                "Kirin'in Bahçeleri... Mistik güçlerle dolu bu topraklar, artık lanetli.", 5f);
            
            preKirin.AddDialogue("General Aylin", 
                "Hoş geldin kahraman. Senin hakkında çok şey duydum.", 4f);
            
            preKirin.AddDialogue("General Aylin", 
                "Kirin, doğanın dengesinin koruyucusuydu. Şimdi o da karanlığa hizmet ediyor.", 5f);
            
            preKirin.AddDialogue("Simurg", 
                "Bu sefer daha zor olacak. Havadan düşmanlar da gelecek. Dikkatli ol!", 5f);
            
            preKirin.AddDialogue("General Aylin", 
                "Kirin'i yenmek için sadece güç değil, strateji de gerekecek. Ben seninle birlikteyim!", 5f);

            storySegments.Add("pre_kirin", preKirin);

            // Harita sonrası
            StorySegment postKirin = new StorySegment("Kirin Sonrası");
            
            postKirin.AddDialogue("General Aylin", 
                "Kirin özgür! Bahçeler yeniden hayat buluyor!", 4f);
            
            postKirin.AddDialogue("Simurg", 
                "İki efsanevi yaratık kurtarıldı. Ancak en büyük sınav henüz gelmedi.", 5f);
            
            postKirin.AddDialogue("Simurg", 
                "Ejderha'nın Kalesi... Karanlığın merkezi. Hazır mısın son savaşa?", 5f);

            storySegments.Add("post_kirin", postKirin);
        }

        /// <summary>
        /// Harita 3 - Ejderha'nın hikayesi
        /// </summary>
        private void CreateEjderhaStory()
        {
            // Harita öncesi
            StorySegment preEjderha = new StorySegment("Ejderha Öncesi");
            
            preEjderha.AddDialogue("Simurg", 
                "Burası... Ejderha'nın Kalesi. Karanlığın kalbi burası.", 5f);
            
            preEjderha.AddDialogue("General Kaan", 
                "Sonunda geldin. Çok bekledim bu anı.", 4f);
            
            preEjderha.AddDialogue("General Kaan", 
                "Ejderha, en güçlü efsanevi yaratıktı. Şimdi en büyük tehdidimiz.", 5f);
            
            preEjderha.AddDialogue("Simurg", 
                "Bu son savaş. Tüm gücünü kullan, tüm bilgeliğini. Yoksa her şey kaybolacak.", 6f);
            
            preEjderha.AddDialogue("General Kaan", 
                "Seninle omuz omuza savaşacağım. Hadi gösterelim karanlığa gücümüzü!", 5f);

            storySegments.Add("pre_ejderha", preEjderha);

            // Harita sonrası
            StorySegment postEjderha = new StorySegment("Ejderha Sonrası");
            
            postEjderha.AddDialogue("General Kaan", 
                "BAŞARDIK! Ejderha özgür! Karanlık yenildi!", 5f);
            
            postEjderha.AddDialogue("Simurg", 
                "Tebrikler genç kahraman. Üç efsanevi yaratığı kurtardın.", 5f);
            
            postEjderha.AddDialogue("Simurg", 
                "Dünya yeniden dengeye kavuştu. Ve sen... Sen bir efsane oldun.", 5f);
            
            postEjderha.AddDialogue("Simurg", 
                "Adın sonsuza kadar anılacak. Kahramanlar Salonu'nda seni bekliyor!", 5f);

            storySegments.Add("post_ejderha", postEjderha);
        }

        /// <summary>
        /// Oyun sonu
        /// </summary>
        private void CreateEnding()
        {
            StorySegment ending = new StorySegment("Son");
            
            ending.AddDialogue("Simurg", 
                "Yolculuğun sona erdi, ancak efsanen yeni başlıyor.", 4f);
            
            ending.AddDialogue("Simurg", 
                "Üç general, üç efsanevi yaratık... Hepsi sana minnettar.", 5f);
            
            ending.AddDialogue("Simurg", 
                "Ben, Simurg, senin kahramanlığını sonsuza kadar hatırlayacağım.", 5f);
            
            ending.AddDialogue("Simurg", 
                "Elveda genç kahraman. Yolların hep aydınlık olsun!", 4f);

            storySegments.Add("ending", ending);
        }

        #endregion

        /// <summary>
        /// Belirli bir hikaye bölümünü oynatır
        /// </summary>
        public void PlayStorySegment(string segmentKey)
        {
            if (!storySegments.ContainsKey(segmentKey))
            {
                Debug.LogWarning($"Hikaye bölümü bulunamadı: {segmentKey}");
                return;
            }

            StorySegment segment = storySegments[segmentKey];
            Debug.Log($"\n=== {segment.segmentName} ===\n");

            foreach (var dialogue in segment.dialogues)
            {
                Debug.Log($"{dialogue.speaker}: \"{dialogue.text}\"");
            }

            Debug.Log(""); // Boş satır
        }

        /// <summary>
        /// Tüm hikayeyi sırayla oynatır (Test için)
        /// </summary>
        public void PlayFullStory()
        {
            Debug.Log("\n" + new string('=', 70));
            Debug.Log("=== SİMURG'UN HİKAYESİ - TAM OYNANIŞI ===");
            Debug.Log(new string('=', 70) + "\n");

            PlayStorySegment("intro");
            PlayStorySegment("pre_grifon");
            PlayStorySegment("post_grifon");
            PlayStorySegment("pre_kirin");
            PlayStorySegment("post_kirin");
            PlayStorySegment("pre_ejderha");
            PlayStorySegment("post_ejderha");
            PlayStorySegment("ending");

            Debug.Log(new string('=', 70));
        }

        /// <summary>
        /// Harita başlamadan önce hikaye göster
        /// </summary>
        public void ShowPreMapStory(int mapIndex)
        {
            string[] keys = { "pre_grifon", "pre_kirin", "pre_ejderha" };
            if (mapIndex >= 0 && mapIndex < keys.Length)
            {
                PlayStorySegment(keys[mapIndex]);
            }
        }

        /// <summary>
        /// Harita bittikten sonra hikaye göster
        /// </summary>
        public void ShowPostMapStory(int mapIndex)
        {
            string[] keys = { "post_grifon", "post_kirin", "post_ejderha" };
            if (mapIndex >= 0 && mapIndex < keys.Length)
            {
                PlayStorySegment(keys[mapIndex]);
            }
        }
    }
}
