using UnityEngine;

namespace WXQToolKits.FolderKit.Singleton
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public bool global = true;
        static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType<T>();
                }
                return _instance;
            }

        }


        private void Awake()
        {
            Debug.LogWarningFormat("{0}{1} awake" ,typeof(T),this.GetInstanceID());
            if (global)
            {
                if (_instance != null && _instance != this.gameObject.GetComponent<T>())
                {
                    Destroy(this.gameObject);
                    return;
                }
                DontDestroyOnLoad(this.gameObject);
                _instance = gameObject.GetComponent<T>();

            }
            this.OnStart();
        }

        protected virtual void OnStart()
        {

        }
    }
}