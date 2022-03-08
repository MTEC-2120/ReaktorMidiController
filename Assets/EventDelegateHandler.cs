using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDelegateHandler : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {




        EventDelegateTest.OnTestDelegate += Test1;  
        EventDelegateTest.OnTestDelegate += Test2;
        EventDelegateTest.OnTestDelegate += Test3;




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
}
