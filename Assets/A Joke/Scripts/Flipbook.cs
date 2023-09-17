using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GG23.Game.Joke
{
    public class Flipbook : MonoBehaviour
    {
        [SerializeField] private int maxPanelImages=3;
        [SerializeField] private Transform flipbook;
        [SerializeField] private GameObject flipbookPageRef;
        [SerializeField] private AudioSource sourceToPlay;
        private void Start()
        {
            SpawnPages();
            StartCoroutine(DoTheFlipbook());
        }

        private void SpawnPages()
        {
            for (int i = 0; i < maxPanelImages; i++)
            {
                var go = Instantiate(flipbookPageRef, flipbook);
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>($"{(i%maxPanelImages)+1}");
            }
            
            Destroy(flipbookPageRef);
        }
        
        private IEnumerator DoTheFlipbook()
        {
           yield return new WaitForEndOfFrame();
           
            float maxLoopTime = Time.time + 16;
            while (Time.time <= maxLoopTime)
            {
                float timeStep = 0;
                var rTrf = flipbook.GetChild(maxPanelImages-1).GetComponent<RectTransform>();
                var startPos = rTrf.anchoredPosition;
                var endPos = new Vector2(startPos.x, 0);
                
                if(sourceToPlay != null)
                    sourceToPlay.Play();
                
                while (timeStep <= 1)
                {
                    timeStep += Time.deltaTime / 0.125f;
                    rTrf.anchoredPosition = Vector2.Lerp(startPos, endPos, timeStep);
                    yield return new WaitForEndOfFrame();
                }
                
                yield return new WaitForEndOfFrame();

                Transform trf = flipbook.GetChild(0);
                trf.GetComponent<RectTransform>().anchoredPosition = startPos;
                
                rTrf.SetAsFirstSibling();
            }
        }
    }
}