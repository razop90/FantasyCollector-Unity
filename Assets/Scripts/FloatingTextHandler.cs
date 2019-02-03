using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class FloatingTextHandler : MonoBehaviour
    {
        public static FloatingText popupText;
        public static GameObject canvas;

        public static void Initialize()
        {
            canvas = GameObject.Find("Damage Canvas");
            if (!popupText)
                popupText = Resources.Load<FloatingText>("Prefabs/PopupTextParent");
        }

        public static void CreateFloatingText(Transform location, string text, Color color, int fontSize = 18)
        {
            CreateFloatingText(location.position, text, color, fontSize);
        }

        public static void CreateFloatingText(Vector3 position, string text, Color color, int fontSize = 18)
        {
            FloatingText instance = Instantiate(popupText);
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(new Vector3(position.x + Random.Range(-2f, 2f), position.y + Random.Range(-2f, 2f), position.z));

            instance.SetText(text, color, fontSize);
            instance.transform.SetParent(canvas.transform, false);
            instance.transform.position = screenPosition;
        }
    }
}
