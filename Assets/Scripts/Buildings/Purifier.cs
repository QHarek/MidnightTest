using System.Collections;
using UnityEngine;

public class Purifier : Machine
{
    [SerializeField] private GameObject _effects;

    public override void Upgrade()
    {
        base.Upgrade();
        _processTime /= 2;
    }

    protected override void StopProcessing()
    {
        base.StopProcessing();
        _effects.SetActive(false);
        StopCoroutine(PurifyDust());
        StartCoroutine(BeginDestroying());
    }

    private IEnumerator BeginDestroying()
    {
        yield return new WaitForSeconds(15);
        if (_processedLoot.Count == 0 && _rawLoot.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    internal override void ProcessLoot(bool isLoaded = false)
    {
        _effects.SetActive(true);
        if (!isLoaded)
        {
            _processedLoot.Clear();
            foreach (var loot in _rawLoot)
            {
                switch (loot.Key)
                {
                    case "DustCommon":
                        _processedLoot.Add("DustUncommon", loot.Value);
                        break;
                    case "DustUncommon":
                        _processedLoot.Add("DustRare", loot.Value);
                        break;
                    case "DustRare":
                        _processedLoot.Add("DustEpic", loot.Value);
                        break;
                }
            }
            _rawLoot.Clear();
        }
        StartCoroutine(PurifyDust());
    }

    private IEnumerator PurifyDust()
    {
        while (_processingProgress < 100 && _isAvailable)
        {
            _processingProgress++;
            if (_processingProgress == 100)
            {
                _effects.SetActive(false);
            }
            _updateUI.UpdateProgress(_processingProgress);
            yield return new WaitForSeconds(_processTime / 100f);
        }
    }
}