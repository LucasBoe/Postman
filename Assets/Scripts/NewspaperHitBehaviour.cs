using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NewspaperHitBehaviour : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.TryGetComponent<Newspaper>(out Newspaper np))
        {
            OnNewspaperHit(np, collision);
        }
    }

    protected abstract void OnNewspaperHit(Newspaper newspaper, Collision collision);

}
