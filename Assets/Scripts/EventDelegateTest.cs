using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDelegateTest : MonoBehaviour
{
    // A delegate is a placeholder to any function with the same signature.
    public delegate void TestDelegate(int ivar);
    public static event TestDelegate OnTestDelegate;

    private void Awake()
    {
        //OnTestDelegate += Test1;  // Shorthand for call Test1 whenever event is invoked. // Subscribe to event listener.
        //OnTestDelegate += Test2;
        //OnTestDelegate += Test3; 
    }
    // Start is called before the first frame update
    void Start()
    {
        OnTestDelegate.Invoke(5); 
    }


    public void Test1(int ivar1)
    {
        Debug.Log("Test1 : " + ivar1);

    }

    public void Test2(int ivar1)
    {
        Debug.Log("Test2 : " + ivar1);


    }

    public void Test3(int ivar1)
    {

        Debug.Log("Test3 : " + ivar1);

    }

    public void Test4(int ivar1)
    {
        Debug.Log("Test4 : " + ivar1);


    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
