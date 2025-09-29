using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// This class is a generic factory of MonoBehaviours that includes the usage of a pool.
/// </summary>
/// <typeparam name="T">Type used by the factory, derived from a MonoBehaviour.</typeparam>
public class Factory<T> where T : MonoBehaviour
{
    private readonly T _prefab;
    private readonly ObjectPool<T> _pool;

    public Factory(T prefab)
    {
        _prefab = prefab;
        _pool = new ObjectPool<T>(Create, OnGet, OnRelease, OnDestroy);
    }

    public T Get()
    {
        return _pool.Get();
    }

    public void Release(T instance)
    {
        _pool.Release(instance);
    }

    private T Create()
    {
        return Object.Instantiate(_prefab);
    }

    private void OnGet(T instance)
    {
        instance.gameObject.SetActive(true);
    }
    
    private void OnRelease(T instance)
    {
        instance.gameObject.SetActive(false);
    }

    private void OnDestroy(T instance)
    {
        Object.Destroy(instance.gameObject);
    }

}
