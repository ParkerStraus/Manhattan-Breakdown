using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
public class DropShadow : MonoBehaviour
{
    public Vector2 ShadowOffset;
    public Material ShadowMaterial;

    SpriteRenderer spriteRenderer;
    GameObject shadowGameObject;
    SpriteRenderer shadowSpriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Create a new gameobject to be used as drop shadow
        shadowGameObject = new GameObject("Shadow");
            // Create a new SpriteRenderer for Shadow gameobject
            shadowSpriteRenderer = shadowGameObject.AddComponent<SpriteRenderer>();

            // Set the shadow gameobject's sprite to the original sprite
            shadowSpriteRenderer.sprite = spriteRenderer.sprite;
            // Set the shadow gameobject's material to the shadow material we created
            shadowSpriteRenderer.material = ShadowMaterial;

            // Update the sorting layer of the shadow to always lie behind the sprite
            shadowSpriteRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
            shadowSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        
    }

    void LateUpdate()
    {
        // Update the position and rotation of the sprite's shadow with moving sprite
        shadowGameObject.transform.localPosition = transform.position + (Vector3)ShadowOffset;
        shadowGameObject.transform.localRotation = transform.localRotation;

        if (spriteRenderer != null && spriteRenderer.enabled)
        {
            shadowSpriteRenderer.sprite = spriteRenderer.sprite;
            shadowSpriteRenderer.flipX = spriteRenderer.flipX;
            shadowSpriteRenderer.flipY = spriteRenderer.flipY;
        }
    }

    void OnDestroy()
    {
        GameObject.Destroy(shadowGameObject);
    }

}
