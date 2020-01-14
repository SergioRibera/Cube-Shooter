using System;
using System.Collections.Generic;

//Clase que funciona como un diccionario de string con metodos adaptados
[Serializable]
public class StringMatrix
{
    public List<string> Keys;
    public List<string> Values;

    //Constructores
    public StringMatrix()
    {
        if (Keys == null)
            Keys = new List<string>();
        if (Values == null)
            Values = new List<string>();
    }
    public StringMatrix(Dictionary<string, string> content)
    {
        if (Keys == null)
            Keys = new List<string>();
        if (Values == null)
            Values = new List<string>();
        foreach (var c in content)
        {
            Keys.Add(c.Key);
            Values.Add(c.Value);
        }
    }
    public StringMatrix(List<string> keys, List<string> values)
    {
        if (Keys == null)
            Keys = new List<string>();
        if (Values == null)
            Values = new List<string>();
        foreach (var k in keys)
        {
            Keys.Add(k);
        }
        foreach (var v in values)
        {
            Values.Add(v);
        }
    }

    //verificamos que tenga la misma cantidad de keys como de values, caso contrario devolvemos un error
    public int Count { get { return (Keys.Count == Values.Count) ? Keys.Count : throw new Exception("El Tamaño De Las Listas No Son Iguales"); } }
    //Metodo que limpia las listas
    public void Clear()
    {
        Keys.Clear();
        Values.Clear();
    }
    //Metodos que remueven Elementos dependiendo del index o de la Key
    public void Remove(int index)
    {
        Keys.Remove(Keys[index]);
        Values.Remove(Values[index]);
    }
    public void Remove(string key)
    {
        int i = Keys.IndexOf(key);
        Keys.Remove(Keys[i]);
        Values.Remove(Values[i]);
    }
    //Metodos que permiten añadir nuevos elementos de manera independiente o conjunta
    public void Add(string key, string content)
    {
        Keys.Add(key);
        Values.Add(content);
    }
    public void Add(Dictionary<string, string> content)
    {
        foreach (var c in content)
        {
            Keys.Add(c.Key);
            Values.Add(c.Value);
        }

    }
    //Obtenemos la llave en base al value
    public string GetKey(string content)
    {
        int index = Values.IndexOf(content);
        return Keys[index];
    }
    //Obtenemos la llave en base al index del value
    public string GetKey(int content)
    {
        return Keys[content];
    }
    //Obtenemos el value en base a la llave
    public string GetValue(string key)
    {
        int index = Keys.IndexOf(key);
        return Values[index];
    }
    //Obtenemos el value en base al index del key
    public string GetValue(int key)
    {
        return Values[key];
    }
    //Metodo que hace un recorrido por las listas y en cada recorrido llama a la funcion delegada que recibe
    public void Foreach(Action<string, string> a)
    {
        for (int i = 0; i < Count; i++)
        {
            a(Keys[i], Values[i]);
        }
    }
    //Remplazamos el metodo por defecto de ToString por este personalizado en donde mostraremos los datos a nuestro gusto o necesidades
    public override string ToString()
    {
        string s = "";

        for (int i = 0; i < Count; i++)
        {
            s += GetKey(i) + " : " + GetValue(i) + "\n";
        }
        return s;
    }
    //Simulamos el uso de Corchetes para obtener o editar un valor en base a la llave
    public string this[string Key]
    {
        get
        {
            return Values[Keys.IndexOf(Key)];
        }
        set
        {
            if (Keys.IndexOf(Key) < 0)
                Keys.Add(value);
            else
                Keys[Keys.IndexOf(Key)] = value;

            if (Values.IndexOf(Key) < 0)
                Values.Add(value);
            else
                Values[Values.IndexOf(Key)] = value;
        }
    }
    //Obtenemos el valor o la llave en base al index
    public string Get(int K, bool key = false)
    {
        if (key)
        {
            return Keys[K];
        }
        return Values[K];
    }
    //Editamos o añadimos un valor o llave 
    public void Set(int K, string value, bool key = false)
    {
        if (key)
        {
            if (Keys.Count < K)
                Keys.Add(value);
            else
                Keys[K] = value;
        }
        else
        {
            if (Values.Count < K)
                Values.Add(value);
            else
                Values[K] = value;
        }
    }
    //Convertimos de manera implicita un Dictionary en un StringMatrix
    public static implicit operator StringMatrix(Dictionary<string, string> s)
    {
        return new StringMatrix(s);
    }
    //Convertimos de manera implicita un StringMatrix en un Disctionary
    public static implicit operator Dictionary<string, string>(StringMatrix sm)
    {
        Dictionary<string, string> s = new Dictionary<string, string>();
        for (int i = 0; i < sm.Count; i++)
            s.Add(sm.GetKey(i), sm.GetValue(i));
        return s;
    }
    //Convetimos de manera implicita un StringMatrix en un String
    public static implicit operator string(StringMatrix sm)
    {
        return sm.ToString();
    }
}
//Clase que funciona como un diccionario de string con metodos adaptados
[Serializable]
public class MyDictionary<K, V>
{
    public List<K> Keys;
    public List<V> Values;

    //Constructores
    public MyDictionary()
    {
        if (Keys == null)
            Keys = new List<K>();
        if (Values == null)
            Values = new List<V>();
    }
    public MyDictionary(Dictionary<K, V> content)
    {
        if (Keys == null)
            Keys = new List<K>();
        if (Values == null)
            Values = new List<V>();
        foreach (var c in content)
        {
            Keys.Add(c.Key);
            Values.Add(c.Value);
        }
    }
    public MyDictionary(List<K> keys, List<V> values)
    {
        if (Keys == null)
            Keys = new List<K>();
        if (Values == null)
            Values = new List<V>();
        foreach (var k in keys)
        {
            Keys.Add(k);
        }
        foreach (var v in values)
        {
            Values.Add(v);
        }
    }

    //verificamos que tenga la misma cantidad de keys como de values, caso contrario devolvemos un error
    public int Count { get { return (Keys.Count == Values.Count) ? Keys.Count : throw new Exception("El Tamaño De Las Listas No Son Iguales"); } }
    //Metodo que limpia las listas
    public void Clear()
    {
        Keys.Clear();
        Values.Clear();
    }
    //Metodos que remueven Elementos dependiendo del index o de la Key
    public void Remove(int index)
    {
        Keys.Remove(Keys[index]);
        Values.Remove(Values[index]);
    }
    public void Remove(K key)
    {
        int i = Keys.IndexOf(key);
        Keys.Remove(Keys[i]);
        Values.Remove(Values[i]);
    }
    //Metodos que permiten añadir nuevos elementos de manera independiente o conjunta
    public void Add(K key, V content)
    {
        Keys.Add(key);
        Values.Add(content);
    }
    public void Add(Dictionary<K, V> content)
    {
        foreach (var c in content)
        {
            Keys.Add(c.Key);
            Values.Add(c.Value);
        }

    }
    //Obtenemos la llave en base al value
    public K GetKey(V content)
    {
        int index = Values.IndexOf(content);
        return Keys[index];
    }
    //Obtenemos la llave en base al index del value
    public K GetKey(int content)
    {
        return Keys[content];
    }
    //Obtenemos el value en base a la llave
    public V GetValue(K key)
    {
        int index = Keys.IndexOf(key);
        return Values[index];
    }
    //Obtenemos el value en base al index del key
    public V GetValue(int key)
    {
        return Values[key];
    }
    //Metodo que hace un recorrido por las listas y en cada recorrido llama a la funcion delegada que recibe
    public void Foreach(Action<K, V> a)
    {
        for (int i = 0; i < Count; i++)
        {
            a(Keys[i], Values[i]);
        }
    }
    //Remplazamos el metodo por defecto de ToString por este personalizado en donde mostraremos los datos a nuestro gusto o necesidades
    public override string ToString()
    {
        string s = "";

        for (int i = 0; i < Count; i++)
        {
            s += GetKey(i) + " : " + GetValue(i) + "\n";
        }
        return s;
    }
    //Simulamos el uso de Corchetes para obtener o editar un valor en base a la llave
    public V this[K Key]
    {
        get
        {
            return Values[Keys.IndexOf(Key)];
        }
        set
        {
            Values.Add(value);
        }
    }
    //Obtenemos el valor o la llave en base al index
    public V Get(int K)
    {
        return Values[K];
    }
    //Editamos o añadimos un valor o llave 
    public void Set(int K, V value)
    {
        if (Values.Count < K)
            Values.Add(value);
        else
            Values[K] = value;
    }
    public void Set(int K, K value)
    {
        if (Keys.Count < K)
            Keys.Add(value);
        else
            Keys[K] = value;
    }
    //Convertimos de manera implicita un Dictionary en un StringMatrix
    public static implicit operator MyDictionary<K, V>(Dictionary<K, V> s)
    {
        return new MyDictionary<K, V>(s);
    }
    //Convertimos de manera implicita un StringMatrix en un Disctionary
    public static implicit operator Dictionary<K, V>(MyDictionary<K, V> sm)
    {
        Dictionary<K, V> s = new Dictionary<K, V>();
        for (int i = 0; i < sm.Count; i++)
            s.Add(sm.GetKey(i), sm.GetValue(i));
        return s;
    }
    //Convetimos de manera implicita un StringMatrix en un String
    public static implicit operator string(MyDictionary<K, V> sm)
    {
        return sm.ToString();
    }
}