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

    public void LoadScene(string name, bool OnStack, LoadSceneMode loadSceneMode, params object[] objects)
    {
        if (OnStack)
        {
            _sceneStack.Push(SceneManager.GetActiveScene().name);
        }
        foreach (object o in objects)
            _objectStack.Push(o);
        if(loadSceneMode == LoadSceneMode.Additive)
            _sceneStack.Push(name);

        SceneManager.LoadScene(name, loadSceneMode);
    }

    public bool OneSceneBack()
    {
        if(SceneManager.sceneCount >= 2)
        {
            string addativeScene = _sceneStack.Pop();
            string originalScene = _sceneStack.Pop(); //not used
            SceneManager.UnloadSceneAsync(addativeScene);
            return true;
        }
        else if(_sceneStack.Count > 0)
        {
            SceneManager.LoadScene(_sceneStack.Pop());
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetStack()
    {
        _sceneStack = new Stack<string>();
        _objectStack = new Stack<object>();
    }
}
