using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    Stack<string> _sceneStack = new Stack<string>();

    Stack<object> _objectStack = new Stack<object>();

    public int StackCount()
    {
        return _objectStack.Count;
    }

    public object PeekStack()
    {
        return _objectStack.Peek();
    }

    public object PopFromStack()
    {
        return _objectStack.Pop();
    }

    public void PushToStack(object o)
    {
        _objectStack.Push(o);
    }

    public void LoadScene(string name, params object[] objects)
    {
        _sceneStack.Push(SceneManager.GetActiveScene().name);
        foreach (object o in objects)
            _objectStack.Push(o);

        SceneManager.LoadScene(name);
    }

    public void LoadScene(string name, bool OnStack, params object[] objects)
    {
        if (OnStack)
            _sceneStack.Push(SceneManager.GetActiveScene().name);
        foreach (object o in objects)
            _objectStack.Push(o);

        SceneManager.LoadScene(name);
    }

    public bool OneSceneBack()
    {
        if(_sceneStack.Count > 0)
        {
            SceneManager.LoadScene(_sceneStack.Pop());
            return true;
        }
        else
        {
            Debug.Log("No Scenes to pop");
            return false;
        }
    }

    public void ResetStack()
    {
        _sceneStack = new Stack<string>();
        _objectStack = new Stack<object>();
    }
}
