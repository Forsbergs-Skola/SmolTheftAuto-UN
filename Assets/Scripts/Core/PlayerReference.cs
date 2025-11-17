using UnityEngine;

namespace SmolTheftAuto.Core
{
    // Provides centralized access to player reference without tight coupling
    // Allows other systems to find the player without direct dependencies
    public class PlayerReference : MonoBehaviour
    {
        private static PlayerReference instance;
        private Transform playerTransform;
        private GameObject playerGameObject;

        public static Transform PlayerTransform => instance != null ? instance.playerTransform : null;
        public static GameObject PlayerGameObject => instance != null ? instance.playerGameObject : null;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            FindPlayer();
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag(GameConstants.Tags.PLAYER);
            if (player != null)
            {
                playerGameObject = player;
                playerTransform = player.transform;
            }
        }

        public static T GetPlayerComponent<T>() where T : class
        {
            if (instance != null && instance.playerGameObject != null)
            {
                if (typeof(T).IsInterface)
                {
                    return instance.playerGameObject.GetComponent(typeof(T)) as T;
                }
                return instance.playerGameObject.GetComponent<T>();
            }
            return null;
        }
    }
}
