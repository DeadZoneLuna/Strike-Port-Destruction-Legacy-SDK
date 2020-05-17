using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using Object = UnityEngine.Object;
public static class Ext
{
    public static GameObject CreateSubModel(this Transform source, SkinnedMeshRenderer skinnedMesh, string name = null)
    {
        GameObject Temp = new GameObject(string.IsNullOrEmpty(name) ? skinnedMesh.gameObject.name : name);
        Temp.transform.parent = source;
        Temp.SetLayer(source.gameObject.layer);

        SkinnedMeshRenderer TempRenderer = Temp.AddComponent<SkinnedMeshRenderer>();

        Transform[] ObjectBones = new Transform[skinnedMesh.bones.Length];

        for (int i = 0; i < skinnedMesh.bones.Length; i++)
        {
            ObjectBones[i] = FindChildByName(skinnedMesh.bones[i].name, source);
        }

        TempRenderer.bones = ObjectBones;
        TempRenderer.sharedMesh = skinnedMesh.sharedMesh;
        TempRenderer.sharedMaterials = skinnedMesh.sharedMaterials;
        TempRenderer.updateWhenOffscreen = true;
        return TempRenderer.gameObject;
    }

    private static Transform FindChildByName(string Name, Transform GO)
    {
        Transform ReturnObj;

        if (GO.name == Name)
            return GO.transform;

        foreach (Transform child in GO)
        {
            ReturnObj = FindChildByName(Name, child);

            if (ReturnObj != null)
                return ReturnObj;
        }

        return null;
    }

    /// <summary>
    /// Reset animation to default state
    /// </summary>
    /// <param name="animation"></param>
    /// <returns></returns>
    public static void ResetAnimationState(this Animation animation)
    {
        animation.Rewind();
        foreach (AnimationState state in animation)
        {
            if (state.wrapMode == WrapMode.Default || state.wrapMode == WrapMode.Once)
            {
                state.time = 0;
                animation.Play(state.name);
                break;
            }
        }

        animation.Sample();
        animation.Stop();
    }

    public static void Print(this object o)
    {
        Debug.Log(o);
    }

    public static Transform FirstOrDefault(this Transform transform, Func<Transform, bool> query)
    {
        if (query(transform))
        {
            return transform;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            var result = FirstOrDefault(transform.GetChild(i), query);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public static Transform FindInChild(this Transform t,params string[] name)
    {
        return t.GetTransforms().FirstOrDefault(a => name.Contains(a.name));
    }

    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        foreach (Transform child in aParent)
        {
            if (child.name == aName)
                return child;
            var result = child.FindDeepChild(aName);
            if (result != null)
                return result;
        }
        return null;
    }


    public static T2 TryGet<T, T2>(this Dictionary<T, T2> dict, T key)
    {
        T2 t;
        dict.TryGetValue(key, out t);
        return t;
    }

    public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
    {
        ICollection<TSource> is2 = source as ICollection<TSource>;
        if (is2 != null)
        {
            return is2.Contains(value);
        }
        return source.Contains(value, null);
    }

    public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
    {
        if (comparer == null)
        {
            comparer = EqualityComparer<TSource>.Default;
        }
        if (source == null)
        {
            throw new ArgumentException("source");
        }
        foreach (TSource local in source)
        {
            if (comparer.Equals(local, value))
            {
                return true;
            }
        }
        return false;
    }

    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
    {
        if (source == null)
        {
            throw new Exception("source is null");
        }
        IList<TSource> list = source as IList<TSource>;
        if (list != null)
        {
            if (list.Count > 0)
            {
                return list[0];
            }
        }
        else
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }
        }
        return default(TSource);
    }

    public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        if (source == null)
        {
            throw new ArgumentException("source");
        }
        return source.CreateOrderedEnumerable<TKey>(keySelector, null, false);
    }

    public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        if (source == null)
        {
            throw new ArgumentException("source");
        }        
        return source.CreateOrderedEnumerable<TKey>(keySelector, null, true);
    }
    public static float Max<T>(this IEnumerable<T> source, Func<T, float> selector)
    {
        float max = float.MinValue;
        foreach (var a in source)
        {
            var f = selector(a);
            if (max < f)
                max = f;
        }
        return max;
    }
    public static float Min<T>(this IEnumerable<T> source, Func<T, float> selector)
    {
        float min = float.MaxValue;
        foreach (var a in source)
        {
            var f = selector(a);
            if (min > f)
                min = f;
        }
        return min;
    }

    public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (var s in source)
            if (predicate(s)) return true;
        return false;
    }

    public static bool Any<TSource>(this IEnumerable<TSource> source)
    {
        //#if UNITY_IPHONE
        if (source.GetEnumerator().MoveNext())
            return true;
        return false;
        //foreach (var s in source)
        //return true;
        //return false;
    }

    public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (var s in source)
            if (!predicate(s)) return false;
        return true;
    }

    public static T Next<T>(this IEnumerable<T> ts, T t) 
    {        
        T o = default(T);
        bool set = false;
        foreach (var a in ts)
        {
            if (set && t.Equals(o)) return a;
            o = a;
            set = true;
        }
        return ts.First();
    }

    public static T Prev<T>(this IEnumerable<T> tt, T t) where T : class
    {
        T o = null;
        foreach (var a in tt)
        {
            if (o != null && a == t) return o;
            o = a;
        }
        return o;
    }
    public static void SetActive(this Component th, bool to)
    {
        th.gameObject.SetActive(to);
        //foreach (Transform a in th.transform.GetTransforms())
            //a.gameObject.activeInHierarchy = to;
    }
    public static void SetLayer(this Component th, int to)
    {
        foreach (Transform a in th.transform.GetTransforms())
            a.gameObject.layer = to;
    }
    public static  void SetLayer(this Component th, int from, int to)
    {
        foreach (Transform a in th.transform.GetTransforms())
            if (a.gameObject.layer == @from)
                a.gameObject.layer = to;
    }
    public static string[] Split2(this string s)
    {
        return s.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
    }
    //public static void SetValue<T, T2>(this List<KeyValuePair<T, T2>> d, T key, T2 value) where T : class
    //{
    //    for (int i = 0; i < d.Count; i++)
    //    {
    //        if (d[i].key == key)
    //        {
    //            d[i] = new KeyValuePair<T, T2>(key, value);
    //            return;
    //        }
    //    }
    //    d.Add(new KeyValuePair<T, T2>(key, value));
    //}
    //public static bool toBool(this int v)
    //{
    //    return v != 0;
    //}
    //public static int toInt(this bool v)
    //{
    //    return v ? 1 : 0;
    //}
    public static T GetValue<T>(this Component c, string str)
    {
        return (T)c.GetType().GetField(str).GetValue(c);
    }
    public static void SetValue<T>(this Component c, string str, T value)
    {
        c.GetType().GetField(str).SetValue(c, value);
    }
    public static string[] Split(this string s, string d)
    {
        return s.Split(new string[] { d }, StringSplitOptions.None);
    }
    public static string[] Split2(this string s, string d)
    {
        return s.Split(new string[] { d }, StringSplitOptions.RemoveEmptyEntries);
    }
    public static string[][] Split3(this string s, string d, string d2)
    {
        var ss = s.Split(new string[] { d }, StringSplitOptions.RemoveEmptyEntries);
        string[][] asd = new string[ss.Length][];
        for (int i = 0; i < ss.Length; i++)
        {
            asd[i] = ss[i].Split(d2);
        }
        return asd;
    }
	public static string CalculateMD5(string input)
	{
		byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(input);
		byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
		string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
		return hashedString;
	}
    public static string CalculateMD5Hash(string input)
    {
        MD5 md5 = MD5.Create();
        input += input;
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
            sb.Append(hash[i].ToString("X2"));
        return sb.ToString().ToLower();
    }
    public static void SetLayer(this Transform g, int l)
    {
        foreach (var t in g.GetTransforms())
            t.gameObject.layer = l;
    }
    //public static void SetActive(this Transform g, bool value)
    //{
    //    foreach (var t in g.GetTransforms())
    //        t.gameObject.activeInHierarchy = value;
    //}

    public static int SelectIndex<T>(this IEnumerable<T> strs, T t) where T : class
    {
        int i = 0;
        foreach (var a in strs)
        {
            if (a == t)
                return i;
            i++;
        }
        return -1;
    }
    public static IEnumerable<Transform> GetTransforms(this Transform ts)
    {
        yield return ts;
        foreach (Transform t in ts)
        {
            foreach (var t2 in GetTransforms(t))
                yield return t2;
        }
    }

    public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> coll, int N)
    {
        return coll.Reverse().Take(N).Reverse();
    }

    public static T Random<T>(this IEnumerable<T> source)
    {        
        return source.Skip(UnityEngine.Random.Range(0, source.Count())).FirstOrDefault();
    }

    public static T AddOrGet<T>(this GameObject g) where T : Component
    {
        var c = g.GetComponent<T>();
        if (c == null) return g.AddComponent<T>();
        else
            return c;
    }
    public static void SetLayer(this GameObject g, int l)
    {
        foreach (var t in g.transform.GetTransforms())
            t.gameObject.layer = l;
    }

    public static T Parse<T>(this string s)
    {
        return (T)Enum.Parse(typeof(T), s);
    }
    public static IEnumerable<Transform> Parent(this Transform t)
    {
        while (t != null)
        {
            yield return t;
            t = t.parent;
        }
    }
    public static T GetComponentInParrent<T>(this Transform t) where T : Component
    {
        for (int i = 0; ; i++)
        {
            if (t == null || i > 4) return null;
            var c = t.GetComponent<T>();
            if (c != null) return c;
            t = t.parent;
        }
    }
    public static MonoBehaviour GetMonoBehaviorInParrent(this Transform t)
    {
        for (int i = 0; ; i++)
        {
            if (t == null || i > 4) return null;
            var c = t.GetComponent<MonoBehaviour>();
            if (c != null) return c;
            t = t.parent;
        }
    }
    //public static IEnumerable<T> ShuffleIterator<T>(
    //   this IEnumerable<T> source, Random rng)
    //{
    //    T[] buffer = source.ToArray();
    //    for (int n = 0; n < buffer.Length; n++)
    //    {
    //        int k = rng.Next(n, buffer.Length);
    //        yield return buffer[k];
    //        buffer[k] = buffer[n];
    //    }
    //}
    public static int Max(this IEnumerable<int> source)
    {
        int num = 0;
        bool flag = false;
        foreach (int num2 in source)
        {
            if (flag)
            {
                if (num2 > num)
                    num = num2;
            }
            else
            {
                num = num2;
                flag = true;
            }
        }
        if (!flag)
            Debug.LogWarning("no elements");
        return num;
    }
}
[XmlRoot("dictionary")]
public class XmlSerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
    public System.Xml.Schema.XmlSchema GetSchema()
    {
        return null;
    }
    public void ReadXml(System.Xml.XmlReader reader)
    {
        XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
        XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
        bool wasEmpty = reader.IsEmptyElement;
        reader.Read();
        if (wasEmpty)
            return;
        while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
        {
            reader.ReadStartElement("item");
            reader.ReadStartElement("key");
            TKey key = (TKey)keySerializer.Deserialize(reader);
            reader.ReadEndElement();
            reader.ReadStartElement("value");
            TValue value = (TValue)valueSerializer.Deserialize(reader);
            reader.ReadEndElement();
            this.Add(key, value);
            reader.ReadEndElement();
            reader.MoveToContent();
        }
        reader.ReadEndElement();
    }
    public void WriteXml(System.Xml.XmlWriter writer)
    {
        XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
        XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
        foreach (TKey key in this.Keys)
        {
            writer.WriteStartElement("item");
            writer.WriteStartElement("key");
            keySerializer.Serialize(writer, key);
            writer.WriteEndElement();
            writer.WriteStartElement("value");
            TValue value = this[key];
            valueSerializer.Serialize(writer, value);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
