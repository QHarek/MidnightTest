using System.Collections;
using UnityEngine;

public class Crusher : Machine
{
    [SerializeField] private Animator _animator;

    public override void Upgrade()
    {
        base.Upgrade();
        _processTime /= 2;
    }

    protected override void StopProcessing()
    {
        base.StopProcessing();
        _animator.enabled = false;
        StopCoroutine(CrushCrystals());
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
        _animator.enabled = true;
        if (!isLoaded)
        {
            _processedLoot.Clear();
            foreach (var loot in _rawLoot)
            {
                switch (loot.Key)
                {
                    case "CrystalCommon":
                        _processedLoot.Add("DustCommon", loot.Value * 2);
                        break;
                    case "CrystalUncommon":
                        _processedLoot.Add("DustUncommon", loot.Value * 2);
                        break;
                    case "CrystalRare":
                        _processedLoot.Add("DustRare", loot.Value * 2);
                        break;
                    case "CrystalEpic":
                        _processedLoot.Add("DustEpic", loot.Value * 2);
                        break;
                }
            }
            _rawLoot.Clear();
        }
        StartCoroutine(CrushCrystals());
    }

    private IEnumerator CrushCrystals()
    {
        while (_processingProgress < 100 && _isAvailable)
        {
            _processingProgress++;
            if (_processingProgress == 100)
            {
                _animator.enabled = false;
            }
            _updateUI.UpdateProgress(_processingProgress);
            yield return new WaitForSeconds(_processTime / 100f);
        }
    }
}