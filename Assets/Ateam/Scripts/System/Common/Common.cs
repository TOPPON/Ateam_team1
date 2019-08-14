using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ateam
{
    public static class Common 
    {
        public enum MOVE_TYPE
        {
            UP = 0,
            DOWN,
            LEFT,
            RIGHT,
            NONE_MAX,
        }

        //---------------------------------------------------
        // CreateHashTable
        //---------------------------------------------------
        public static Hashtable CreateHashTable(params object[] data)
        {
            Hashtable table = new Hashtable();

            for (int i = 0; i < data.Length; i +=2)
            {
                table.Add((string)data[i], data[i + 1]);
            }

            return table;
        }


        public delegate void dataCallback(System.Object obj);

        //---------------------------------------------------
        // LoadAsync
        //---------------------------------------------------
        public static IEnumerator LoadAsync(string path, dataCallback callback)
        {
            ResourceRequest req = Resources.LoadAsync(path);

            while (!req.isDone)
            {
                yield return null;
            }

            if (req.asset == null)
            {
                Debug.LogError("Data None : " + path.ToString());
            }
            else
            {
                callback(req.asset);
            }
        }

        //---------------------------------------------------
        // ExistsVector2List
        //---------------------------------------------------
        public static bool ExistsVector2List(List<Vector2> src, Vector2 dst)
        {
            foreach (Vector2 i in src)
            {
                if ((int)i.x == (int)dst.x && (int)i.y == (int)dst.y)
                {
                    return true;
                }
            }

            return false;
        }

        //---------------------------------------------------
        // DegToRad
        //---------------------------------------------------
        public static float DegToRad(float deg)
        {
            return (float)(deg * Math.PI / 180);
        }

        //---------------------------------------------------
        // CheckHash
        //---------------------------------------------------
        public static bool CheckHash(byte[] src, byte[] dst)
        {
            bool ret = false;

            if (src.Length == dst.Length)
            {
                int i = 0;

                while ((i < dst.Length) && (dst[i] == src[i]))
                {
                    i++;
                }

                if (i == src.Length)
                {
                    ret = true;
                }
            }

            return ret;
        }

    }
        
}
