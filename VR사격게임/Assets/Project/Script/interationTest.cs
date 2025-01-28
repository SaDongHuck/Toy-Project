using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interationTest : MonoBehaviour
{
    public void print(string x) => print(x as object);
    public void OnSelect()
    {
        print("동작함");
    }
}
