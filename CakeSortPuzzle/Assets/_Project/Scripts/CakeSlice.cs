using UnityEngine;

public class CakeSlice : MonoBehaviour
{
    private CakeSliceType cakeSliceType;

    public CakeSliceType CakeSliceType => cakeSliceType;

    public void SetCakeSliceType(CakeSliceType cakeSliceType)
    {
        this.cakeSliceType = cakeSliceType;
    }
}

public enum CakeSliceType
{
    Blue,
    Green,
    Red
}