
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    [SerializeField] private MeshRenderer Renderer;
    [SerializeField] public Texture2D[] backgrounds;
    [SerializeField] private int Increment = 0;
    private static System.Random rng = new System.Random();
    // Start is called before the first frame update
    void Start()
    {
        Shuffle<Texture2D>(backgrounds); 
        StartCoroutine(DisplayBackground());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public static void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }

    private IEnumerator DisplayBackground()
    {
        while (true) {
            //Animate background
            Renderer.material.SetTexture("_MainTex", backgrounds[Increment]);
            //if at end of array shuffle and set increment to 0
            yield return new WaitForSeconds(15f);
            Increment++;
            if(Increment == backgrounds.Length)
            {
                Increment = 0;
                Shuffle<Texture2D>(backgrounds);
            }
            //at end of animation start it again
            yield return null;
        }
    }
}
