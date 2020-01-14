using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace LibraryPersonal
{
    public struct UtilNames
    {
        public const string PLAYER_DATA = "playerdata.dat";
    }
    public static class Datos
    {
        public static void ShowToast(string note)
        {
            if (Ruta.RunningOnAndroid())
            {
                var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toast = new AndroidJavaClass("android.widget.Toast").CallStatic<AndroidJavaObject>("makeText", activity, note, 1);
                    toast.Call("show");
                }));
                return;
            }
            Debug.Log(note);
        }
        public static string RandomId(int LengthNewId = 15, bool Mayusculas = true, bool minusculas = true, bool simbolos = true)
        {
            if (!Mayusculas && !minusculas && !simbolos || LengthNewId <= 0)
                return "Null";
            string newId = "";
            string abecedarioMayus = "A-B-C-D-E-F-G-H-I-J-K-L-M-N-Ñ-O-P-Q-R-S-T-U-V-W-X-Y-Z";
            string abecedarioMinus = "a-b-c-d-e-f-g-h-i-j-k-l-m-n-ñ-o-p-q-r-s-t-u-v-w-x-y-z";
            string caracteres = "-_-/--&-";
            Random r = new Random();

            int actualLength = 0;
            while(actualLength < LengthNewId)
            {
                int value = r.Next(0, 1);
                string letra = "";
                switch (value)
                {
                    case 0:
                        int proba_Char = r.Next(0, 5);
                        if (Mayusculas)
                        {
                            if (proba_Char == 2)
                            {
                                if (simbolos)
                                {
                                    letra = caracteres.Split('-')[r.Next(0, 4)] + abecedarioMayus.Split('-')[r.Next(0, 26)];
                                    actualLength++;
                                }
                            }
                            else
                            {
                                letra = abecedarioMayus.Split('-')[r.Next(0, 26)];
                                actualLength++;
                            }
                        }
                        break;
                    case 1:
                        int proba_Char2 = r.Next(0, 5);
                        if (minusculas)
                        {
                            if (proba_Char2 == 3)
                            {
                                if (simbolos)
                                {
                                    letra = caracteres.Split('-')[r.Next(0, 4)] + abecedarioMinus.Split('-')[r.Next(0, 26)];
                                    actualLength++;
                                }
                            }
                            else
                            {
                                letra = abecedarioMinus.Split('-')[r.Next(0, 26)];
                                actualLength++;
                            }
                        }
                        break;
                }
                newId = newId + letra;
            }
            return newId;
        }

        public static void CreateFilesToDownload(string path, string dataText)
        {
            string directoryPath = path.Replace(Path.GetFileName(path), "");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            File.WriteAllText(path, dataText);
        }
        public static RemoteSettings remoteSettings = null;
        internal static string[] GetIndex()
        {
            string dex = File.ReadAllText(Ruta.DirAssets + "dex");
            string[] splits = new string[dex.Split('\n').Length - 1];
            for (int i = 0; i < dex.Split('\n').Length - 1; i++)
            {
                splits[i] = dex.Split('\n')[i];
                string directoryPath = splits[i].Split('|')[1].Replace(Path.GetFileName(splits[i].Split('|')[1]), "");
                if (!Directory.Exists(Ruta.DirAssets + directoryPath))
                    Directory.CreateDirectory(Ruta.DirAssets + directoryPath);
                if (File.Exists(Ruta.DirAssets + splits[i].Split('|')[1]))
                    File.Delete(Ruta.DirAssets + splits[i].Split('|')[1]);
            }
            return splits;
        }
        public static IEnumerator ConfigRemoteSettings(DataManager d)
        {
            WWW conexion = new WWW("https://sergioribera.000webhostapp.com/RemoteSettings/php/datos.php?proyect=CubeShooter");
            yield return conexion;
            if (conexion.bytesDownloaded == 0 || conexion.text == "404" || conexion.text == "302" || conexion.error != null)
            {
                //error de coneccion
                ShowToast("Revisa Tu Coneccion a Internet");
                if (Ruta.ExistFile(NameFiles.RemoteSettings))
                {
                    remoteSettings = Load<RemoteSettings>(NameFiles.RemoteSettings);
                }
            }
            else
            {
                string jsonString = "{" + '"' + "remoteDatas" + '"' + ':' + conexion.text + '}';
                remoteSettings = JsonUtility.FromJson<RemoteSettings>(jsonString);
                remoteSettings.Save<RemoteSettings>(NameFiles.RemoteSettings);
            }
            d.remoteSettings = remoteSettings;
            d.Compare_Version();
        }

        /// <summary>
        /// Extensión que devuelve un Objeto apartir de un string que contiene el JSON
        /// </summary>
        /// <typeparam name="T">Corresponde a la clase que devolverá</typeparam>
        /// <param name="s">Corresponde al string que contiene el JSON del objeto</param>
        /// <returns></returns>
        public static T Deserializar<T>(this string s, bool encrypt = true)
        {
            try
            {
                if (encrypt)
                    return JsonUtility.FromJson<T>(s.Desencrypt());
                return JsonUtility.FromJson<T>(s);
            }
            catch (Exception)
            {
                if (encrypt)
                    s = s.Desencrypt();
                object obj = JsonUtility.FromJson<object>(s);
                T data = default;
                foreach (var objProperties in obj.GetType().GetProperties())
                {
                    foreach (var dataProperties in data.GetType().GetProperties())
                    {
                        if (objProperties.Name == dataProperties.Name)
                            data.GetType().GetProperty(objProperties.Name).SetValue(objProperties.GetType(), objProperties.GetValue(objProperties.GetType()));
                    }
                }
                return data;
            }
        }
        /// <summary>
        /// Extensión que devuelve un JSON del objeto extensionado
        /// </summary>
        /// <param name="o">Corresponde al objeto a Serializar</param>
        /// <returns></returns>
        public static string Serializar(this object o, bool encrypt = true)
        {
            if (encrypt)
                return JsonUtility.ToJson(o).Encrypt();
            return JsonUtility.ToJson(o);
        }
        static T Instancia<T>()
        {
            T obj = default;
            if (typeof(T).IsValueType)
            {
                obj = default;
            }
            else if (typeof(T) == typeof(string))
            {
                obj = (T)Convert.ChangeType(string.Empty, typeof(T));
            }
            else
            {
                obj = Activator.CreateInstance<T>();
            }
            return obj;
        }

        static AndroidJavaClass environment;
        public static string GetPath {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    environment = new AndroidJavaClass("android.os.Environment");
                    using (AndroidJavaObject externalStorageDirectory = environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
                    {
                        string root = externalStorageDirectory.Call<string>("getPath") + "/" + Application.companyName;

                        return root + "/Sergio Ribera/" + Application.productName + "/";
                    }
                }
                return "C://Sergio Ribera/" + Application.productName + "/";
            }
        }
        public static bool Exist(string nameFile)
        {
            string path = GetPath;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += nameFile;
            return File.Exists(path);
        }
        public static T Save<T>(this object o, string nameFile)
        {
            string path = GetPath;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += nameFile;
            File.WriteAllText(path, o.Serializar());
            return (T)o;
        }
        public static T Load<T>(this object o, string nameFile)
        {
            string path = GetPath;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (o == null) return Instancia<T>();
            path += nameFile;
            if (!File.Exists(path)) return Instancia<T>();
            o = File.ReadAllText(path).Deserializar<T>();
            return (T)o;
        }
        public static T Load<T>(string nameFile)
        {
            string path = GetPath;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += nameFile;
            if (!File.Exists(path)) return Instancia<T>();
            return File.ReadAllText(path).Deserializar<T>();
        }
        public static void Foreach(this List<string> l, Action<string> a)
        {
            if (l.Count > 0)
            {
                foreach (var i in l)
                {
                    a(i);
                }
            }
            else
                a(null);
        }

        static readonly string key = "Key Where Cube Shooter";
        /// <summary>
        /// es =Devuelve un string encriptado
        /// </summary>
        /// <param name="text">Establece el string a Encriptar</param>
        /// <returns></returns>
        public static string Encrypt(this string text)
        {
            //arreglo de bytes donde guardaremos la llave
            byte[] keyArray;
            //arreglo de bytes donde guardaremos el texto
            //que vamos a encriptar
            byte[] Arreglo_a_Cifrar = UTF8Encoding.UTF8.GetBytes(text);
            //se utilizan las clases de encriptación
            //provistas por el Framework
            //Algoritmo MD5
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            //se guarda la llave para que se le realice
            //hashing
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

            hashmd5.Clear();

            //Algoritmo 3DAS
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            //se empieza con la transformación de la cadena
            ICryptoTransform cTransform = tdes.CreateEncryptor();

            //arreglo de bytes dond
            byte[] ArrayResultado = cTransform.TransformFinalBlock(Arreglo_a_Cifrar, 0, Arreglo_a_Cifrar.Length);

            tdes.Clear();

            //se regresa el resultado en forma de una cadena
            return Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);
        }
        /// <summary>
        /// es = Devuelve un string Desencriptado
        /// </summary>
        /// <param name="text">Establece el string a Desencriptar</param>
        /// <returns></returns>
        public static string Desencrypt(this string text)
        {
            byte[] keyArray;
            //convierte el texto en una secuencia de bytes
            byte[] Array_a_Descifrar = Convert.FromBase64String(text);

            //se llama a las clases que tienen los algoritmos
            //de encriptación se le aplica hashing
            //algoritmo MD5
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tdes.CreateDecryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(Array_a_Descifrar, 0, Array_a_Descifrar.Length);

            tdes.Clear();

            //se regresa en forma de cadena
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static bool IsEmpty(string[] ss)
        {
            bool b = false;
            foreach (var s in ss)
            {
                b = string.IsNullOrEmpty(s) && string.IsNullOrWhiteSpace(s);
                if (b) throw new Exception(string.Format("El string  {0}  está vacio, es nulo o es solo un espacio e blanco", ss));
            }
            return b;
        }
        public static bool IsEmpty(List<string> ss)
        {
            bool b = false;
            foreach (var s in ss)
            {
                b = string.IsNullOrEmpty(s) && string.IsNullOrWhiteSpace(s);
                if (b) throw new Exception(string.Format("El string  {0}  está vacio, es nulo o es solo un espacio e blanco", ss));
            }
            return b;
        }
        public static bool IsEmpty(string ss)
        {
            bool b = string.IsNullOrEmpty(ss) && string.IsNullOrWhiteSpace(ss);
            if (b) throw new Exception(string.Format("El string  {0}  está vacio, es nulo o es solo un espacio e blanco", ss));
            return b;
        }

        public static bool IsSimilarString(this string s1, string s2)
        {
            bool a = false;

            if (s1.Contains(s2) || s1.ToLower().Contains(s2.ToLower()))
                a = true;

            return a;
        }
        public static string ToFirstUpper(this string s)
        {
            //Separamos el string por espacios para obtener las palabras
            string[] palabras = s.Trim().Split(' ');
            string sR = "";
            //hacemos un recorrido por el array de palabras
            for (int i = 0; i < palabras.Length; i++)
            {
                //Convertimos la palabra en array de caracteres
                char[] letters = palabras[i].ToCharArray();
                if (letters.Length > 1)
                {
                    //si es una palabra mayor a 1 letra y verificamos que son letras
                    if (char.IsLetter(letters[0]))
                        letters[0] = char.Parse(letters[0].ToString().ToUpper());//hacemos el primer caracter de la palabra mayuscula
                }
                //remplazamos la palabra por los nuevos caracteres editados
                palabras[i] = string.Join("", letters);
            }
            //recorremos nuevamente las palabras armando el texto original nuevamente
            foreach (var word in palabras)
            {
                sR += word + " ";
            }
            //recortamos espacios en blanco del principio y el final
            return sR.Trim();
        }

        public static string Format(this string format, params object[] args)
        {
            string newString = format;
            for (int i = 0; i < args.Length; i++)
            {
                string rep = "{" + i + "}";
                newString = newString.Replace(rep, args[i].ToString());
            }
            return newString;
        }
        public static string Compatibilizate(this string s)
        {
            return s.Replace(" ", "");
        }
        public static string Decompatibilizar(this string s)
        {
            //Separamos el string por espacios para obtener las palabras
            string sR = "";
            List<string> palabras = new List<string>();
            //Convertimos la palabra en array de caracteres
            char[] letters = s.ToCharArray();
            //si es una palabra mayor a 1 letra y verificamos que son letras
            for (int i = 0; i < letters.Length; i++)
            {
                if (char.IsLetter(letters[i]))
                    if (char.IsUpper(letters[i]))
                        sR += " ";
                sR += letters[i].ToString();
            }
            //recortamos espacios en blanco del principio y el final
            return sR.Trim();
        }
        public static string RemoveQuotes(this string Value)
        {
            return Value.Replace("\"", "");
        }
        public static float TwoDecimal(this float v)
        {
            return (float) Math.Round(v);
        }
    }    
    public static class ExtraToolColor
    {
        public static string ToHex(this Color color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }
        public static Color hexToColor(this string hex)
        {
            if (hex.Contains("0x"))
                hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            if (hex.Contains("#"))
                hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }
        public static string GetRandomColor()
        {
            string[] c = { "#98FF2A", "#2AFFBA", "#AF2AFF", "#FC2AFF", "#FF2AA5", "#FF2A49", "#FF642A", "#FFBE2A", "#F0FF2A", "#2AFF2B", "#FF2A2E" };
            Random r = new Random();
            int i = r.Next(0, c.Length - 1);
            return c[i];
        }
    }
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        [SerializeField, HideInInspector] int[] _Buckets;
        [SerializeField, HideInInspector] int[] _HashCodes;
        [SerializeField, HideInInspector] int[] _Next;
        [SerializeField, HideInInspector] int _Count;
        [SerializeField, HideInInspector] int _Version;
        [SerializeField, HideInInspector] int _FreeList;
        [SerializeField, HideInInspector] int _FreeCount;
        [SerializeField, HideInInspector] TKey[] _Keys;
        [SerializeField, HideInInspector] TValue[] _Values;

        readonly IEqualityComparer<TKey> _Comparer;

        // Mainly for debugging purposes - to get the key-value pairs display
        public Dictionary<TKey, TValue> AsDictionary
        {
            get { return new Dictionary<TKey, TValue>(this); }
        }

        public int Count
        {
            get { return _Count - _FreeCount; }
        }

        public TValue this[TKey key, TValue defaultValue]
        {
            get
            {
                int index = FindIndex(key);
                if (index >= 0)
                    return _Values[index];
                return defaultValue;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                int index = FindIndex(key);
                if (index >= 0)
                    return _Values[index];
                throw new KeyNotFoundException(key.ToString());
            }

            set { Insert(key, value, false); }
        }

        public SerializableDictionary()
            : this(0, null)
        {
        }

        public SerializableDictionary(int capacity)
            : this(capacity, null)
        {
        }

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        {
        }

        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity");

            Initialize(capacity);

            _Comparer = (comparer ?? EqualityComparer<TKey>.Default);
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null)
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : this((dictionary != null) ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            foreach (KeyValuePair<TKey, TValue> current in dictionary)
                Add(current.Key, current.Value);
        }

        public bool ContainsValue(TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < _Count; i++)
                {
                    if (_HashCodes[i] >= 0 && _Values[i] == null)
                        return true;
                }
            }
            else
            {
                var defaultComparer = EqualityComparer<TValue>.Default;
                for (int i = 0; i < _Count; i++)
                {
                    if (_HashCodes[i] >= 0 && defaultComparer.Equals(_Values[i], value))
                        return true;
                }
            }
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return FindIndex(key) >= 0;
        }

        public void Clear()
        {
            if (_Count <= 0)
                return;

            for (int i = 0; i < _Buckets.Length; i++)
                _Buckets[i] = -1;

            Array.Clear(_Keys, 0, _Count);
            Array.Clear(_Values, 0, _Count);
            Array.Clear(_HashCodes, 0, _Count);
            Array.Clear(_Next, 0, _Count);

            _FreeList = -1;
            _Count = 0;
            _FreeCount = 0;
            _Version++;
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            int[] bucketsCopy = new int[newSize];
            for (int i = 0; i < bucketsCopy.Length; i++)
                bucketsCopy[i] = -1;

            var keysCopy = new TKey[newSize];
            var valuesCopy = new TValue[newSize];
            var hashCodesCopy = new int[newSize];
            var nextCopy = new int[newSize];

            Array.Copy(_Values, 0, valuesCopy, 0, _Count);
            Array.Copy(_Keys, 0, keysCopy, 0, _Count);
            Array.Copy(_HashCodes, 0, hashCodesCopy, 0, _Count);
            Array.Copy(_Next, 0, nextCopy, 0, _Count);

            if (forceNewHashCodes)
            {
                for (int i = 0; i < _Count; i++)
                {
                    if (hashCodesCopy[i] != -1)
                        hashCodesCopy[i] = (_Comparer.GetHashCode(keysCopy[i]) & 2147483647);
                }
            }

            for (int i = 0; i < _Count; i++)
            {
                int index = hashCodesCopy[i] % newSize;
                nextCopy[i] = bucketsCopy[index];
                bucketsCopy[index] = i;
            }

            _Buckets = bucketsCopy;
            _Keys = keysCopy;
            _Values = valuesCopy;
            _HashCodes = hashCodesCopy;
            _Next = nextCopy;
        }

        private void Resize()
        {
            Resize(PrimeHelper.ExpandPrime(_Count), false);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            int hash = _Comparer.GetHashCode(key) & 2147483647;
            int index = hash % _Buckets.Length;
            int num = -1;
            for (int i = _Buckets[index]; i >= 0; i = _Next[i])
            {
                if (_HashCodes[i] == hash && _Comparer.Equals(_Keys[i], key))
                {
                    if (num < 0)
                        _Buckets[index] = _Next[i];
                    else
                        _Next[num] = _Next[i];

                    _HashCodes[i] = -1;
                    _Next[i] = _FreeList;
                    _Keys[i] = default(TKey);
                    _Values[i] = default(TValue);
                    _FreeList = i;
                    _FreeCount++;
                    _Version++;
                    return true;
                }
                num = i;
            }
            return false;
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (_Buckets == null)
                Initialize(0);

            int hash = _Comparer.GetHashCode(key) & 2147483647;
            int index = hash % _Buckets.Length;
            int num1 = 0;
            for (int i = _Buckets[index]; i >= 0; i = _Next[i])
            {
                if (_HashCodes[i] == hash && _Comparer.Equals(_Keys[i], key))
                {
                    if (add)
                        throw new ArgumentException("Key already exists: " + key);

                    _Values[i] = value;
                    _Version++;
                    return;
                }
                num1++;
            }
            int num2;
            if (_FreeCount > 0)
            {
                num2 = _FreeList;
                _FreeList = _Next[num2];
                _FreeCount--;
            }
            else
            {
                if (_Count == _Keys.Length)
                {
                    Resize();
                    index = hash % _Buckets.Length;
                }
                num2 = _Count;
                _Count++;
            }
            _HashCodes[num2] = hash;
            _Next[num2] = _Buckets[index];
            _Keys[num2] = key;
            _Values[num2] = value;
            _Buckets[index] = num2;
            _Version++;

            //if (num3 > 100 && HashHelpers.IsWellKnownEqualityComparer(comparer))
            //{
            //    comparer = (IEqualityComparer<TKey>)HashHelpers.GetRandomizedEqualityComparer(comparer);
            //    Resize(entries.Length, true);
            //}
        }

        private void Initialize(int capacity)
        {
            int prime = PrimeHelper.GetPrime(capacity);

            _Buckets = new int[prime];
            for (int i = 0; i < _Buckets.Length; i++)
                _Buckets[i] = -1;

            _Keys = new TKey[prime];
            _Values = new TValue[prime];
            _HashCodes = new int[prime];
            _Next = new int[prime];

            _FreeList = -1;
        }

        private int FindIndex(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (_Buckets != null)
            {
                int hash = _Comparer.GetHashCode(key) & 2147483647;
                for (int i = _Buckets[hash % _Buckets.Length]; i >= 0; i = _Next[i])
                {
                    if (_HashCodes[i] == hash && _Comparer.Equals(_Keys[i], key))
                        return i;
                }
            }
            return -1;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = FindIndex(key);
            if (index >= 0)
            {
                value = _Values[index];
                return true;
            }
            value = default(TValue);
            return false;
        }

        private static class PrimeHelper
        {
            public static readonly int[] Primes = new int[]
            {
            3,
            7,
            11,
            17,
            23,
            29,
            37,
            47,
            59,
            71,
            89,
            107,
            131,
            163,
            197,
            239,
            293,
            353,
            431,
            521,
            631,
            761,
            919,
            1103,
            1327,
            1597,
            1931,
            2333,
            2801,
            3371,
            4049,
            4861,
            5839,
            7013,
            8419,
            10103,
            12143,
            14591,
            17519,
            21023,
            25229,
            30293,
            36353,
            43627,
            52361,
            62851,
            75431,
            90523,
            108631,
            130363,
            156437,
            187751,
            225307,
            270371,
            324449,
            389357,
            467237,
            560689,
            672827,
            807403,
            968897,
            1162687,
            1395263,
            1674319,
            2009191,
            2411033,
            2893249,
            3471899,
            4166287,
            4999559,
            5999471,
            7199369
            };

            public static bool IsPrime(int candidate)
            {
                if ((candidate & 1) != 0)
                {
                    int num = (int)Math.Sqrt((double)candidate);
                    for (int i = 3; i <= num; i += 2)
                    {
                        if (candidate % i == 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return candidate == 2;
            }

            public static int GetPrime(int min)
            {
                if (min < 0)
                    throw new ArgumentException("min < 0");

                for (int i = 0; i < PrimeHelper.Primes.Length; i++)
                {
                    int prime = PrimeHelper.Primes[i];
                    if (prime >= min)
                        return prime;
                }
                for (int i = min | 1; i < 2147483647; i += 2)
                {
                    if (PrimeHelper.IsPrime(i) && (i - 1) % 101 != 0)
                        return i;
                }
                return min;
            }

            public static int ExpandPrime(int oldSize)
            {
                int num = 2 * oldSize;
                if (num > 2146435069 && 2146435069 > oldSize)
                {
                    return 2146435069;
                }
                return PrimeHelper.GetPrime(num);
            }
        }

        public ICollection<TKey> Keys
        {
            get { return _Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return _Values; }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            int index = FindIndex(item.Key);
            return index >= 0 &&
                EqualityComparer<TValue>.Default.Equals(_Values[index], item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException(string.Format("index = {0} array.Length = {1}", index, array.Length));

            if (array.Length - index < Count)
                throw new ArgumentException(string.Format("The number of elements in the dictionary ({0}) is greater than the available space from index to the end of the destination array {1}.", Count, array.Length));

            for (int i = 0; i < _Count; i++)
            {
                if (_HashCodes[i] >= 0)
                    array[index++] = new KeyValuePair<TKey, TValue>(_Keys[i], _Values[i]);
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly SerializableDictionary<TKey, TValue> _Dictionary;
            private int _Version;
            private int _Index;
            private KeyValuePair<TKey, TValue> _Current;

            public KeyValuePair<TKey, TValue> Current
            {
                get { return _Current; }
            }

            internal Enumerator(SerializableDictionary<TKey, TValue> dictionary)
            {
                _Dictionary = dictionary;
                _Version = dictionary._Version;
                _Current = default(KeyValuePair<TKey, TValue>);
                _Index = 0;
            }

            public bool MoveNext()
            {
                if (_Version != _Dictionary._Version)
                    throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}", _Version, _Dictionary._Version));

                while (_Index < _Dictionary._Count)
                {
                    if (_Dictionary._HashCodes[_Index] >= 0)
                    {
                        _Current = new KeyValuePair<TKey, TValue>(_Dictionary._Keys[_Index], _Dictionary._Values[_Index]);
                        _Index++;
                        return true;
                    }
                    _Index++;
                }

                _Index = _Dictionary._Count + 1;
                _Current = default(KeyValuePair<TKey, TValue>);
                return false;
            }

            void IEnumerator.Reset()
            {
                if (_Version != _Dictionary._Version)
                    throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}", _Version, _Dictionary._Version));

                _Index = 0;
                _Current = default(KeyValuePair<TKey, TValue>);
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
            }
        }
    }
}